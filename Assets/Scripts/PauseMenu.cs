using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlMenu;
    [SerializeField] private GameObject controlMenu2;
    [SerializeField] private GameObject pauseFirstSelected;
    [SerializeField] private GameObject settingsFirstSelected;
    [SerializeField] private GameObject controlFirstSelected;
    [SerializeField] private GameObject control2FirstSelected;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject controlsButton;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonSelectound;

    private AudioSource audioSource;
    private InputSettings inputs;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable(){
        inputs = new InputSettings();
        inputs.Menus.Enable();
        inputs.Menus.Echap.performed += ctx => Pause();
        inputs.Menus.Back.performed += ctx => Back();
    }
    
    private void OnDisable(){
        inputs.Menus.Echap.performed -= ctx => Pause();
        inputs.Menus.Back.performed -= ctx => Back();
        inputs.Menus.Disable();
    }

    public void Pause()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(pauseFirstSelected);
            Time.timeScale = 1;
        }
        else
        {
            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                EventSystem.current.SetSelectedGameObject(settingsButton);
            }
            else if (controlMenu.activeSelf)
            {
                controlMenu.SetActive(false);
                EventSystem.current.SetSelectedGameObject(controlsButton);
            }
            else if (controlMenu2.activeSelf)
            {
                controlMenu2.SetActive(false);
                EventSystem.current.SetSelectedGameObject(controlsButton);
            }
            else
                Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
    }

    public void Settings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsFirstSelected);
    }

    public void Controls()
    {
        pauseMenu.SetActive(false);
        controlMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(controlFirstSelected);
    }

    public void Back()
    {
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(settingsButton);
        } else if (controlMenu.activeSelf)
        {
            controlMenu.SetActive(false);
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(controlsButton);
        } else if (controlMenu2.activeSelf)
        {
            controlMenu2.SetActive(false);
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(controlsButton);
        }
        else if(pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(pauseFirstSelected);
            Time.timeScale = 1;
        }
    }

    public void SwitchControlsMenu()
    {
        if (controlMenu.activeSelf)
        {
            controlMenu.SetActive(false);
            controlMenu2.SetActive(true);
            EventSystem.current.SetSelectedGameObject(control2FirstSelected);
        }
        else
        {
            controlMenu.SetActive(true);
            controlMenu2.SetActive(false);
            EventSystem.current.SetSelectedGameObject(controlFirstSelected);
        }
    }

    public void ButtonClick()
    {
        if (FeedbackController.Instance.SoundEffects)
            audioSource.PlayOneShot(buttonClickSound);
    }

    public void ButtonSelect()
    {
        if (FeedbackController.Instance.SoundEffects)
            audioSource.PlayOneShot(buttonSelectound);
    }
}