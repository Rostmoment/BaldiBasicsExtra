using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace BBE.Helpers
{
    public class TextInput : MonoBehaviour
    {
        public TMP_Text tmp;

        private string value = "";

        public int MaxLen;

        private bool UseField = false;

        public bool CanUseLetters = true;

        public bool CanUseNumbers = true;

        public string Tip;

        public string Value => value;


        public void Initialize(TMP_Text tmp)
        {
            this.tmp = tmp;
            UpdateText();
        }

        public void SetActivity(bool activity)
        {
            UseField = activity;
        }

        private void Update()
        {
            tmp.autoSizeTextContainer = false;
            tmp.autoSizeTextContainer = true;
            if (!UseField) return;
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (value.Length > 0)
                {
                    value = value.Remove(value.Length - 1, 1);
                }
                UpdateText();
            }
            if (value.Length >= MaxLen) return;
            else if ((Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) && Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl))
            {
                value = "";
                UpdateText();
            }
            else if (Input.inputString.Length > 0 && !Input.GetKey(KeyCode.Backspace))
            {
                if ((char.IsLetter(Input.inputString[0]) && CanUseLetters) || (char.IsNumber(Input.inputString[0]) && CanUseNumbers))
                {
                    value += Input.inputString[0];
                    UpdateText();
                }
            }
            else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.V))
            {
                string systemCopyBuffer = GUIUtility.systemCopyBuffer;
                value = systemCopyBuffer;
                UpdateText();
            }
        }

        public void SetValue(string value)
        {
            this.value = value;
            UpdateText();
        }

        private void UpdateText()
        {
            tmp.text = Singleton<LocalizationManager>.Instance.GetLocalizedText(Tip) + value;
        }
    }

}
