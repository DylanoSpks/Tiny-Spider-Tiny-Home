using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject PausePanel;
    
    //Update is called each frame
    void Update()
    {
        
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void resume()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }
}