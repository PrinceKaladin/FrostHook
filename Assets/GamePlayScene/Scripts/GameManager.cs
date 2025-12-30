using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Элементы")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private FloatingText fishPlusText;

    [Header("Панели")]
    [SerializeField] private GameObject pausePanel;      // Панель паузы
    [SerializeField] private GameObject gameOverPanel;   // Панель Game Over

    [Header("Крючок для +1")]
    [SerializeField] private Transform hookTransform;

    // Кэшируем CanvasGroup для анимации
    private CanvasGroup pauseCanvasGroup;
    private CanvasGroup gameOverCanvasGroup;

    private int score = 0;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Получаем CanvasGroup с панелей (если они назначены)
        if (pausePanel != null)
            pauseCanvasGroup = pausePanel.GetComponent<CanvasGroup>();
        if (gameOverPanel != null)
            gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();

        // Убеждаемся, что панели скрыты при старте
        HidePanelImmediate(pausePanel, pauseCanvasGroup);
        HidePanelImmediate(gameOverPanel, gameOverCanvasGroup);
    }

    private void Update()
    {
        // Пример: пауза по клавише Escape (можно убрать или заменить на кнопку)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                HidePause();
            else
                ShowPause();
        }
    }

    public void OnCatchFish()
    {
        score++;
        if (scoreText != null)
            scoreText.text = score.ToString();

        if (fishPlusText != null && hookTransform != null)
        {
            fishPlusText.Show(hookTransform.position, "+1");
        }
    }

    public void GameOver()
    {
        PlayerData.Instance.BestScore = Mathf.Max(PlayerData.Instance.BestScore, score);
        Debug.Log("Game Over! Score: " + score + " Best: " + PlayerData.Instance.BestScore);

        Time.timeScale = 0f;
        ShowGameOver();
    }

    // ==================== PAUSE ====================

    public void ShowPause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        StartCoroutine(FadeInPanel(pausePanel, pauseCanvasGroup));
    }

    public void HidePause()
    {
        if (!isPaused) return;

        StartCoroutine(FadeOutAndResume(pausePanel, pauseCanvasGroup));
    }

    private IEnumerator FadeOutAndResume(GameObject panel, CanvasGroup cg)
    {
        yield return StartCoroutine(FadePanel(cg, 1f, 0f, 0.3f));

        panel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    // ==================== GAME OVER ====================

    public void ShowGameOver()
    {
        StartCoroutine(FadeInPanel(gameOverPanel, gameOverCanvasGroup));
    }

    // ==================== АНИМАЦИЯ ====================

    private IEnumerator FadeInPanel(GameObject panel, CanvasGroup cg)
    {
        if (panel == null) yield break;

        panel.SetActive(true);

        if (cg == null)
            cg = panel.GetComponent<CanvasGroup>();

        if (cg == null)
        {
            cg = panel.AddComponent<CanvasGroup>();
        }

        yield return StartCoroutine(FadePanel(cg, 0f, 1f, 0.4f));
    }

    private void HidePanelImmediate(GameObject panel, CanvasGroup cg)
    {
        if (panel == null) return;

        panel.SetActive(false);

        if (cg != null)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    private IEnumerator FadePanel(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        if (cg == null) yield break;

        cg.alpha = startAlpha;
        cg.interactable = endAlpha > 0.5f;
        cg.blocksRaycasts = endAlpha > 0.5f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // используем unscaled, чтобы анимация работала при timeScale = 0
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        cg.alpha = endAlpha;
        cg.interactable = endAlpha > 0.5f;
        cg.blocksRaycasts = endAlpha > 0.5f;
    }

}