using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideoSequence : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public VideoPlayer videoPlayer;
    public List<VideoClip> videoClips;
    public GameObject continuePromptUI;
    public string nextSceneName = "SampleScene";

    [Header("Delay Settings")]
    public float promptDelay = 1.0f;

    private int _currentIndex = 0;
    private bool _videoFinished = false;
    private Coroutine _delayCoroutine;

    void Start()
    {
        if (videoPlayer == null) 
            videoPlayer = GetComponent<VideoPlayer>();

        continuePromptUI.SetActive(false);
        videoPlayer.loopPointReached += OnVideoEnd;
        PlayCurrentVideo();
    }

    void Update()
    {
        // ESC skips the entire intro
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If a prompt delay is still running, stop it
            if (_delayCoroutine != null)
                StopCoroutine(_delayCoroutine);

            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // SPACE advances after video-end + delay
        if (_videoFinished && Input.GetKeyDown(KeyCode.Space))
        {
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
        continuePromptUI.SetActive(false);
        videoPlayer.clip = videoClips[_currentIndex];
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Start the delayed prompt coroutine
        _delayCoroutine = StartCoroutine(ShowPromptWithDelay());
    }

    IEnumerator ShowPromptWithDelay()
    {
        yield return new WaitForSeconds(promptDelay);
        continuePromptUI.SetActive(true);
        _videoFinished = true;
    }
}
