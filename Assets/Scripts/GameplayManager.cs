using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject winFirstSelected;
    [SerializeField] private GameObject gameFirstSelected;
    [SerializeField] private Bounds winZone;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] [Min(0)] private float gameOverWaitTime;

    public static GameplayManager Instance;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (Physics2D.OverlapBox(winZone.center, winZone.extents, 0, playerMask))
            Win();
    }

    public void Win()
    {
        if (!winMenu.activeSelf)
        {
            Time.timeScale = 0;
            winMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(winFirstSelected);
        }
    }
    
    public void GameOver()
    {
        if (!gameOverMenu.activeSelf)
        {
            Invoke("ShowGameOver", gameOverWaitTime);
        }
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0;
        gameOverMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(gameFirstSelected);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(winZone.center, winZone.extents);
    }
}