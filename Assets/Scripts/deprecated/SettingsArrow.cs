using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace deprecated
{
    public class SettingsArrow : MonoBehaviour
    {
        [SerializeField] private List<Button> buttons;
        [SerializeField] private Button backButton;
        [SerializeField] private List<float> offsets;
        [SerializeField] private float backOffset;
        [SerializeField] [Min(1)] private int nbRows;
        private int buttonIndex = 0;
        private int nbColumns = 0;
        private bool onBackButton = false;
        private void Start()
        {
            nbColumns = Mathf.CeilToInt((float) buttons.Count / nbRows);
            UpdatePos();
        }
    
        public void GoUp()
        {
            if (onBackButton)
            {
                onBackButton = false;
                buttonIndex = 0;
            }
            else if (buttonIndex % nbRows == 0)
                onBackButton = true;
            else
                buttonIndex--;
        
            UpdatePos();
        }
    
        public void GoDown()
        {
            if (onBackButton)
            {
                onBackButton = false;
                buttonIndex = 0;
            }
            else if (buttonIndex % nbRows == nbRows - 1 || buttonIndex == buttons.Count - 1)
                onBackButton = true;
            else
                buttonIndex++;
        
            UpdatePos();
        }
    
        public void GoRight()
        {
            if (Mathf.CeilToInt((float) (buttonIndex + 1) / nbRows) == nbColumns)
                buttonIndex -= nbColumns + 1;
            else
                buttonIndex = Mathf.Min(buttons.Count - 1, buttonIndex + nbRows);
        
            UpdatePos();
        }
    
        public void GoLeft()
        {
            if (Mathf.CeilToInt((float) (buttonIndex + 1) / nbRows) == 1)
                buttonIndex = Mathf.Min(buttonIndex + nbColumns + 1, buttons.Count - 1);
            else
                buttonIndex = Mathf.Max(0, buttonIndex - nbRows);
        
            UpdatePos();
        }

        public void GoToButton(Button button)
        {
            int i = buttons.IndexOf(button);
            onBackButton = i < 0;
            buttonIndex = Mathf.Max(buttonIndex, 0);
        
            UpdatePos();
        }

        private void UpdatePos()
        {
            if(onBackButton)
                transform.position = new Vector2(backButton.transform.position. x + backOffset, backButton.transform.position.y);
            else
                transform.position = new Vector2(buttons[buttonIndex].transform.position. x + offsets[buttonIndex], buttons[buttonIndex].transform.position.y);
        }

        public void PressButton()
        {
            if(onBackButton)
                backButton.onClick.Invoke();
            else
                buttons[buttonIndex].onClick.Invoke();
        }
    }
}