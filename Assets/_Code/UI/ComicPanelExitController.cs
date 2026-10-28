using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class ComicPanelExitController : MonoBehaviour
{
    public GameObject skipBtn;

    public ComicPagePanel comicPage;
    public CanvasGroup textGrp;
    public TypewriterText textRevealer;

    public float delay = 1f;
    public CanvasGroup howToPlayGrp;

    public static event Action stopPanelAnimation;

    // very temporary fix,  change afterwards //////////////////////////////
    public void EnableSkipButton()
    {
        StartCoroutine(ShowSkipButtonAfterDelay());
    }

    IEnumerator ShowSkipButtonAfterDelay()
    {
        yield return new WaitForSeconds(0.8f);
        skipBtn.SetActive(true);
    }

    public void SkipComicPanelCutscene()
    {
        stopPanelAnimation?.Invoke();
        ExitComicPanel();
    }
    ////////////////////////////////////////////////////////////////////////

    public void ExitComicPanel()
    {
        StartCoroutine(ExitComicPanelAfterDelay());
    }

    IEnumerator ExitComicPanelAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        textGrp.gameObject.SetActive(true);
        textGrp.DOFade(1f, 1f).OnComplete(() => {
            comicPage.gameObject.SetActive(false);
            textRevealer.StartWriting();
        });
    }

    public void ShowHowToPlay()
    {
        StartCoroutine(ShowHowToPlayAfterDelay());
    }

    IEnumerator ShowHowToPlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        howToPlayGrp.gameObject.SetActive(true);
        howToPlayGrp.DOFade(1f, 1f).OnComplete(() =>
        {
            textGrp.gameObject.SetActive(false);
        });
    }
}
