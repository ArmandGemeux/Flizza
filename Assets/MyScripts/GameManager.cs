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
    public int palletFlippedAtStart = 0;

    [HideInInspector]
    public float currentGameTimer;
    private float gameTimerAtStart = 0.0f;

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
        }
    }
    #endregion

    void Start()
    {
        currentGameTimer = gameTimerAtStart;
        Invoke("RandomizePalletPosAtStart", 1.25f);
    }

    void Update()
    {
        if (gameHasStarted && !gameIsFinished && !gameIsPaused)
            currentGameTimer += Time.deltaTime;
    }

    public void CheckIfThePlayerHasWon()
    {
        numberOfGreenPallet = 0;

        foreach (PalletController pc in palletControllers)
        {
            if (pc.isGreen)
            {
                numberOfGreenPallet++;
            }
        }

        if (numberOfGreenPallet == palletControllers.Count)
        {
            gameIsFinished = true;

            UIManager.s_Singleton.FadeInEndOfGameResults();

            MasterAudio.PausePlaylist();
            MasterAudio.PlaySoundAndForget("VictorySound");
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
