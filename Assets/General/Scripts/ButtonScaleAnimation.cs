using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonScaleAnimation : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    private Vector3 normalScale = Vector3.one;
    private Vector3 pressedScale = Vector3.one * 0.95f;

    [SerializeField] private float animationDuration = 0.1f;

    private Coroutine scaleRoutine;

    private void Awake()
    {
        transform.localScale = normalScale;
    }

    private void OnEnable()
    {
        transform.localScale = normalScale;

        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
            scaleRoutine = null;
        }
    }

    private void OnDisable()
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
            scaleRoutine = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartScaleAnimation(pressedScale);

        if (PlayerData.Instance != null &&
            PlayerData.Instance.SoundOn &&
            SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButton();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartScaleAnimation(normalScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleAnimation(normalScale);
    }

    private void StartScaleAnimation(Vector3 targetScale)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleTo(targetScale));
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale = Vector3.Lerp(startScale, target, t);
            yield return null;
        }

        transform.localScale = target;
        scaleRoutine = null;
    }
}
