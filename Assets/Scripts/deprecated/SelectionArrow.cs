using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace deprecated
{
    public class SelectionArrow : MonoBehaviour
    {
        [SerializeField] private List<Button> buttons;
        [SerializeField] private List<float> offsets;
        private int buttonIndex = 0;
        private void Start()
        {
            UpdatePos();
        }
        public void GoUp()
        {
            buttonIndex -= 1;
            if (buttonIndex < 0)
                buttonIndex = buttons.Count - 1;
            UpdatePos();
        }
        public void GoDown()
        {
            buttonIndex += 1;
            if (buttonIndex >= buttons.Count)
                buttonIndex = 0;
            UpdatePos();
        }

        public void GoToButton(Button button)
        {
            buttonIndex = buttons.IndexOf(button);
            UpdatePos();
        }

        private void UpdatePos()
        {
            transform.position = new Vector2(buttons[buttonIndex].transform.position. x + offsets[buttonIndex], buttons[buttonIndex].transform.position.y);
        }

        public void PressButton()
        {
            buttons[buttonIndex].onClick.Invoke();
        }
    }
}
