using UnityEngine;
using System.Collections;

public class ControlsIntro : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject controlsPanel;
    public GameObject pressSpacePanel;

    [Header("Timing")]
    public float introDuration = 5f;

    // static so other scripts can check
    public static bool IsIntroActive { get; private set; }

    private IEnumerator Start()
    {
        IsIntroActive = true;                // tell everyone “we’re in intro”
        yield return null;                   // let Awake/Start of other scripts run
        Time.timeScale = 0f;                 // pause

        controlsPanel.SetActive(true);
        pressSpacePanel.SetActive(false);

        yield return new WaitForSecondsRealtime(introDuration);

        pressSpacePanel.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        controlsPanel.SetActive(false);
        pressSpacePanel.SetActive(false);
        Time.timeScale = 1f;                 // un-pause

        IsIntroActive = false;               // intro over
    }
}