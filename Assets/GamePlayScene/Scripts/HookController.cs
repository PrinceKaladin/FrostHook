using UnityEngine;
using System;
using System.Collections;

public class HookController : MonoBehaviour
{
    public enum HookState
    {
        AtTopReady,
        CastingDown,
        AtBottomIdle,
        Biting,
        ReelingUp
    }

    [Header("Позиции")]
    [SerializeField] private Transform topPosition;
    [SerializeField] private Transform bottomPosition;

    [Header("Время заброса/подъёма")]
    [SerializeField] private float castTime = 2f;
    [SerializeField] private float reelTime = 2f;

    [Header("Поклёвка - ВРЕМЕННЫЕ НАСТРОЙКИ")]
    [SerializeField] private float minBiteDelay = 2f;
    [SerializeField] private float maxBiteDelay = 5f;
    [SerializeField] private float biteDuration = 2.0f;

    [Header("Спрайты крючка")]
    [SerializeField] private Sprite hookWormSprite;
    [SerializeField] private Sprite hookEmptySprite;
    [SerializeField] private Sprite hookFish1Sprite;
    [SerializeField] private Sprite hookFish2Sprite;

    [Header("АНИМАЦИЯ ПОКЛЁВКИ")]
    [SerializeField] private float biteJumpSpeed = 5f;
    [SerializeField] private float jumpHeight = 0.35f;

    [Header("ЗВУКИ")]
    [SerializeField] private AudioClip castSound;        // Звук заброса удочки
    [SerializeField] private AudioClip biteSplashSound;  // Звук "бульк" когда поплавок уходит вниз

    // ========== ПЕРЕМЕННЫЕ СОСТОЯНИЯ ==========
    private HookState state = HookState.AtTopReady;
    private Vector3 bottomPos;
    private Coroutine currentCoroutine;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    private Sprite selectedFishSprite;

    // Для отслеживания движения поплавка вниз (для звука поклёвки)
    private float previousY = 0f;
    private bool wasGoingDown = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Добавляем AudioSource, если его нет
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (topPosition == null)
            topPosition = transform;

        if (bottomPosition == null)
        {
            GameObject bottomObj = new GameObject("HookBottomPosition");
            bottomPosition = bottomObj.transform;
            bottomPosition.position = new Vector3(topPosition.position.x, -8f, topPosition.position.z);
            bottomPosition.parent = transform.parent;
        }

        transform.position = topPosition.position;
        bottomPos = bottomPosition.position;
        previousY = transform.position.y;

        ShowWorm();
    }

    public void OnAction()
    {
        switch (state)
        {
            case HookState.AtTopReady:
                Cast();
                break;
            default:
                Pull();
                break;
        }
    }

    private void Cast()
    {
        if (state != HookState.AtTopReady) return;

        StopAllHookCoroutines();
        state = HookState.CastingDown;

        // Проигрываем звук заброса (если звук включён)
        if (PlayerData.Instance.SoundOn && castSound != null)
        {
            audioSource.PlayOneShot(castSound);
        }

        currentCoroutine = StartCoroutine(CastRoutine());
    }

    private IEnumerator CastRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = bottomPosition.position;

        float elapsed = 0f;
        while (elapsed < castTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / castTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        bottomPos = targetPos;
        state = HookState.AtBottomIdle;
        currentCoroutine = StartCoroutine(BiteSequence());
    }

    private IEnumerator BiteSequence()
    {
        float delay = UnityEngine.Random.Range(minBiteDelay, maxBiteDelay);
        yield return new WaitForSeconds(delay);

        if (state != HookState.AtBottomIdle)
            yield break;

        currentCoroutine = StartCoroutine(BitingRoutine());
    }

    private IEnumerator BitingRoutine()
    {
        state = HookState.Biting;

        float elapsed = 0f;
        while (elapsed < biteDuration)
        {
            elapsed += Time.deltaTime;

            float phase = elapsed * Mathf.PI * biteJumpSpeed;
            float jump = Mathf.Sin(phase) * jumpHeight;

            Vector3 newPos = bottomPos + new Vector3(0f, jump, 0f);
            transform.position = newPos;

            // Проверяем: идёт ли поплавок ВНИЗ?
            bool isGoingDown = newPos.y < previousY;

            // Если начал идти вниз (переход из подъёма в спад) — проигрываем звук поклёвки
            if (isGoingDown && !wasGoingDown && PlayerData.Instance.SoundOn && biteSplashSound != null)
            {
                audioSource.PlayOneShot(biteSplashSound);
            }

            wasGoingDown = isGoingDown;
            previousY = newPos.y;

            yield return null;
        }

        transform.position = bottomPos;
        previousY = bottomPos.y;
        wasGoingDown = false;

        if (state == HookState.Biting)
        {
            state = HookState.AtBottomIdle;
            currentCoroutine = StartCoroutine(BiteSequence());
        }
    }

    private void Pull()
    {
        StopAllHookCoroutines();

        Vector3 startPos = transform.position;

        if (state == HookState.CastingDown || state == HookState.AtBottomIdle)
        {
            // Срыв — пустой крючок
            StartCoroutine(ReelInRoutine(startPos, hookEmptySprite, false, () =>
            {
                GameManager.Instance.GameOver();
            }));
        }
        else if (state == HookState.Biting)
        {
            // Успех — рыба!
            selectedFishSprite = UnityEngine.Random.value < 0.5f ? hookFish1Sprite : hookFish2Sprite;

            StartCoroutine(ReelInRoutine(startPos, null, true, () =>
            {
                GameManager.Instance.OnCatchFish();
                ShowWorm();
            }));
        }
    }

    private IEnumerator ReelInRoutine(Vector3 startPos, Sprite staticSprite, bool isFish, Action onComplete)
    {
        state = HookState.ReelingUp;

        if (isFish && selectedFishSprite != null)
        {
            sr.sprite = selectedFishSprite;
        }
        else if (!isFish && staticSprite != null)
        {
            sr.sprite = staticSprite;
        }

        float elapsed = 0f;
        Vector3 targetPos = topPosition.position;

        while (elapsed < reelTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / reelTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        state = HookState.AtTopReady;
        onComplete?.Invoke();
    }

    private void ShowWorm()
    {
        if (sr && hookWormSprite)
            sr.sprite = hookWormSprite;
    }

    private void StopAllHookCoroutines()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        StopAllCoroutines();

        if (state == HookState.Biting)
        {
            transform.position = bottomPos;
            previousY = bottomPos.y;
            wasGoingDown = false;
        }
    }
}