using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;      // Your UI element
    [Tooltip("An Image or GameObject you show while loading.")]
    public GameObject placeholder; 

    void Awake()
    {
        // Don’t autoplay, but wait for a real frame
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;

        // Show a placeholder so we never see “nothing”
        if (placeholder) placeholder.SetActive(true);

        // Begin buffering immediately
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        // Hide the placeholder
        if (placeholder) placeholder.SetActive(false);

        // Wire up the RenderTexture & RawImage
        var rt = new RenderTexture(
            (int)vp.clip.width,
            (int)vp.clip.height,
            0);
        vp.targetTexture = rt;
        rawImage.texture = rt;
        rawImage.gameObject.SetActive(true);

        // Now play instantly
        vp.Play();
    }
}