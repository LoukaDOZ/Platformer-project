using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject controlsMenu2;
    [SerializeField] private GameObject mainFirstSelected;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject controlsButton;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonSelectound;

    private AudioSource audioSource;

    private InputSettings inputs;
    
    private void OnEnable(){
        inputs = new InputSettings();
        inputs.Menus.Enable();
        inputs.Menus.Echap.performed += ctx => Controls();
        inputs.Menus.Back.performed += ctx => Controls();
    }
    
    private void OnDisable(){
        inputs.Menus.Echap.performed -= ctx => Controls();
        inputs.Menus.Back.performed -= ctx => Controls();
        inputs.Menus.Disable();
    }

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(mainFirstSelected);
        audioSource = GetComponent<AudioSource>();
    }

    public void Controls()
    {
        if (controlsMenu.activeSelf)
        {
            mainMenu.SetActive(true);
            controlsMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(controlsButton);
        }
        
        
        if (controlsMenu2.activeSelf)
        {
            mainMenu.SetActive(true);
            controlsMenu2.SetActive(false);
            EventSystem.current.SetSelectedGameObject(controlsButton);
        }
        
        
        if (settingsMenu.activeSelf)
        {
            mainMenu.SetActive(true);
            settingsMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(settingsButton);
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
