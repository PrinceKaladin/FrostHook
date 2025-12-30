using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject Education;

    private CanvasGroup menuCanvasGroup;
    private CanvasGroup settingsCanvasGroup;
    private CanvasGroup educationCanvasGroup;

    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine currentCoroutine = null;

    private void Awake()
    {
        menuCanvasGroup = Menu.GetComponent<CanvasGroup>();
        settingsCanvasGroup = Settings.GetComponent<CanvasGroup>();
        educationCanvasGroup = Education.GetComponent<CanvasGroup>();

        HideAllImmediately();
        ShowMenu();
    }

    public void ShowMenu()
    {
        StopCurrentAnimation();
        currentCoroutine = StartCoroutine(FadeToPanel(menuCanvasGroup, settingsCanvasGroup, educationCanvasGroup));
    }

    public void ShowSettings()
    {
        StopCurrentAnimation();
        currentCoroutine = StartCoroutine(FadeToPanel(settingsCanvasGroup, menuCanvasGroup, educationCanvasGroup));
    }

    public void ShowEducation()
    {
        StopCurrentAnimation();
        currentCoroutine = StartCoroutine(FadeToPanel(educationCanvasGroup, menuCanvasGroup, settingsCanvasGroup));
    }

    private void StopCurrentAnimation()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    private IEnumerator FadeToPanel(CanvasGroup target, CanvasGroup other1, CanvasGroup other2)
    {
        yield return StartCoroutine(FadeOut(other1));
        yield return StartCoroutine(FadeOut(other2));
        yield return StartCoroutine(FadeIn(target));
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float elapsed = 0f;
        canvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null || canvasGroup.alpha == 0f)
        {
            if (canvasGroup != null) canvasGroup.gameObject.SetActive(false);
            yield break;
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);
    }

    private void HideAllImmediately()
    {
        SetPanelState(Menu, menuCanvasGroup, false);
        SetPanelState(Settings, settingsCanvasGroup, false);
        SetPanelState(Education, educationCanvasGroup, false);
    }

    private void SetPanelState(GameObject go, CanvasGroup cg, bool active)
    {
        if (go != null)
        {
            go.SetActive(active);
            if (cg != null)
            {
                cg.alpha = active ? 1f : 0f;
                cg.blocksRaycasts = active;
                cg.interactable = active;
            }
        }
    }
}