using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{

    /// <summary>
    /// Apply Gravity to a CharacterController or RigidBody
    /// </summary>
    public class PlayerGravity : MonoBehaviour
    {

        [Tooltip("If true, will apply gravity to the CharacterController component, or RigidBody if no CC is present.")]

        public bool GravityEnabled = true;

        [Tooltip("Amount of Gravity to apply to the CharacterController or Rigidbody. Default is 'Physics.gravity'.")]
        public Vector3 Gravity = Physics.gravity;

        CharacterController characterController;
        SmoothLocomotion smoothLocomotion;

        Rigidbody playerRigidbody;

        private float _movementY;
        private Vector3 _initialGravityModifier;

        // Save us a null check in FixedUpdate
        private bool _validRigidBody = false;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            smoothLocomotion = GetComponentInChildren<SmoothLocomotion>();
            playerRigidbody = GetComponent<Rigidbody>();

            _validRigidBody = playerRigidbody != null;

            _initialGravityModifier = Gravity;
        }

        // Apply Gravity in LateUpdate to ensure it gets applied after any character movement is applied in Update
        void LateUpdate()
        {

            // Apply Gravity to Character Controller
            if (GravityEnabled && characterController != null && characterController.enabled)
            {
                _movementY += Gravity.y * Time.deltaTime;

                // reset y movement if we are grounded
                //if (characterController.isGrounded)
                //{
                //    _movementY = 0;
                //}

                //bool _isGroundedBefore = characterController.isGrounded;

                //// Default to smooth locomotion
                //if(smoothLocomotion) {
                //    smoothLocomotion.MoveCharacter(new Vector3(0, _movementY, 0) * Time.deltaTime);
                //}
                //// Fallback to character controller
                //else if(characterController) {
                //    characterController.Move(new Vector3(0, _movementY, 0) * Time.deltaTime);
                //}

                ////Reset Y movement if we are grounded
                //if (characterController.isGrounded)
                //{
                //    _movementY = 0;
                //}

                var characterControllerBase = characterController.transform.TransformPoint(characterController.center).y - characterController.height / 2;

                if (characterControllerBase > 0)
                {
                    // Add Gravity 
                    if (characterController)
                    {
                        characterController.Move(new Vector3(0, _movementY, 0) * Time.deltaTime);
                    }
                }

                characterControllerBase = characterController.transform.TransformPoint(characterController.center).y - characterController.height / 2;

                if (characterControllerBase <= 0)
                {
                    var currentPos = characterController.transform.position;
                    //we assume that the floor is at 0, therefore the y position of the character controller shuld he half the height
                    //characterController.transform.position = new Vector3(currentPos.x, characterController.height / 2, currentPos.z);
                    //Vector3 adjustedPosition = new Vector3(currentPos.x, characterController.height / 2, currentPos.z);
                    //Vector3 adjustedPosition = new Vector3(currentPos.x, 0f, currentPos.z);
                    //characterController.transform.parent.position = adjustedPosition;
                    transform.localPosition = new Vector3(0f, characterController.height / 2, 0f);
                    _movementY = 0;
                }

                //Debug.Log($"[{transform.parent.position}, Controller: {transform.position}] The player is grounded: BEFORE = {_isGroundedBefore}, AFTER = {characterController.isGrounded}");
            }
        }

        void FixedUpdate()
        {
            // Apply Gravity to Rigidbody Controller
            if (_validRigidBody && GravityEnabled)
            {
                //playerRigidbody.AddRelativeForce(Gravity, ForceMode.VelocityChange);
                //playerRigidbody.AddForce(new Vector3(0, -Gravity.y * playerRigidbody.mass, 0));

                //if(smoothLocomotion && smoothLocomotion.ControllerType == PlayerControllerType.Rigidbody && smoothLocomotion.GroundContacts < 1) {

                //}

                //playerRigidbody.AddForce(Gravity * playerRigidbody.mass);
            }
        }

        public void ToggleGravity(bool gravityOn)
        {

            GravityEnabled = gravityOn;

            if (gravityOn)
            {
                Gravity = _initialGravityModifier;
            }
            else
            {
                Gravity = Vector3.zero;
            }
        }
    }
}

