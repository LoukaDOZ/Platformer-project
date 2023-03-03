using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private bool fadeInOnStart = true;

    public static GameManager Instance;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    public void Start()
    {
        if(fadeInOnStart) fader.FadeIn();
    }

    public void LoadScene(int sceneIndex)
    {
        fader.TransitionToScene(sceneIndex);
    }

    public void ReloadScene()
    {
        fader.TransitionToScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
