using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkTonic.MasterAudio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI clickCountText;
    [SerializeField] private TextMeshProUGUI clickCountTextAtTheEnd;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerTextAtTheEnd;
    
    [Header("CanvasGroup")]
    [SerializeField] private CanvasGroup endOfGameResultsPopup;
    [SerializeField] private CanvasGroup menuPausePopup;
    [SerializeField] private CanvasGroup orderOrPlayPopup;
    [SerializeField] private CanvasGroup freePizzaCodePopup;
    [SerializeField] private CanvasGroup deliveryPizzaPopup;


    [Header("Win Count Parameters")]
    public Button getAPizzaButton;
    
    [Header("Sprites")]
    public Sprite musicOn;
    public Sprite musicOff;

    [Header("Images")]
    public Image muteSoundImage;

    public bool isMusicMuted;

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
        InitializeMusicState();
        timerText.text = GameManager.s_Singleton.currentGameTimer.ToString();

        if (GameManager.s_Singleton.loadCount <= 1 && !GameManager.hasOrdered)
            FadeInOrderOrPlayPopup();
        else
            orderOrPlayPopup.blocksRaycasts = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        TimerUpdate();

        //if(Input.GetKeyDown(KeyCode.W))
        //{
        //    UpdateWinCount();
        //}
    }


    void InitializeMusicState()
    {
        if (!PlaylistControllerManager.instance.isMusicMuted)
            UnmuteMusic();
        else if (PlaylistControllerManager.instance.isMusicMuted)
            MuteMusic();

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
        // A decommenter si on a beosin que le joueur ne puisse pas restart le jeu, quand celui-ci est en pause
        //If we don't want to restart when the game is paused
        /*
        if (!GameManager.s_Singleton.gameIsPaused)
            StartCoroutine(RestartCoroutine());
        */

        if (!isMusicMuted)
            PlaylistControllerManager.instance.isMusicMuted = false;
        else if (isMusicMuted)
            PlaylistControllerManager.instance.isMusicMuted = true;

        StartCoroutine(RestartCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        Debug.Log("Reset the game");
        PlayButtonSoundOnClick();

        yield return new WaitForSeconds(0.5f);

        GameManager.s_Singleton.ResetTheGame();
    }

    public void OnClickOrderButton()
    {
        Debug.Log("Order");
        FadeOutOrderOrPlayPopup();
        SceneManager.LoadScene("02_SceneCommande");
    }

    public void OnClickPlayButton()
    {
        //Debug.Log("Play");
        FadeOutOrderOrPlayPopup();
        GameManager.s_Singleton.gameIsPaused = false;
        GameManager.s_Singleton.RandomizePalletPosAtStart();
    }

    public void OnClickGetAPizzaButton()
    {
        Debug.Log("Got a Pizza!!!!!!!!!!!!!!!!!!!");
        FadeInFreePizzaCodePopup();
    }

    public void OnMuteMusicButtonClick()
    {
        if (!GameManager.s_Singleton.gameIsFinished)
        {
            PlayButtonSoundOnClick();

            if (!isMusicMuted)
            {
                PlaylistControllerManager.instance.isMusicMuted = true;
                MuteMusic();
            }
            else if (isMusicMuted)
            {
                PlaylistControllerManager.instance.isMusicMuted = false;
                UnmuteMusic();
            }
        }
    }

    #region Music States
    void MuteMusic()
    {
        muteSoundImage.sprite = musicOff;
        MasterAudio.MuteEverything();
        isMusicMuted = true;
    }

    void UnmuteMusic()
    {
        muteSoundImage.sprite = musicOn;
        MasterAudio.UnmuteEverything();
        isMusicMuted = false;
    }
    #endregion

    public void OnPauseButtonClick()
    {
        if (!GameManager.s_Singleton.gameIsFinished)
        {
            PlayButtonSoundOnClick();

            if (!GameManager.s_Singleton.gameIsPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void OnResumeButtonClick()
    {
        if (!GameManager.s_Singleton.gameIsFinished)
        {
            PlayButtonSoundOnClick();
            ResumeGame();
        }
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
        StartCoroutine(FadeCanvasGroup(menuPausePopup, menuPausePopup.alpha, 1));
        menuPausePopup.blocksRaycasts = true;

        GameManager.s_Singleton.gameIsPaused = true;
    }

    void ResumeGame()
    {
        StartCoroutine(FadeCanvasGroup(menuPausePopup, menuPausePopup.alpha, 0));
        menuPausePopup.blocksRaycasts = false;

        GameManager.s_Singleton.gameIsPaused = false;
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
        StartCoroutine(FadeCanvasGroup(endOfGameResultsPopup, endOfGameResultsPopup.alpha, 1, 1f));
        clickCountTextAtTheEnd.text = "En  " + GameManager.s_Singleton.clickCount + " clics.";
        timerTextAtTheEnd.text = "Temps" + " : " + timerText.text + ".";
    }

    public void FadeOutEndOfGameResults()
    {
        StartCoroutine(FadeCanvasGroup(endOfGameResultsPopup, endOfGameResultsPopup.alpha, 0, 1f));
    }

    public void FadeInOrderOrPlayPopup()
    {
        StartCoroutine(FadeCanvasGroup(orderOrPlayPopup, orderOrPlayPopup.alpha, 1, 1.5f));
        orderOrPlayPopup.blocksRaycasts = true;
        GameManager.s_Singleton.Save();
    }

    public void FadeOutOrderOrPlayPopup()
    {
        StartCoroutine(FadeCanvasGroup(orderOrPlayPopup, orderOrPlayPopup.alpha, 0));
        orderOrPlayPopup.blocksRaycasts = false;
    }

    public void FadeInFreePizzaCodePopup()
    {
        StartCoroutine(FadeCanvasGroup(freePizzaCodePopup, freePizzaCodePopup.alpha, 1, 1.5f));
        freePizzaCodePopup.blocksRaycasts = true;
    }

    public void FadeOutFreePizzaCodePopup()
    {
        StartCoroutine(FadeCanvasGroup(freePizzaCodePopup, freePizzaCodePopup.alpha, 0));
        freePizzaCodePopup.blocksRaycasts = false;
    }

    public void FadeInDeliveryPizzaPopup()
    {
        StartCoroutine(FadeCanvasGroup(deliveryPizzaPopup, deliveryPizzaPopup.alpha, 1, 1.5f));
        deliveryPizzaPopup.blocksRaycasts = true;
    }

    public void FadeOutDeliveryPizzaPopup()
    {
        StartCoroutine(FadeCanvasGroup(deliveryPizzaPopup, deliveryPizzaPopup.alpha, 0));
        deliveryPizzaPopup.blocksRaycasts = false;
    }
    #endregion
}
