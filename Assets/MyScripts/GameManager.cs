using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Add all the pallets present in the game here")]
    public List<PalletController> palletControllers;

    //GameStates
    [Header("Debug Game States")]
    public bool gameHasStarted = false;
    public bool gameIsPaused = true;
    public bool gameIsFinished = false;

    public int clickCount = 0;
    public int numberOfGreenPallet;
    [SerializeField] private int winCount;
    [SerializeField] private int numberOfWinsToGetAPizza;

    [HideInInspector]
    public float currentGameTimer;
    private float gameTimerAtStart = 0.0f;
    public int palletFlippedAtStart = 0;

    public int loadCount;
    public TextMeshProUGUI winCounterText;

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

            winCount = PlayerPrefs.GetInt("WinCount");
            loadCount = PlayerPrefs.GetInt("LoadCount");
            winCounterText.text = PlayerPrefs.GetString("WinCounterText");
        }

        Debug.Log(winCount);

    }

    public void Save()
    {
        PlayerPrefs.SetInt("WinCount", winCount);
        PlayerPrefs.SetInt("LoadCount", loadCount);
        PlayerPrefs.SetString("WinCounterText", winCounterText.text);
    }
    #endregion

    void Start()
    {
        UpdateLoadCountValue();

        Invoke("InitializeWinCount", 0.5f);

        currentGameTimer = gameTimerAtStart;

        if(loadCount > 1)
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

    //Invoked Method
    void InitializeWinCount()
    {
        if (winCount != numberOfWinsToGetAPizza && loadCount <= 1)
            winCounterText.text = winCount.ToString() + " / " + numberOfWinsToGetAPizza.ToString();
    }

    public void UpdateWinCount()
    {
        if (winCount != numberOfWinsToGetAPizza)
        {
            winCount++;
            winCounterText.text = winCount.ToString("0") + " / " + numberOfWinsToGetAPizza.ToString();
            Save();
        }
    }

    void UpdateLoadCountValue()
    {
        loadCount++;
        Save();
    }

    void EnableGetAPizzaButton()
    {
        if (winCount == numberOfWinsToGetAPizza)
        {
            UIManager.s_Singleton.getAPizzaButton.interactable = true;
            winCounterText.text = "Pizza offerte obtenue!";
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

            if (winCount == numberOfWinsToGetAPizza)
                winCounterText.text = "Pizza offerte obtenue!";

            MasterAudio.PausePlaylist();
            MasterAudio.PlaySoundAndForget("VictorySound");

            Save();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("LoadCount");
        Debug.Log("Quit");
    }

    #region Randomize Pallet Position At Start
    public void RandomizePalletPosAtStart()
    {
        gameIsPaused = false;
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
