using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static ImagesConfig;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum Clusters { A,B,C,D,E,F};

    public List<Immagine> allImages;
    public List<List<Immagine>> imgClusters;
    public List<ImagesConfig> resourcesImage;
    public Immagine spawnedImage;

    [Header("Swiper")]
    public Image currentImage;
    public Image nextImage;

    public void Awake()
    {
        instance = this;
        Init();
    }
    public void Init()
    {
        allImages = new List<Immagine>();
        imgClusters = new List<List<Immagine>>();
        for(int i = 0; i<3;i++)
        {
            imgClusters.Add(new List<Immagine>());
        }

        foreach(ImagesConfig imagec in resourcesImage)
        {
            
            foreach (CompleteImageConfig cic in imagec.listImageConfig)
            {
                
                foreach (ImageConfig config in cic.images)
                {
                    Immagine img = new Immagine();

                    img.cluster = cic.cluster;
                    img.IDCompleteImage = cic.id;
                    img.imageConfig = config;
                    img.isAlreadySpawned = false;


                    allImages.Add(img);
                    imgClusters[(int)img.cluster].Add(img);
                }

            }

        }

        SpawnNewImage();
    }

    public void RemoveImage(Clusters c, Immagine i)
    {
        i.isAlreadySpawned = true;
        imgClusters[(int)c].Remove(i);
    }

    public Immagine ChooseNewImage(Clusters c)
    {
        float size = imgClusters[(int)c].Count;
        int valoreCasuale = (int)UnityEngine.Random.Range(0, size-1);
        Immagine img = new Immagine();
        img = imgClusters[(int)c][valoreCasuale];
        RemoveImage(c, img);
        return img;
    }

    public void SpawnNewImage()
    {
        Immagine i = ChooseNewImage(Clusters.A);
        spawnedImage = i;
        currentImage.sprite = i.imageConfig.image;
    }

    public void Reject()
    {
        foreach(ValuesImage VI in spawnedImage.imageConfig.RejectedValues)
        {
            SlidersManager.instance.UpdateSliders(VI);
        }
    }

    public void Accept()
    {
        foreach (ValuesImage VI in spawnedImage.imageConfig.AcceptedValues)
        {
            SlidersManager.instance.UpdateSliders(VI);
        }
    }

}

[Serializable]
public class Immagine
{
    public Clusters cluster;
    public string IDCompleteImage;
    public ImageConfig imageConfig;
    public bool isAlreadySpawned;
}
