using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager_Commande : MonoBehaviour
{
    [SerializeField] private CanvasGroup customersInfoWindow;

    [SerializeField] private TextMeshProUGUI orderRecapText;
    private string pizzaName;
    private float pizzaPrice;


    public void GetPizzaName(string _pizzaName)
    {
        pizzaName = _pizzaName;
    }

    public void GetPizzaName(float _pizzaPrice)
    {
        pizzaPrice = _pizzaPrice;
    }

    public void OnClickPizzaButton()
    {
        Debug.Log(gameObject.name);
        FadeInCustomersInfoWindow();
        UpdatePizzaNameInOrderRecap(pizzaName, pizzaPrice);
    }

    public void OnClickToBakeButton()
    {
        StartCoroutine(SetOderState());
    }

    IEnumerator SetOderState()
    {
        GameManager.hasOrdered = true;

        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene("01_SceneJeu");
    }


    void UpdatePizzaNameInOrderRecap(string myPizzaName, float myPizzaPrice)
    {
        orderRecapText.text = "   " + myPizzaName + " , " + " prix : " + myPizzaPrice + " € ";
    }

    #region Fade Windows or Popup
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

    public void FadeInCustomersInfoWindow()
    {
        StartCoroutine(FadeCanvasGroup(customersInfoWindow, customersInfoWindow.alpha, 1));
        customersInfoWindow.blocksRaycasts = true;
    }

    public void FadeOutCustomersInfoWindow()
    {
        StartCoroutine(FadeCanvasGroup(customersInfoWindow, customersInfoWindow.alpha, 0));
        customersInfoWindow.blocksRaycasts = false;
        Debug.Log("Click Exit button");
    }
    #endregion
}
