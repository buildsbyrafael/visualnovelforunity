using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryCapPlayer : MonoBehaviour
{
    public StoryCap currentCap;
    public BottomBarController bottomBarController;
    public ScreenFader screenFader;
    public Image backgroundImage;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private List<SceneBlock> allBlocks;
    private List<string> sceneIds;
    private int currentSceneIndex;

    private Coroutine musicLoopRoutine;

    public void ResetPlayer()
    {
        allBlocks = StoryTextParser.Parse(currentCap.chapterTextAsset);
        sceneIds = new List<string>();

        foreach (var block in allBlocks)
        {
            if (!sceneIds.Contains(block.sceneId))
                sceneIds.Add(block.sceneId);
        }

        currentSceneIndex = 0;

        if (sfxSource != null)
        {
            sfxSource.Stop();
            
            sfxSource.clip = null;
        }
    }

    public void SetupMusic()
    {
        if (musicLoopRoutine != null)
            StopCoroutine(musicLoopRoutine);

        if (musicSource != null && currentCap.backgroundMusicClips.Count > 0)
        {
            musicLoopRoutine = StartCoroutine(PlayMusicPlaylistLoop());
        }
    }

    private IEnumerator PlayMusicPlaylistLoop()
    {
        int index = 0;

        while (true)
        {
            if (index >= currentCap.backgroundMusicClips.Count)
                index = 0;

            var clip = currentCap.backgroundMusicClips[index];

            if (clip != null)
            {
                musicSource.clip = clip;
                musicSource.volume = currentCap.musicVolume;
                musicSource.loop = false;
                musicSource.Play();
                yield return new WaitForSeconds(clip.length);
            }

            index++;
        }
    }

    public IEnumerator PlayCapCoroutine()
    {
        if (allBlocks == null || sceneIds == null || sceneIds.Count == 0)
            yield break;

        while (currentSceneIndex < sceneIds.Count)
        {
            string sceneId = sceneIds[currentSceneIndex];
            var sceneBlocks = allBlocks.FindAll(b => b.sceneId == sceneId && b.IsConditionMet());

            foreach (var block in sceneBlocks)
            {
                var bgName = block.GetEffectiveBackground();
                Sprite bgSprite = null;
                if (!string.IsNullOrEmpty(bgName))
                    bgSprite = Resources.Load<Sprite>($"Backgrounds/{bgName}");

                if (bgSprite != null)
                {
                    bottomBarController.ClearText();
                    yield return new WaitForSeconds(0.3f);
                    yield return StartCoroutine(screenFader.FadeTransition(bgSprite, backgroundImage));
                }

                var rawText = block.GetDisplayText();
                var sentences = DialogueReader.ParseDialogue(rawText);
                bottomBarController.Play(sentences);

                while (bottomBarController.IsRunningScene())
                    yield return null;
            }

            currentSceneIndex++;
        }

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(FadeOutFinalCombined());
    }

    private IEnumerator FadeOutFinalCombined()
    {
        float duration = screenFader.transitionDuration;
        float timer = 0f;

        Color barColor = bottomBarController.barText.color;
        Color nameColor = bottomBarController.personNameText.color;
        Image fader = screenFader.faderImage;

        float musicFadeDuration = duration + 1.5f;
        float musicStartVolume = musicSource != null ? musicSource.volume : 0f;
        float musicTimer = 0f;

        while (timer < duration || musicTimer < musicFadeDuration)
        {
            if (timer < duration)
            {
                float t = timer / duration;
                bottomBarController.barText.color = new Color(barColor.r, barColor.g, barColor.b, 1f - t);
                bottomBarController.personNameText.color = new Color(nameColor.r, nameColor.g, nameColor.b, 1f - t);
                if (fader != null)
                    fader.color = new Color(0f, 0f, 0f, t);
                timer += Time.deltaTime;
            }
            if (musicSource != null && musicTimer < musicFadeDuration)
            {
                float mt = musicTimer / musicFadeDuration;
                musicSource.volume = Mathf.Lerp(musicStartVolume, 0f, mt);
                musicTimer += Time.deltaTime;
            }
            yield return null;
        }

        if (fader != null)
            fader.color = new Color(0f, 0f, 0f, 1f);
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.volume = musicStartVolume;
        }

        bottomBarController.ClearText();
    }

    public AudioSource PlaySFX(string effectName)
    {
        if (string.IsNullOrEmpty(effectName)) return null;

        foreach (var sfx in currentCap.soundEffects)
        {
            if (sfx.effectName == effectName)
            {
                if (sfxSource != null && sfx.effectClip != null)
                {
                    sfxSource.volume = sfx.volume;
                    sfxSource.PlayOneShot(sfx.effectClip);
                    return sfxSource;
                }
                break;
            }
        }
        return null;
    }

    public IEnumerator PlaySFXAndWait(string effectName)
    {
        if (string.IsNullOrEmpty(effectName)) yield break;

        foreach (var sfx in currentCap.soundEffects)
        {
            if (sfx.effectName == effectName)
            {
                if (sfxSource != null && sfx.effectClip != null)
                {
                    sfxSource.volume = sfx.volume;
                    sfxSource.PlayOneShot(sfx.effectClip);
                    yield return new WaitForSeconds(sfx.effectClip.length);
                }
                yield break;
            }
        }
    }

    public List<SceneBlock> GetBlocksForDecision(string decisionId)
    {
        if (currentCap == null || currentCap.chapterTextAsset == null)
            return new List<SceneBlock>();

        List<SceneBlock> allBlocks = StoryTextParser.Parse(currentCap.chapterTextAsset);

        SceneBlock firstA = null;
        SceneBlock firstB = null;

        foreach (var block in allBlocks)
        {
            if (block.conditionDecisionId == decisionId)
            {
                if (block.conditionSelectionId == "A" && firstA == null)
                    firstA = block;
                else if (block.conditionSelectionId == "B" && firstB == null)
                    firstB = block;
            }
        }

        var result = new List<SceneBlock>();
        if (firstA != null) result.Add(firstA);
        if (firstB != null) result.Add(firstB);
        return result;
    }
}