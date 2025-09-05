using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public GameObject menuUI;
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public ScreenFader screenFader;
    public Image menuBackgroundImage;
    public Button startButton;

    public float menuFadeInDelay = 2f;

    private bool hasStarted = false;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonPressed);
            startButton.gameObject.SetActive(true);
            startButton.interactable = false;
        }

        StartCoroutine(PlayMenuSequence());

        if (musicSource != null && menuMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = menuMusic;
            musicSource.volume = 1f;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private IEnumerator PlayMenuSequence()
    {
        hasStarted = false;

        if (screenFader != null && screenFader.faderImage != null)
            screenFader.faderImage.color = new Color(0, 0, 0, 1f);

        yield return new WaitForSeconds(menuFadeInDelay);

        if (menuUI != null)
            menuUI.SetActive(true);

        if (!hasStarted && screenFader != null && menuBackgroundImage != null)
            yield return screenFader.FadeTransition(menuBackgroundImage.sprite, menuBackgroundImage);

        if (startButton != null)
            startButton.interactable = true;
    }

    public void OnStartButtonPressed()
    {
        if (hasStarted) return;
        hasStarted = true;

        StopAllCoroutines();
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();

        if (screenFader != null)
            yield return screenFader.FadeOut();

        if (menuUI != null)
            menuUI.SetActive(false);

        var storyGamePlayer = FindFirstObjectByType<StoryGamePlayer>();
        if (storyGamePlayer != null)
        {
            storyGamePlayer.gameObject.SetActive(true);
            storyGamePlayer.StartGame();
        }
    }
}