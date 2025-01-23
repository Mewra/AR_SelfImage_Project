using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.XR.ARFoundation;
using System;
using UnityEngine.UI;

public class ARManager : MonoBehaviour
{
    public static ARManager instance;

    public string screenshotName = "screenshot.jpg";
    public int countdownTime = 3;

    [Header("UI")]
    public GameObject fotoScene;
    public GameObject UIfotoScene;
    public TMP_Text countdownText;
    public GameObject scattaFotoBtn;
    public GameObject ricominciaBtn;

    [Header("Screenshot")]
    public string path;
    public byte[] screenshotBytes;
    public RawImage uiImage;


    private void Awake()
    {
        instance = this;
        countdownText.gameObject.SetActive(false);
    }

    #region Screenshot
    IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();
        // Creazione di una Texture2D
        int width = Screen.width;
        int height = Screen.height;
        Debug.Log("Width " + width + " Height " + height);
        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Acquisizione dello schermo
        screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshotTexture.Apply();

        screenshotBytes = screenshotTexture.EncodeToJPG();

        // Salvataggio su disco
        path = Path.Combine(Application.persistentDataPath, screenshotName);
        File.WriteAllBytes(path, screenshotBytes);

        Debug.Log($"Screenshot salvato in: {path}");
        uiImage.texture = screenshotTexture;

        Debug.Log("PLAY OFFLINE" + GameManager.instance.playOffline);
        if (!GameManager.instance.playOffline)
        {
            LinkAPIManager.instance.SendReportImage();
        }
        // Pulizia della memoria
        //Destroy(screenshotTexture);
    }
    // Start is called before the first frame update
    public void TakeScreenshot()
    {
        StartCoroutine(TogliUI());
        StartCoroutine(CaptureScreenshot());
    }


    public void OnClickScatta()
    {
        countdownText.gameObject.SetActive(true);
        StartCoroutine(StartCountdown());
    }



    IEnumerator StartCountdown()
    {
        int currentTime = countdownTime;

        // Loop del countdown
        while (currentTime > 0)
        {
            // Aggiorna il testo
            countdownText.text = currentTime.ToString();

            // Aspetta un secondo
            yield return new WaitForSeconds(1);

            // Riduci il tempo rimanente
            currentTime--;
        }
        
        TakeScreenshot();
    }

    IEnumerator TogliUI()
    {
        fotoScene.SetActive(false);
        //UIfotoScene.SetActive(false);
        countdownText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        uiImage.gameObject.SetActive(true);
        fotoScene.SetActive(true);
        ricominciaBtn.SetActive(true);
        scattaFotoBtn.SetActive(false);

    }
    #endregion

    public void ResetFotoScene()
    {
        fotoScene.SetActive(false);
        UIfotoScene.SetActive(true);
        uiImage.gameObject.SetActive(false);
        ricominciaBtn.SetActive(false);
        scattaFotoBtn.SetActive(true);

    }
}
