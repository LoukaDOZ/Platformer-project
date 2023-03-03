using UnityEngine;

namespace deprecated
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Fader fader;
        [SerializeField] private SelectionArrow arrow;
        protected InputSettings inputs;

        protected virtual void OnEnable()
        {
            inputs = new InputSettings();
            inputs.Menus.Enable();
            inputs.Menus.MenuUp.performed += ctx => arrow.GoUp();
            inputs.Menus.MenuDown.performed += ctx => arrow.GoDown();
            inputs.Menus.MenuSelect.performed += ctx => arrow.PressButton();
        }

        public void LoadScene(int sceneIndex)
        {
            fader.TransitionToScene(sceneIndex);
        }

        public void Quit()
        {
            Application.Quit();
        }

        protected virtual void OnDisable()
        {
            inputs.Menus.Disable();
            inputs.Menus.MenuUp.performed -= ctx => arrow.GoUp();
            inputs.Menus.MenuDown.performed -= ctx => arrow.GoDown();
            inputs.Menus.MenuSelect.performed -= ctx => arrow.PressButton();
        }
    }
}
