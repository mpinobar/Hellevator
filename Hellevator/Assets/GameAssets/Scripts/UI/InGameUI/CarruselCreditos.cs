using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarruselCreditos : MonoBehaviour
{
    [SerializeField] float timeForEachPanel = 3f;
    float panelTimer;
    [SerializeField] EndCredits endCredits;
    bool fadingIn = true;

    byte index = 0;
    [SerializeField] float alphaChangeSpeed = 2;
    float currentAlpha = 0;
    Color currentColor;
    Text [] childTexts;
    Image cenefa;
    Image bichillo;
    private void Start()
    {
        transform.GetChild(index).gameObject.SetActive(true);
        GetChildTextReferences();
    }
    // Update is called once per frame
    void Update()
    {
        if (fadingIn)
        {
            currentAlpha += Time.deltaTime * alphaChangeSpeed;
            currentColor = Color.white;
            currentColor.a = currentAlpha;
            for (int i = 0; i < childTexts.Length; i++)
            {
                childTexts[i].color = currentColor;
            }
            cenefa.color = currentColor;
            bichillo.color = currentColor;
            if (currentAlpha >= 1)
                panelTimer += Time.deltaTime;

            if (panelTimer >= timeForEachPanel)
            {
                currentAlpha = 1;
                fadingIn = false;
            }
        }
        else
        {
            currentAlpha -= Time.deltaTime * alphaChangeSpeed;
            currentColor = Color.white;
            currentColor.a = currentAlpha;

            if (currentAlpha <= 0)
            {
                if (index >= transform.childCount - 1)
                {
                    End();
                    return;
                }
                transform.GetChild(index).gameObject.SetActive(false);
                index++;
                transform.GetChild(index).gameObject.SetActive(true);
                GetChildTextReferences();
                currentAlpha = 0;
                panelTimer = 0;
                fadingIn = true;
            }
            for (int i = 0; i < childTexts.Length; i++)
            {
                childTexts[i].color = currentColor;
            }
            cenefa.color = currentColor;
            bichillo.color = currentColor;
        }
    }
    private void GetChildTextReferences()
    {
        childTexts = transform.GetChild(index).GetComponentsInChildren<Text>();
        Image[] imgs = transform.GetChild(index).GetComponentsInChildren<Image>();
        cenefa = imgs[0];
        bichillo = imgs[1];
    }
    private void End()
    {
        endCredits.HideEndCredits();
    }
}
