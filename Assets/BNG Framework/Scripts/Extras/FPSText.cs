using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BNG {

    /// <summary>
    /// A simple script to display FPS onto a Text label
    /// </summary>
    public class FPSText : MonoBehaviour {
        public TextMeshProUGUI text;
        float deltaTime = 0.0f;

        void Start() {
        }

        void Update() {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            text.text = Math.Ceiling(1.0f / deltaTime) + " FPS";
        }

        void OnGUI() {
            text.text = Math.Ceiling(1.0f / deltaTime) + " FPS";
        }
    }
}