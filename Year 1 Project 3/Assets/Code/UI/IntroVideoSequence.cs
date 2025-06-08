using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideoSequence : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [Tooltip("Drop your 4 (or however many) VideoClips here.")]
    public List<VideoClip> videoClips;
    [Tooltip("The VideoPlayer component that will play these clips.")]
    public VideoPlayer videoPlayer;
    [Tooltip("Name of the scene to load after the last video.")]
    public string nextSceneName = "MainScene";

    private int _currentIndex;
    private bool _videoFinished;

    void Start()
    {
        // Auto-assign VideoPlayer if you forgot
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoClips == null || videoClips.Count == 0)
        {
            Debug.LogError("[IntroVideoSequence] No clips assigned!");
            enabled = false;
            return;
        }

        videoPlayer.loopPointReached += OnVideoEnd;
        PlayCurrentVideo();
    }

    void Update()
    {
        if (_videoFinished && Input.GetKeyDown(KeyCode.Space))
        {
            _videoFinished = false;
            _currentIndex++;

            if (_currentIndex < videoClips.Count)
                PlayCurrentVideo();
            else
                SceneManager.LoadScene(nextSceneName);
        }
    }

    private void PlayCurrentVideo()
    {
        videoPlayer.clip = videoClips[_currentIndex];
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        _videoFinished = true;
        Debug.Log($"Video {_currentIndex + 1}/{videoClips.Count} finished. Press SPACE to continue.");
    }
}