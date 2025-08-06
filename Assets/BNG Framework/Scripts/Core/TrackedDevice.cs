using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace BNG
{

    /// <summary>
    /// A simple alternative to the TrackedPoseDriver component.
    /// Feel free to swap this out with a TrackedPoseDriver from the XR Legacy Input Helpers package or using the new Unity Input System
    /// </summary>
    public class TrackedDevice : MonoBehaviour
    {

        public TrackableDevice Device = TrackableDevice.HMD;
        public UpdateMoment UpdateMoment;
        public bool UseOculusPosition;

        protected InputDevice deviceToTrack;

        protected Vector3 initialLocalPosition;
        protected Quaternion initialLocalRotation;

        protected Vector3 currentLocalPosition;
        protected Quaternion currentLocalRotation;

        protected Vector3 currentVelocity;
        protected Vector3 currentAngularVelocity;

        public List<double> UpdateTimeBuffer = new List<double>();
        public List<Vector3> LocalPositionBuffer = new List<Vector3>();
        public List<Vector3> LocalVelocityBuffer = new List<Vector3>();
        public List<Vector3> LocalAngularVelocityBuffer = new List<Vector3>();

        private int _fixedUpdateCount;

        protected virtual void Awake()
        {
            initialLocalPosition = transform.localPosition;
            initialLocalRotation = transform.localRotation;
        }

        protected virtual void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        protected virtual void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        protected virtual void Update()
        {
            RefreshDeviceStatus();

            if (UpdateMoment.HasFlag(UpdateMoment.Update))
                UpdateDevice(UpdateMoment.Update);
        }

        protected virtual void FixedUpdate()
        {
            if (UpdateMoment.HasFlag(UpdateMoment.FixedUpdate))
                UpdateDevice(UpdateMoment.FixedUpdate);
        }

        public virtual void RefreshDeviceStatus()
        {
            if (!deviceToTrack.isValid)
            {

                if (Device == TrackableDevice.HMD)
                {
                    deviceToTrack = InputBridge.Instance.GetHMD();
                }
                else if (Device == TrackableDevice.LeftController)
                {
                    deviceToTrack = InputBridge.Instance.GetLeftController();
                }
                else if (Device == TrackableDevice.RightController)
                {
                    deviceToTrack = InputBridge.Instance.GetRightController();
                }
            }
        }

        public virtual void UpdateDevice(UpdateMoment moment)
        {

            // Check and assign our device status
            if (deviceToTrack.isValid)
            {

                UpdateTimeBuffer.Add(Time.timeAsDouble);
                if (UpdateTimeBuffer.Count > 10)
                {
                    UpdateTimeBuffer.RemoveAt(0);
                }

                if (Device == TrackableDevice.HMD)
                {
                    transform.localPosition = currentLocalPosition = InputBridge.Instance.GetHMDLocalPosition();
                    transform.localRotation = currentLocalRotation = InputBridge.Instance.GetHMDLocalRotation();
                }
                else if (Device == TrackableDevice.LeftController)
                {
                    
                    if (UseOculusPosition)
                    {
                        bool validPosition = OVRInput.GetControllerPositionValid(OVRInput.Controller.LTouch);
                        transform.localPosition = currentLocalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
                        transform.localRotation = currentLocalRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
                    }
                    else
                    {
                        transform.localPosition = currentLocalPosition = InputBridge.Instance.GetControllerLocalPosition(ControllerHand.Left);
                        transform.localRotation = currentLocalRotation = InputBridge.Instance.GetControllerLocalRotation(ControllerHand.Left);
                    }

                    currentAngularVelocity = InputBridge.Instance.GetControllerAngularVelocity(ControllerHand.Left);
                    currentVelocity = InputBridge.Instance.GetControllerVelocity(ControllerHand.Left);
                }
                else if (Device == TrackableDevice.RightController)
                {

                    if (UseOculusPosition)
                    {
                        bool validPosition = OVRInput.GetControllerPositionValid(OVRInput.Controller.RTouch);
                        transform.localPosition = currentLocalPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                        transform.localRotation = currentLocalRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
                    }
                    else
                    {
                        transform.localPosition = currentLocalPosition = InputBridge.Instance.GetControllerLocalPosition(ControllerHand.Right);
                        transform.localRotation = currentLocalRotation = InputBridge.Instance.GetControllerLocalRotation(ControllerHand.Right);
                    }

                    currentAngularVelocity = InputBridge.Instance.GetControllerAngularVelocity(ControllerHand.Right);
                    currentVelocity = InputBridge.Instance.GetControllerVelocity(ControllerHand.Right);

                }

                bool stalePositionUpdate = LocalPositionBuffer.Count > 0 && transform.localPosition == LocalPositionBuffer.Last();
                bool staleVelocityUpdate = LocalVelocityBuffer.Count > 0 && currentVelocity == LocalVelocityBuffer.Last();
                bool staleAngularVelocityUpdate = LocalAngularVelocityBuffer.Count > 0 && currentAngularVelocity == LocalAngularVelocityBuffer.Last();

                if (moment == UpdateMoment.FixedUpdate && Device == TrackableDevice.RightController)
                {
                    if (stalePositionUpdate)
                    {
                        //Debug.Log($"[Tracked Device] <color=red>{Device} stale position update at : {_fixedUpdateCount}</color>");
                    }
                    if(staleVelocityUpdate)
                    {
                        //Debug.Log($"[Tracked Device] <color=green>{Device} stale velocity update at : {_fixedUpdateCount}</color>");
                    }
                    if(staleAngularVelocityUpdate)
                    {
                        //Debug.Log($"[Tracked Device] <color=blue>{Device} stale angular velocity update at : {_fixedUpdateCount}</color>");
                    }

                    if(!stalePositionUpdate && !staleVelocityUpdate && !staleAngularVelocityUpdate)
                    {
                        //Debug.Log($"[Tracked Device] Valid update at: {_fixedUpdateCount}, {Time.time}");
                    }

                    _fixedUpdateCount++;

                }

                LocalVelocityBuffer.Add(currentVelocity);
                LocalAngularVelocityBuffer.Add(currentAngularVelocity);
                LocalPositionBuffer.Add(transform.localPosition);

                if (LocalPositionBuffer.Count > 10)
                {
                    LocalPositionBuffer.RemoveAt(0);
                    LocalAngularVelocityBuffer.RemoveAt(0);
                    LocalVelocityBuffer.RemoveAt(0);
                }

            }
        }

        protected virtual void OnBeforeRender()
        {
            if (UpdateMoment.HasFlag(UpdateMoment.BeforeRender))
                UpdateDevice(UpdateMoment.BeforeRender);
        }
    }

    public enum TrackableDevice
    {
        HMD,
        LeftController,
        RightController
    }

    [System.Flags]
    public enum UpdateMoment
    {
        Update = 1 << 0,
        FixedUpdate = 1 << 1,
        BeforeRender = 1 << 2
    }
}

