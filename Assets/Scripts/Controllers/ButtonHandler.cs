﻿using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class ButtonHandler : MonoBehaviour
    {
        public GameObject textField;
        private bool isInUse;
        private bool isPaused;

        public void ChangeText()
        {
            if (!isPaused)
            {
                isInUse = !isInUse;
                textField.GetComponent<Text>().text = isInUse ? "STOP" : "USE";
            }
        }

        public void ChangeTextToFalse()
        {
            isInUse = false;
            textField.GetComponent<Text>().text = "USE";
        }

        public void ChangePauseState()
        {
            isPaused = !isPaused;
        }
    
        public void ChangePauseStateToFalse()
        {
            isPaused = false;
        }
    }
}