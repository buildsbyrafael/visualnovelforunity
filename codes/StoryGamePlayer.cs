using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StoryGamePlayer : MonoBehaviour
{
    public List<StoryCap> gameChapters;
    public StoryCapPlayer capPlayer;
    public VideoPlayer loadingVideoPlayer;
    public RawImage loadingRawImage;
    public ScreenFader screenFader;

    private int currentChapterIndex = 0;
    
    public static bool IsInputLocked = false;

    public void StartGame()
    {
        StartCoroutine(PlayGame());
    }

    private IEnumerator PlayGame()
    {
        if (loadingVideoPlayer != null)
        {
            yield return PlayLoadingVideo();
        }

        while (currentChapterIndex < gameChapters.Count)
        {
            StoryCap chapter = gameChapters[currentChapterIndex];

            if (chapter != null)
            {
                capPlayer.currentCap = chapter;
                capPlayer.ResetPlayer();
                capPlayer.gameObject.SetActive(true);
                capPlayer.SetupMusic();
                yield return StartCoroutine(capPlayer.PlayCapCoroutine());
                capPlayer.gameObject.SetActive(false);
            }

            bool isLastChapter = currentChapterIndex == gameChapters.Count - 1;

            if (!isLastChapter && loadingVideoPlayer != null)
            {
                yield return PlayLoadingVideo();
            }

            currentChapterIndex++;
        }
    }

    private IEnumerator PlayLoadingVideo()
    {
        IsInputLocked = true;

        yield return screenFader.FadeOut();

        loadingVideoPlayer.Stop();
        loadingRawImage.texture = loadingVideoPlayer.targetTexture;
        loadingRawImage.gameObject.SetActive(true);
        loadingVideoPlayer.gameObject.SetActive(true);

        loadingVideoPlayer.Prepare();

        while (!loadingVideoPlayer.isPrepared)
        {
            yield return null;
        }

        loadingVideoPlayer.Play();

        while (loadingVideoPlayer.isPlaying)
        {
            yield return null;
        }

        loadingVideoPlayer.Stop();

        loadingRawImage.gameObject.SetActive(false);
        loadingVideoPlayer.gameObject.SetActive(false);
        loadingRawImage.texture = null;

        screenFader.faderImage.color = new Color(0, 0, 0, 1);

        IsInputLocked = false;
    }
}