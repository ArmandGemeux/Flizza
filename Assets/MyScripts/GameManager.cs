using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Add all the pallets present in the game here")]
    public List<PalletController> palletControllers;

    //GameStates
    [Header("Debug Game States")]
    public bool gameHasStarted = false;
    public bool gameIsPaused = false;
    public bool gameIsFinished = false;

    public int clickCount = 0;
    public int numberOfGreenPallet;
    [SerializeField] private int winCount;
    [SerializeField] private int numberOfWinsToGetAPizza;

    [HideInInspector]
    public float currentGameTimer;
    private float gameTimerAtStart = 0.0f;
    public int palletFlippedAtStart = 0;

    [SerializeField] private float delayUntilRestart = 0.2f;

    #region Singleton
    public static GameManager s_Singleton;

    private void Awake()
    {
        if (s_Singleton != null)
        {
            Destroy(this);
        }
        else
        {
            s_Singleton = this;

            if (PlayerPrefs.HasKey("WinCount"))
            {
                winCount = PlayerPrefs.GetInt("WinCount");
            }
            else
            {
                Save();
            }
        }

        Debug.Log(winCount);

    }

    public void Save()
    {
        PlayerPrefs.SetInt("WinCount", winCount);
    }
    #endregion

    void Start()
    {
        InitializeWinCount();
        currentGameTimer = gameTimerAtStart;
        Invoke("RandomizePalletPosAtStart", 0.25f);
        EnableGetAPizzaButton();
    }

    void Update()
    {
        if (gameHasStarted && !gameIsFinished && !gameIsPaused)
        {
            currentGameTimer += Time.deltaTime;
        }
    }

    void InitializeWinCount()
    {
        UIManager.s_Singleton.winCounterText.text = winCount.ToString() + " / " + numberOfWinsToGetAPizza.ToString();
    }

    public void UpdateWinCount()
    {
        if (winCount != numberOfWinsToGetAPizza)
        {
            winCount++;
            UIManager.s_Singleton.winCounterText.text = winCount.ToString("0") + " / " + numberOfWinsToGetAPizza.ToString();
            Save();
        }
    }

    void EnableGetAPizzaButton()
    {
        if (winCount == numberOfWinsToGetAPizza)
        {
            Debug.Log("nombre de victoires = nombre total pour avoir un bon kdo ! ");
            UIManager.s_Singleton.getAPizzaButton.interactable = true;
        }
    }

    public void CheckIfThePlayerHasWon()
    {
        numberOfGreenPallet = 0;

        //Check si tous les pallets sont verts
        foreach (PalletController pc in palletControllers)
        {
            if (pc.isGreen)
            {
                numberOfGreenPallet++;
            }
        }

        //S'ils sont tous verts alors le joueur a gagné
        if (numberOfGreenPallet == palletControllers.Count)
        {
            gameIsFinished = true;

            UpdateWinCount();
            UIManager.s_Singleton.FadeInEndOfGameResults();

            MasterAudio.PausePlaylist();
            MasterAudio.PlaySoundAndForget("VictorySound");
            //MasterAudio.MutePlaylist();
            //MasterAudio.PlaySoundAndForget("VictorySound");
        }
    }

    #region Randomize Pallet Position At Start
    void RandomizePalletPosAtStart()
    {
        Debug.Log("Random");

        for (int i = 0; i < palletControllers.Count - 1; i++)
        {
            palletControllers[i].clickOnPallet = Random.Range(0, palletControllers.Count + 1);
            if (palletControllers[i].clickOnPallet % 2 == 1)
            {
                palletFlippedAtStart++;
                Debug.Log("Can Flip " + palletControllers[i]);
                palletControllers[i].FlipPallets();
            }
        }

        if (palletFlippedAtStart == 0)
        {
            Debug.Log("Noppeuh");
            palletControllers[4].FlipPallets();
        }

        gameHasStarted = true;
        clickCount = 0;
    }
    #endregion

    #region Reseting the Game
    void ReEnableColliderOnRestart()
    {
        for (int i = 0; i < palletControllers.Count; i++)
        {
            palletControllers[i].myCollider.enabled = true;
        }
    }

    public void ResetTheGame()
    {
        ReEnableColliderOnRestart();
        MasterAudio.UnpausePlaylist();
        SceneManager.LoadScene("01_SceneJeu");
    }
    #endregion

}
