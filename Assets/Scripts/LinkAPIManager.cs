using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static GameManager;

public class LinkAPIManager : MonoBehaviour
{
    public static LinkAPIManager instance;
    public SessionReport sessionReport;

    private void Start()
    {
        instance = this;
        //SerializeProva();
    }

    /*
    public void SerializeProva()
    {
        List<Interaction> interactions = new List<Interaction>();
        Interaction interaction1 = new Interaction
        {
            fragment_id = "fragment1",
            image_id = "immaginecompletaID1",
            cluster_id = Clusters.A.ToString(),
            liked = true

        };

        Interaction interaction2 = new Interaction
        {
            fragment_id = "fragment2",
            image_id = "immaginecompletaID2",
            cluster_id = Clusters.B.ToString(),
            liked = false

        };

        interactions.Add(interaction1);
        interactions.Add(interaction2);

        List<ClusterScores> scores = new List<ClusterScores>();
        ClusterScores score1 = new ClusterScores
        {
            cluster_id = Clusters.A.ToString(),
            score = 19.0f

        };
        ClusterScores score2 = new ClusterScores
        {
            cluster_id = Clusters.B.ToString(),
            score = 34.0f

        };

        scores.Add(score1);
        scores.Add(score2);


        SessionReport session = new SessionReport
        {
            player_id = "1",
            interactions = interactions,
            scores = scores,
            unlocked_filters = new List<string> { "filtro1", "filtro2" },
            unlocked_images = new List<string> { "images1", "images2" }
        };


        string json = JsonUtility.ToJson(session);

        Debug.Log(json);
    }*/


    public void AddInteraction(Immagine i, bool swipped)
    {
        Interaction interaction = new Interaction
        {
            fragment_id = i.imageConfig.id,
            image_id = i.IDCompleteImage,
            cluster_id = i.cluster.ToString(),
            liked = swipped

        };

        sessionReport.interactions.Add(interaction);
    }

    public void AddClusterValues()
    {
        sessionReport.scores = new List<ClusterScores>();
        foreach(SliderClusterModel scm in SlidersManager.instance.sliderClusters)
        {
            string val = TruncateToTwoDecimals(scm.slider.normalizedValue).ToString("F2");
            Debug.Log(val);
            ClusterScores score = new ClusterScores
            {
                cluster_id = scm.cluster.ToString(),
                score = float.Parse(val)

            };

            sessionReport.scores.Add(score);
        }

    }

    public float TruncateToTwoDecimals(float value)
    {
        return Mathf.Floor(value * 100) / 100;
    }

    public void AddUnlockedImage(string id)
    {
        sessionReport.unlocked_images.Add(id);
    }

    public void AddUnlockedFilters(string id)
    {
        sessionReport.unlocked_filters.Add(id);
    }

    public void ResetSessionReport()
    {
        //sessionReport = new SessionReport();
        sessionReport.interactions = new List<Interaction>();
        sessionReport.scores = new List<ClusterScores>();
        sessionReport.unlocked_filters = new List<string>();
        sessionReport.unlocked_images = new List<string>();
    }

    #region API Requests
    public void SendRequestJoinRoom(string nickname, string roomcode)
    {
        string apiUrl = "https://self-image-api-production.up.railway.app/api/room/join";
        var data = new JoinRoomData
        {
            room_code = roomcode,
            nickname = nickname
        };

        // Avvia la coroutine per inviare la richiesta
        StartCoroutine(SendPostJoinRoom(apiUrl, data));
    }

    

    IEnumerator SendPostJoinRoom(string url, JoinRoomData requestData)
    {
        // Converte l'oggetto in JSON
        string jsonData = JsonUtility.ToJson(requestData);

        // Crea il UnityWebRequest con il metodo POST
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Imposta gli header (ad esempio, per inviare un body JSON)
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        // Manda la richiesta e aspetta la risposta
        yield return request.SendWebRequest();


        // Gestisci la risposta
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            sessionReport.player_id = JsonUtility.FromJson<PlayerIDResponse>(responseText).player_id;
            Debug.Log("PlayerID: " + responseText);
            GameManager.instance.StartGame();

        }
        else
        {
            StartCoroutine(GameManager.instance.ShowErrorMessage("Errore nella richiesta: " + request.error));
        }
    }

    public void SendSessionReport()
    {
        string apiUrl = "https://self-image-api-production.up.railway.app/api/room/session-report";

       
        // Avvia la coroutine per inviare la richiesta
        StartCoroutine(SendPostSessionReport(apiUrl, sessionReport));
    }



    IEnumerator SendPostSessionReport(string url, SessionReport requestData)
    {
        // Converte l'oggetto in JSON
        string jsonData = JsonUtility.ToJson(requestData);

        Debug.Log(jsonData);

        // Crea il UnityWebRequest con il metodo POST
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Imposta gli header (ad esempio, per inviare un body JSON)
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        // Manda la richiesta e aspetta la risposta
        yield return request.SendWebRequest();

        // Gestisci la risposta
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);
        }
        else
        {
            Debug.LogError("Errore nella richiesta: " + request.error);
        }
    }

    public void SendImage(string path, byte[] img)
    {
        string apiUrl = "https://self-image-api-production.up.railway.app/api/room/selfie";

        // Avvia la coroutine per inviare la richiesta
        StartCoroutine(SendPostImage(apiUrl, path, img));
    }



    IEnumerator SendPostImage(string url, string path, byte[] img)
    {
        
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

        // Create a form and add fields
        WWWForm form = new WWWForm();
        form.AddField("player_id", LinkAPIManager.instance.sessionReport.player_id);
        form.AddBinaryData("file", img, Path.GetFileName(path), "image/jpeg");

        // Attach the form to the request
        request.uploadHandler = new UploadHandlerRaw(form.data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", form.headers["Content-Type"]);

        // Send the request
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"File uploaded successfully: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"File upload failed: {request.error}");
        }
    }

    public void SendReportImage()
    {

        SendImage(ARManager.instance.path, ARManager.instance.screenshotBytes);
        SendSessionReport();
    }

    #endregion

}

[System.Serializable]
public class JoinRoomData
{
    public string room_code;
    public string nickname;
}

[System.Serializable]
public class PlayerIDResponse
{
    public string player_id;
}

[Serializable]
public class SessionReport{

    public string player_id;
    public List<Interaction> interactions;
    public List<ClusterScores> scores;
    public List<string> unlocked_filters;
    public List<string> unlocked_images;
}

[Serializable]
public class Interaction
{
    public string fragment_id;
    public string image_id;
    public string cluster_id;
    public bool liked;
}

[Serializable]
public class ClusterScores
{
    public string cluster_id;
    public float score;
}
