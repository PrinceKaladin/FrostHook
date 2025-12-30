using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveUpDistance = 1.2f;
    [SerializeField] private float duration = 1f;

    private TextMeshPro tmp;
    private Vector3 startPos;
    private Coroutine animCoroutine;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        gameObject.SetActive(false);
    }

    public void Show(Vector3 worldStartPosition, string text)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        gameObject.SetActive(true);

        transform.position = worldStartPosition;
        tmp.text = text;
        tmp.alpha = 1f;

        startPos = transform.position;
        animCoroutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 targetPos = startPos + Vector3.up * moveUpDistance;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // плавность
            float ease = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetPos, ease);
            tmp.alpha = Mathf.Lerp(1f, 0f, ease);

            yield return null;
        }

        tmp.alpha = 0f;
        gameObject.SetActive(false);
    }
}
