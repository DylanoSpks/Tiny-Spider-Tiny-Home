using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    void Awake()
    {
        // Make absolutely sure we’re not stuck in a paused state
        PauseMenu.IsPaused = false;
        Time.timeScale = 1f;
    }
}