using System;
using System.Collections;
using System.Collections.Generic;
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
        SerializeProva();
    }

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
            value = 19.0f

        };
        ClusterScores score2 = new ClusterScores
        {
            cluster_id = Clusters.B.ToString(),
            value = 34.0f

        };

        scores.Add(score1);
        scores.Add(score2);


        SessionReport session = new SessionReport
        {
            player_id = "1",
            interactions = interactions,
            scores = scores,
            unlocked_filters = new List<string> { "filtro1", "filtro2" },
            unlocked_images = new List<string> { "images1", "images2" },
            image_base64 = "L'immagine"
        };


        string json = JsonUtility.ToJson(session);

        Debug.Log(json);
    }


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

            ClusterScores score = new ClusterScores
            {
                cluster_id = scm.cluster.ToString(),
                value = scm.slider.normalizedValue

            };

            sessionReport.scores.Add(score);
        }

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
        sessionReport = new SessionReport();
    }

    #region API Requests
    public void SendRequestJoinRoom(string nickname, string roomcode)
    {
        string apiUrl = "https://example.com/api/room/join";
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

        }
        else
        {
            Debug.LogError("Errore nella richiesta: " + request.error);
        }
    }

    public void SendSessionReport(string nickname, string roomcode)
    {
        string apiUrl = "https://example.com/api/room/session-report";

       
        // Avvia la coroutine per inviare la richiesta
        StartCoroutine(SendPostSessionReport(apiUrl, sessionReport));
    }



    IEnumerator SendPostSessionReport(string url, SessionReport requestData)
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
            Debug.Log("Response: " + responseText);

        }
        else
        {
            Debug.LogError("Errore nella richiesta: " + request.error);
        }
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
    public string image_base64;
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
    public float value;
}
