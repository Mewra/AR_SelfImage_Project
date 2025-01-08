using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class LinkAPIManager : MonoBehaviour
{
    private void Start()
    {
        Serialize();
    }
    public void Serialize()
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
            unlocked_filters = new string[] { "filtro1", "filtro2" },
            unlocked_images = new string[] { "images1", "images2" },
            image_base64 = "L'immagine"
        };


        string json = JsonUtility.ToJson(session);

        Debug.Log(json);
    }

}


[Serializable]
public class SessionReport{

    public string player_id;
    public List<Interaction> interactions;
    public List<ClusterScores> scores;
    public string[] unlocked_filters;
    public string[] unlocked_images;
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
