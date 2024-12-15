using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    public static ARManager instance;

    public string screenshotName = "screenshot.jpg";
    public int countdownTime = 3;

    [Header("UI")]
    public GameObject fotoScene;
    public GameObject UIfotoScene;
    public TMP_Text countdownText;


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

        // Conversione in byte array (formato JPG)
        byte[] screenshotBytes = screenshotTexture.EncodeToJPG();

        // Salvataggio su disco
        string path = Path.Combine(Application.persistentDataPath, screenshotName);
        File.WriteAllBytes(path, screenshotBytes);

        Debug.Log($"Screenshot salvato in: {path}");

        // Pulizia della memoria
        Destroy(screenshotTexture);
    }
    // Start is called before the first frame update
    public void TakeScreenshot()
    {
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
        UIfotoScene.SetActive(false);
        countdownText.gameObject.SetActive(false);
        TakeScreenshot();
    }
    #endregion

    #region Occlusion

    /*
    [Header("Occlusion")]
    public Camera arCamera;
    public GameObject backgroundObject;
    public AROcclusionManager occlusionManager;
    public float distanceBehind = 8.0f;

    void Update()
    {
        /*
        // Posiziona il background dietro
        Vector3 positionBehind = arCamera.transform.position + arCamera.transform.forward * distanceBehind; 
        backgroundObject.transform.position = positionBehind;
        backgroundObject.transform.rotation = Quaternion.LookRotation(arCamera.transform.forward);
        backgroundObject.GetComponent<MeshRenderer>().material.color = Color.green;
        */

        /*
        // Controlla l'occlusione (solo se supportato dal dispositivo)
        if (occlusionManager.environmentDepthTexture != null)
        {
            backgroundObject.GetComponent<MeshRenderer>().material.color = Color.blue;
            Texture2D depthTexture = occlusionManager.environmentDepthTexture;
            Vector3 screenPoint = arCamera.WorldToScreenPoint(backgroundObject.transform.position);
            float depthAtPoint = GetDepthAtScreenPoint(depthTexture, screenPoint);

            // Nascondi il background se è davanti al corpo
            if (depthAtPoint > distanceBehind)
            {
                backgroundObject.SetActive(false);
                backgroundObject.GetComponent<MeshRenderer>().material.color = Color.black;
            }
            else
            {
                backgroundObject.SetActive(true);
                backgroundObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        
    }

    float GetDepthAtScreenPoint(Texture2D depthTexture, Vector3 screenPoint)
    {
        int x = Mathf.Clamp((int)(screenPoint.x / Screen.width * depthTexture.width), 0, depthTexture.width - 1);
        int y = Mathf.Clamp((int)(screenPoint.y / Screen.height * depthTexture.height), 0, depthTexture.height - 1);
        Color depthColor = depthTexture.GetPixel(x, y);
        return depthColor.r; // Profondità normalizzata
    }

*/
    #endregion
}
