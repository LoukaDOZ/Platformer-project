using UnityEngine;

namespace deprecated
{
    public class PauseMenu : MainMenu
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject settingsMenu;

        protected override void OnEnable()
        {
            base.OnEnable();
            inputs.Menus.Echap.performed += ctx => Pause();
        }
    
        private void Pause()
        {
            if(pauseMenu.activeSelf)
                pauseMenu.SetActive(false);
            else if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            } else
                pauseMenu.SetActive(true);
        }
    }
}