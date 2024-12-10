using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ARManager : MonoBehaviour
{
    public string screenshotName = "screenshot.jpg";

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
}
