using UnityEngine;

namespace deprecated
{
    public class SettingsMenu : MainMenu
    {
        [SerializeField] private SettingsArrow settingsArrow;
        protected override void OnEnable()
        {
            inputs = new InputSettings();
            inputs.Menus.Enable();
            inputs.Menus.MenuUp.performed += ctx => settingsArrow.GoUp();
            inputs.Menus.MenuDown.performed += ctx => settingsArrow.GoDown();
            inputs.Menus.MenuRight.performed += ctx => settingsArrow.GoRight();
            inputs.Menus.MenuLeft.performed += ctx => settingsArrow.GoLeft();
            inputs.Menus.MenuSelect.performed += ctx => settingsArrow.PressButton();
        }

        protected override void OnDisable()
        {
            inputs.Menus.Disable();
            inputs.Menus.MenuUp.performed -= ctx => settingsArrow.GoUp();
            inputs.Menus.MenuDown.performed -= ctx => settingsArrow.GoDown();
            inputs.Menus.MenuRight.performed -= ctx => settingsArrow.GoRight();
            inputs.Menus.MenuLeft.performed -= ctx => settingsArrow.GoLeft();
            inputs.Menus.MenuSelect.performed -= ctx => settingsArrow.PressButton();
        }
    }
}