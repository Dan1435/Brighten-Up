using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using UnityEngine.UI;

using UnityEngine.SceneManagement;



public class CharchterManager : MonoBehaviour

{

    // Static instance allows AdsManager to call ExecuteRevive() directly

    public static CharchterManager Instance;



    [Header("Core Lists")]

    public List<Character> characters = new List<Character>();

    public List<Rooms> rooms = new List<Rooms>();

    public ScoreDisplay scoreDisplay;



    [Header("Timer UI")]

    public Image timeBar;

    public float switchTime = 3f;

    private float timer;



    [HideInInspector] public bool isGameOver = true;

    [HideInInspector] public bool isGamePlaying = false;

    public static int score = 0;

    private static int deathCount = 0; // Static to persist through scene reloads



    [Header("Menu Setup")]

    public GameObject menuCanvas;

    public GameObject instructionsPanel;

    public GameObject gameUI;



    [Header("Pause UI")]

    public GameObject pauseOverlay;

    public Button pauseButton;



    [Header("Revive & Ad UI")]

    public GameObject continueButton;   // The "Watch Ad" button

    public GameObject revivePausePanel; // The "Ready?" panel after reviving



    [Header("Fade Groups")]

    public CanvasGroup gameWorldGroup;

    public CanvasGroup gameOverGroup;

    public CanvasGroup fadeOverlayGroup;

    public CanvasGroup loadingSplashGroup;

    public float fadeDuration = 0.25f;



    [Header("End Screen UI")]

    public GameObject gameOverScreen;

    public TMPro.TextMeshProUGUI currentScoreDisplay;

    public TMPro.TextMeshProUGUI bestScoreDisplay;



    [Header("Audio")]

    public AudioSource sfxSource;

    public AudioClip playButtonSound;

    public AudioClip correctTapSound;

    public AudioClip incorrectTapSound;

    public AudioClip switchRoomSound;



    void Awake()

    {

        if (Instance == null) Instance = this;

    }



    void Start()

    {

        if (loadingSplashGroup != null)

        {

            loadingSplashGroup.gameObject.SetActive(true);

            loadingSplashGroup.alpha = 1;

            loadingSplashGroup.blocksRaycasts = true;

        }



        // Reset the Rewarded Ad limit for the new game session

        if (AdsManager.Instance != null) AdsManager.Instance.ResetRevives();



        Time.timeScale = 0;

        isGameOver = true;

        isGamePlaying = false;

        score = 0;



        if (scoreDisplay != null) scoreDisplay.UpdateScoreUI(0);



        // UI Initial State

        if (menuCanvas != null) menuCanvas.SetActive(true);

        if (instructionsPanel != null) instructionsPanel.SetActive(false);

        if (gameUI != null) gameUI.SetActive(false);

        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        if (pauseOverlay != null) pauseOverlay.SetActive(false);

        if (revivePausePanel != null) revivePausePanel.SetActive(false);



        if (pauseButton != null)

        {

            pauseButton.interactable = true;

            pauseButton.gameObject.SetActive(true);

        }



        if (gameWorldGroup != null) { gameWorldGroup.alpha = 1; gameWorldGroup.gameObject.SetActive(true); }

        if (gameOverGroup != null) { gameOverGroup.alpha = 1; gameOverGroup.gameObject.SetActive(false); }

        if (fadeOverlayGroup != null) { fadeOverlayGroup.alpha = 0; fadeOverlayGroup.gameObject.SetActive(false); }



        StartCoroutine(InitialSplashFade());

    }



    private IEnumerator InitialSplashFade()

    {

        yield return new WaitForSecondsRealtime(0.5f);

        if (loadingSplashGroup != null)

        {

            yield return StartCoroutine(FadeCanvas(loadingSplashGroup, 1, 0, 1.0f));

            loadingSplashGroup.blocksRaycasts = false;

            loadingSplashGroup.gameObject.SetActive(false);

        }

    }



    public void ShowInstructions()

    {

        PlaySFX(playButtonSound);

        if (menuCanvas != null) menuCanvas.SetActive(false);

        if (instructionsPanel != null) instructionsPanel.SetActive(true);

    }



    public void TogglePause(bool pause)

    {

        if (isGameOver) return;

        PlaySFX(playButtonSound);

        Time.timeScale = pause ? 0 : 1;

        foreach (Character c in characters) { if (c != null) c.gameObject.SetActive(!pause); }

        if (pauseOverlay != null) pauseOverlay.SetActive(pause);

    }



    public void StartActualGameplay()

    {

        PlaySFX(playButtonSound);

        Time.timeScale = 1;

        isGameOver = false;

        isGamePlaying = true;



        if (menuCanvas != null) menuCanvas.SetActive(false);

        if (instructionsPanel != null) instructionsPanel.SetActive(false);

        if (gameUI != null) gameUI.SetActive(true);



        foreach (Character c in characters)

        {

            if (c != null)

            {

                c.gameObject.SetActive(true);

                c.UpdateCharacterAppearance(false);

            }

        }

        ResetTimer();

        MoveCharacters();

    }



    void Update()

    {

        if (isGameOver || Time.timeScale <= 0) return;



        timer -= Time.deltaTime;

        if (timeBar != null) timeBar.fillAmount = Mathf.Clamp01(timer / switchTime);



        if (timer <= 0)

        {

            if (CheckAllRoomsForErrors(true)) StartGameOverSequence();

            else NextRound();

        }

    }



    public void StartGameOverSequence()

    {

        if (isGameOver) return;

        isGameOver = true;

        isGamePlaying = false;



        deathCount++;



        if (AdsManager.Instance != null)

        {

            AdsManager.Instance.ShowBanner();



            // Logic to show/hide the Continue button based on ad readiness

            if (continueButton != null)

            {

                bool canRevive = AdsManager.Instance.reviveCount < AdsManager.Instance.maxRevives;

                bool adIsReady = AdsManager.Instance.IsRewardedAdLoaded();

                continueButton.SetActive(canRevive && adIsReady);

            }

        }



        if (gameUI != null) gameUI.SetActive(false);

        if (pauseButton != null) pauseButton.interactable = false;



        StartCoroutine(GameOverRoutine());

    }



    public void OnContinueButtonPressed()

    {

        if (AdsManager.Instance != null)

        {

            // Triggers the Rewarded Ad

            AdsManager.Instance.ShowRewarded();

            if (continueButton != null) continueButton.SetActive(false);

        }

    }



    public void ExecuteRevive()

    {

        if (AdsManager.Instance != null) AdsManager.Instance.HideBanner();

        if (gameOverScreen != null) gameOverScreen.SetActive(false);



        isGameOver = false;

        isGamePlaying = true;



        // IMPORTANT: Reset timer so player doesn't die immediately on resume

        ResetTimer();



        if (gameUI != null) gameUI.SetActive(true);

        if (gameWorldGroup != null) gameWorldGroup.gameObject.SetActive(true);

        if (pauseButton != null) pauseButton.interactable = true;



        // Show "Ready?" panel and pause time to give player a moment

        Time.timeScale = 0;

        if (revivePausePanel != null) revivePausePanel.SetActive(true);

    }



    public void ResumeFromRevive()

    {

        if (revivePausePanel != null) revivePausePanel.SetActive(false);

        Time.timeScale = 1;

        PlaySFX(playButtonSound);

    }



    public void RetryGame()

    {

        if (AdsManager.Instance != null)

        {

            AdsManager.Instance.HideBanner();

            // Show Interstitial every 5 games

            if (deathCount > 0 && deathCount % 5 == 0)

            {

                AdsManager.Instance.ShowInterstitial();

            }

        }

        Time.timeScale = 1;

        score = 0;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }



    public void GoToMainMenu()

    {

        PlaySFX(playButtonSound);

        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }



    public void UpdateCharactersInRoom(Rooms room)

    {

        if (isGameOver) return;

        foreach (Character c in characters) { if (c != null && c.gameObject.activeSelf && c.currentRoom == room) c.UpdateCharacterAppearance(true); }

        if (IsRoomIncorrect(room)) { PlaySFX(incorrectTapSound); room.FlashRed(); StartGameOverSequence(); }

        else { AddScore(room.IsLit); PlaySFX(correctTapSound); if (!CheckAllRoomsForErrors(false)) NextRound(); }

    }



    public void AddScore(bool isTurningOn) { score += isTurningOn ? 2 : 4; if (scoreDisplay != null) scoreDisplay.UpdateScoreUI(score); DifficultyRamp(); }



    private bool IsRoomIncorrect(Rooms r)

    {

        bool needsL = false;

        foreach (Character c in characters) { if (c != null && c.gameObject.activeSelf && c.currentRoom == r && !c.isDog) { needsL = true; break; } }

        return (needsL && !r.IsLit) || (!needsL && r.IsLit);

    }



    public void ShareScore()

    {

        PlaySFX(playButtonSound);

        string appStoreLink = "https://vizulation.com/brightenup";

        string message = "I just scored " + score + " in #BrightenUp!\nBut you can do better than that 😏\n\n" + appStoreLink;

        Application.OpenURL("https://twitter.com/intent/tweet?text=" + System.Uri.EscapeDataString(message));

    }

    public void ShareApp()
    {
        PlaySFX(playButtonSound);

        string appStoreLink = "https://apps.apple.com/us/app/brighten-up/id6759843847";

        Application.OpenURL(appStoreLink);
    }



    private bool CheckAllRoomsForErrors(bool viz)

    {

        bool err = false;

        foreach (Rooms r in rooms) { if (IsRoomIncorrect(r)) { if (viz) { PlaySFX(incorrectTapSound); r.FlashRed(); } err = true; } }

        return err;

    }



    public void MoveCharacters()

    {

        PlaySFX(switchRoomSound);

        int roomCount = rooms.Count;

        if (roomCount > 0)

        {

            foreach (Character c in characters)

            {

                if (c != null) c.SetRoom(rooms[Random.Range(0, roomCount)]);

            }

        }

    }



    public void ResetTimer() { timer = switchTime; }

    public void NextRound() { if (isGameOver) return; ResetTimer(); MoveCharacters(); }



    private void DifficultyRamp()

    {

        if (score >= 1000) switchTime = 1.5f;

        else if (score >= 800) switchTime = 1.8f;

        else if (score >= 600) switchTime = 2.0f;

        else if (score >= 400) switchTime = 2.3f;

        else if (score >= 200) switchTime = 2.5f;

        else if (score >= 100) switchTime = 2.7f;

        else switchTime = 3.0f;

    }



    private IEnumerator GameOverRoutine()

    {

        yield return new WaitForSecondsRealtime(0.3f);

        if (fadeOverlayGroup != null)

        {

            fadeOverlayGroup.gameObject.SetActive(true);

            fadeOverlayGroup.blocksRaycasts = true;

            yield return StartCoroutine(FadeCanvas(fadeOverlayGroup, 0, 1, fadeDuration));

        }

        if (gameWorldGroup != null) gameWorldGroup.gameObject.SetActive(false);

        if (gameOverScreen != null)

        {

            gameOverScreen.SetActive(true);

            int hi = PlayerPrefs.GetInt("HighScore", 0);

            if (score > hi) PlayerPrefs.SetInt("HighScore", score);

            if (currentScoreDisplay != null) currentScoreDisplay.text = score.ToString();

            if (bestScoreDisplay != null) bestScoreDisplay.text = PlayerPrefs.GetInt("HighScore", 0).ToString();

        }

        if (fadeOverlayGroup != null)

        {

            yield return StartCoroutine(FadeCanvas(fadeOverlayGroup, 1, 0, fadeDuration));

            fadeOverlayGroup.blocksRaycasts = false;

            fadeOverlayGroup.gameObject.SetActive(false);

        }

        Time.timeScale = 0;

    }



    private IEnumerator FadeCanvas(CanvasGroup cg, float s, float e, float d)

    {

        float el = 0;

        while (el < d) { el += Time.unscaledDeltaTime; cg.alpha = Mathf.Lerp(s, e, el / d); yield return null; }

        cg.alpha = e;

    }



    private void OnApplicationFocus(bool hasFocus)

    {

        if (!hasFocus && isGamePlaying && !isGameOver) { TogglePause(true); }

    }



    private void PlaySFX(AudioClip clip)

    {

        if (sfxSource != null && clip != null)

        {

            sfxSource.PlayOneShot(clip);

        }

    }

}
