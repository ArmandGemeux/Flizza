using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletController : MonoBehaviour
{
    public PalletController[] sidePallets;

    public Collider myCollider;

    public bool isGreen = true;
    public int clickOnPallet = 0;
    bool canFlip = true;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableFlip());
    }
    
    void OnMouseUpAsButton()
    {
        if (canFlip && GameManager.s_Singleton.gameHasStarted)
        {
            GameManager.s_Singleton.clickCount++;
            clickOnPallet++;
            FlipPallets();
            StartCoroutine(EnableFlip());
            GameManager.s_Singleton.CheckIfThePlayerHasWon();
        }
        else
            return;
    }

    IEnumerator EnableFlip()
    {
        canFlip = false;

        yield return new WaitForSeconds(0.85f);

        canFlip = true;
    }
    
    public void FlipPallets()
    {
        if (!GameManager.s_Singleton.gameIsPaused && !GameManager.s_Singleton.gameIsFinished)
        {
            UIManager.s_Singleton.ClickCounterUpdate(GameManager.s_Singleton.clickCount);

            if(GameManager.s_Singleton.gameHasStarted)
            MasterAudio.PlaySoundAndForget("ClickOnPalletSound");

            foreach (PalletController p in sidePallets)
            {
                if (p.isGreen)
                {
                    p.GetComponent<Animator>().SetBool("_FlipToRed", true);
                    p.GetComponent<Animator>().SetBool("_FlipToGreen", false);
                    p.GetComponent<Collider>().enabled = false;
                    p.isGreen = false;
                }
                else if (!p.isGreen)
                {
                    p.GetComponent<Animator>().SetBool("_FlipToGreen", true);
                    p.GetComponent<Animator>().SetBool("_FlipToRed", false);
                    p.GetComponent<Collider>().enabled = false;
                    p.isGreen = true;
                }
            }
        }
    }

    public void EnableCollider()
    {
        foreach (PalletController pc in sidePallets)
        {
            GetComponent<Collider>().enabled = true;
        }
    }
    
}
