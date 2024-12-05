using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

[CreateAssetMenu(fileName = "AllImagesValues", menuName = "My Scriptable Objects/ImagesConfig")]
[Serializable]
public class ImagesConfig : ScriptableObject
{

    public List<CompleteImageConfig> listImageConfig;

    [Serializable]
    public class CompleteImageConfig
    {
        public string id;
        public Clusters cluster;
        public List<ImageConfig> images;

    }

    [Serializable]
    public class ImageConfig
    {
        
        public string id;
        public Sprite image;
        public List<ValuesImage> AcceptedValues;
        public List<ValuesImage> RejectedValues;

    }

    [Serializable]
    public class ValuesImage
    {
        public Clusters cluster;
        public float value;
    }

    
}
