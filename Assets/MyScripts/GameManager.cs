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
    public static bool hasOrdered = false;

    public int loadCount;
    public TextMeshProUGUI winCountText;

    public Animator deliveryManAnimator;

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
            winCountText.text = PlayerPrefs.GetString("WinCountText");
        }

    }

    public void Save()
    {
        PlayerPrefs.SetInt("WinCount", winCount);
        PlayerPrefs.SetString("WinCountText", winCountText.text);
        PlayerPrefs.SetInt("LoadCount", loadCount);
    }
    #endregion

    void Start()
    {
        UpdateLoadCountValue();

        SetWinCountTextValue();

        currentGameTimer = gameTimerAtStart;

        if(loadCount > 1 || hasOrdered)
            Invoke("RandomizePalletPosAtStart", 0.25f);

        EnableGetAPizzaButton();

        if (hasOrdered)
            deliveryManAnimator.enabled = true;
    }

    void Update()
    {
        if (gameHasStarted && !gameIsFinished && !gameIsPaused)
        {
            currentGameTimer += Time.deltaTime;
        }
    }

    public void UpdateWinCount()
    {
        if (winCount != numberOfWinsToGetAPizza)
        {
            winCount++;
            SetWinCountTextValue();
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
            SetWinCountTextValue();
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
            EnableGetAPizzaButton();
            SetWinCountTextValue();

            MasterAudio.PausePlaylist();
            MasterAudio.PlaySoundAndForget("VictorySound");

            Save();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("HasAlreadyPassedOrder");
        PlayerPrefs.DeleteKey("LoadCount");
        Debug.Log("Quit");
    }

    void SetWinCountTextValue()
    {
        if (winCount == numberOfWinsToGetAPizza)
        {
            winCountText.fontSize = 66;
            winCountText.text = "Pizza Gratuite !";
            
            Debug.Log("Put FontSize to 66");
        }
        
        if (winCount != numberOfWinsToGetAPizza)
        {
            winCountText.fontSize = 95;
            winCountText.text = winCount.ToString("0") + " / " + numberOfWinsToGetAPizza.ToString();
            
            Debug.Log("Put FontSize to 95");
        }
    }

    #region Randomize Pallet Position At Start
    public void RandomizePalletPosAtStart()
    {
        gameIsPaused = false;
        //Debug.Log("Random");

        for (int i = 0; i < palletControllers.Count - 1; i++)
        {
            palletControllers[i].clickOnPallet = Random.Range(0, palletControllers.Count + 1);
            if (palletControllers[i].clickOnPallet % 2 == 1)
            {
                palletFlippedAtStart++;
                //Debug.Log("Can Flip " + palletControllers[i]);
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
        SetWinCountTextValue();
        ReEnableColliderOnRestart();
        MasterAudio.UnpausePlaylist();
        SceneManager.LoadScene("01_SceneJeu");
    }
    #endregion

}
