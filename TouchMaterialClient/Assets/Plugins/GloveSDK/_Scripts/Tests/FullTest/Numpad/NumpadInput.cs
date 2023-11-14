/******************************************************************************
* Copyright © NeuroDigital Technologies, S.L. 2018							  *
* Licensed under the Apache License, Version 2.0 (the "License");			  *
* you may not use this file except in compliance with the License.			  *
* You may obtain a copy of the License at 									  *
* http://www.apache.org/licenses/LICENSE-2.0								  *
* Unless required by applicable law or agreed to in writing, software		  *
* distributed under the License is distributed on an "AS IS" BASIS,			  *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.	  *
* See the License for the specific language governing permissions and		  *
* limitations under the License.										      *
*******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace NDTest
{
    // <summary>
    // Numpad screen
    // </summary>
    public class NumpadInput : MonoBehaviour
    {
        /// <summary>
        /// Visible text
        /// </summary>
        public Text inputText;
        /// <summary>
        /// Entering the correct code, this chest is opened/closed
        /// </summary>
        public PasswordChest chest;
        private string[] digits;

        void Start()
        {
            ResetNumpad();
        }

        void Update()
        {
            string currentText = "";
            for (int i = 0; i < digits.Length; i++)
            {
                if (i < digits.Length - 1)
                    currentText += digits[i] + " ";
                else
                    currentText += digits[i];
            }

            inputText.text = currentText;
        }

        /// <summary>
        /// Check if can be appended any character or send the current sequence
        /// </summary>
        /// <param name="buttonInput">New character to append</param>
        public void EnterText(string buttonInput)
        {
            if (buttonInput.ToLower().Equals("enter"))
            {
                string input = "";
                for (int i = 0; i < digits.Length; i++)
                {
                    input += digits[i];
                }
                chest.Open(input);
                ResetNumpad();
            }
            else
            {
                for (int i = 0; i < digits.Length; i++)
                {
                    if (digits[i].Equals("X"))
                    {
                        digits[i] = buttonInput;
                        break;
                    }
                }
            }
        }

        private void ResetNumpad()
        {
            inputText.text = "X X X X";
            digits = inputText.text.Split(' ');
        }
    }
}