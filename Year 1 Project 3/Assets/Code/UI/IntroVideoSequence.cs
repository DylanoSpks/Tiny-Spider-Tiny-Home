using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideoSequence : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public List<VideoClip> videoClips;
    public GameObject continuePromptUI;
    public string nextSceneName = "MainScene";
    public float promptDelay = 1f;

    private int _currentIndex;
    private bool _videoFinished;

    void Start()
    {
        // Auto-assign VideoPlayer if left empty
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoClips == null || videoClips.Count == 0)
        {
            Debug.LogError("No VideoClips assigned!");
            enabled = false;
            return;
        }

        // Hide the prompt at start
        if (continuePromptUI != null)
            continuePromptUI.SetActive(false);

        // Hook end-of-clip
        videoPlayer.loopPointReached += OnVideoEnd;

        PlayCurrentVideo();
    }

    void Update()
    {
        // On Space, only when a clip has finished
        if (_videoFinished && Input.GetKeyDown(KeyCode.Space))
        {
            // Hide prompt
            if (continuePromptUI != null)
                continuePromptUI.SetActive(false);

            _videoFinished = false;
            _currentIndex++;

            if (_currentIndex < videoClips.Count)
                PlayCurrentVideo();
            else
                SceneManager.LoadScene(nextSceneName);
        }
    }

    void PlayCurrentVideo()
    {
        videoPlayer.clip = videoClips[_currentIndex];
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // start delay before showing prompt
        StartCoroutine(ShowPromptWithDelay());
    }

    IEnumerator ShowPromptWithDelay()
    {
        // wait before showing the UI
        yield return new WaitForSeconds(promptDelay);
        // now enable UI and allow Space to work
        continuePromptUI.SetActive(true);
        _videoFinished = true;
    }
}