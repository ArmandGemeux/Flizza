using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkTonic.MasterAudio;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI clickCountText;
    [SerializeField] private TextMeshProUGUI clickCountTextAtTheEnd;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerTextAtTheEnd;

    [Header("CanvasGroup")]
    [SerializeField] private CanvasGroup endOfGameResults;
    [SerializeField] private CanvasGroup menuPause;

    [Header("Sprites")]
    public Sprite musicOn;
    public Sprite musicOff;

    [Header("Images")]
    public Image muteSoundImage;

    bool isMusicMuted = false;

    #region Singleton
    public static UIManager s_Singleton;

    private void Awake()
    {
        if (s_Singleton != null)
        {
            Destroy(this);
        }
        else
        {
            s_Singleton = this;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = GameManager.s_Singleton.currentGameTimer.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        TimerUpdate();
    }

    public void ClickCounterUpdate(int clickedTime)
    {
        clickCountText.text = clickedTime.ToString();
    }

    void TimerUpdate()
    {
        string min = Mathf.Floor(GameManager.s_Singleton.currentGameTimer / 60).ToString("00");
        string sec = Mathf.Floor(GameManager.s_Singleton.currentGameTimer % 60).ToString("00");
        timerText.text = min + " : " + sec;
    }

    #region Buttons
    public void OnClickRestartButton()
    {
        StartCoroutine(RestartCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        Debug.Log("Reset the game");
        PlayButtonSoundOnClick();

        yield return new WaitForSeconds(0.5f);

        GameManager.s_Singleton.ResetTheGame();
    }

    public void OnMuteMusicButtonClick()
    {
        PlayButtonSoundOnClick();

        if (!isMusicMuted)
        {
            muteSoundImage.sprite = musicOff;
            MasterAudio.MuteEverything();
            isMusicMuted = true;
        }
        else if (isMusicMuted)
        {
            muteSoundImage.sprite = musicOn;
            MasterAudio.UnmuteEverything();
            isMusicMuted = false;
        }
    }

    public void OnPauseButtonClick()
    {
        PlayButtonSoundOnClick();

        if (!GameManager.s_Singleton.gameIsPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void OnResumeButtonClick()
    {
        PlayButtonSoundOnClick();
        ResumeGame();
    }

    public void OnQuitButtonClick()
    {
        PlayButtonSoundOnClick();
        Debug.Log("Quit Game");
        Application.Quit();
    }

    void PlayButtonSoundOnClick()
    {
        MasterAudio.PlaySoundAndForget("ClickButtonSound");
    }
    #endregion

    #region Pause or Resume Game
    void PauseGame()
    {
        StartCoroutine(FadeCanvasGroup(menuPause, menuPause.alpha, 1));
        menuPause.blocksRaycasts = true;

        GameManager.s_Singleton.gameIsPaused = true;
        
        /*
        MasterAudio.MutePlaylist();
        isMusicMuted = true;
        */
    }

    void ResumeGame()
    {
        StartCoroutine(FadeCanvasGroup(menuPause, menuPause.alpha, 0));
        menuPause.blocksRaycasts = false;

        GameManager.s_Singleton.gameIsPaused = false;
        
        /*
        MasterAudio.UnmutePlaylist();
        isMusicMuted = false;
        */
    }
    #endregion
    
    #region CanvasFading
    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.5f)
    {
        float _timerStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timerStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timerStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }

    public void FadeInEndOfGameResults()
    {
        StartCoroutine(FadeCanvasGroup(endOfGameResults, endOfGameResults.alpha, 1));
        clickCountTextAtTheEnd.text = "En  " + GameManager.s_Singleton.clickCount + " clics.";
        timerTextAtTheEnd.text = "Temps" + " : " + timerText.text +  ".";
    }

    public void FadeOutEndOfGameResults()
    {
        StartCoroutine(FadeCanvasGroup(endOfGameResults, endOfGameResults.alpha, 0));
    }
    #endregion
}
