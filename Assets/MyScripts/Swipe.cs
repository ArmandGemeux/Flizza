using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Swipe : MonoBehaviour
{
    private float swipeDistanceThreshold = 50;

    private Vector2 startPosition;
    private Vector2 endPosition;
    
    
    public Animator myAnim;

    public void Start()
    {

    }

    public void Update()
    {

        if (Input.touchCount == 1)
        {
            var touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Stockage du point de départ
                    startPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    // Stockage du point de fin
                    endPosition = touch.position;
                    AnalyzeGesture(startPosition, endPosition);
                    break;
            }
        }
    }

    private void AnalyzeGesture(Vector2 start, Vector2 end)
    {
        // Distance
        if (Vector2.Distance(start, end) > swipeDistanceThreshold)
        {
            // Le mouvement est suffisamment ample
            myAnim.Play("Transition");
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("01_SceneJeu");
    }
}