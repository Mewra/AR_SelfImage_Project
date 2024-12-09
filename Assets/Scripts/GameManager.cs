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
    public List<ImmagineCompleta> allCompletedImages;
    public Immagine spawnedImage;

    [Header("Swiper")]
    public Image currentImage;
    public Image nextImage;

    [Header("Unlocked")]
    public Image unlockedImage;
    public GameObject panelImageUnlocked;
    public GameObject filtroGO;


    public void Awake()
    {
        instance = this;
        Init();
    }
    public void Init()
    {
        allImages = new List<Immagine>();
        imgClusters = new List<List<Immagine>>();
        allCompletedImages = new List<ImmagineCompleta>();
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

                ImmagineCompleta imgCompl = new ImmagineCompleta();
                imgCompl.IDCompleteImage = cic.id;
                imgCompl.sprite = cic.completedImage;
                imgCompl.hasFilter = cic.hasFilter;
                allCompletedImages.Add(imgCompl);

            }

        }

        SpawnNewImage();
    }

    public void RemoveImage(Clusters c, Immagine i)
    {
        i.isAlreadySpawned = true;
        imgClusters[(int)c].Remove(i);
        allImages[allImages.IndexOf(i)].isAlreadySpawned = true;
    }

    public Immagine ChooseNewImage(Clusters c)
    {
        float size = imgClusters[(int)c].Count;
        int valoreCasuale = (int)UnityEngine.Random.Range(0, size-1);
        Immagine img = new Immagine();
        img = imgClusters[(int)c][valoreCasuale];
        
        return img;
    }

    public Clusters ChooseClusterToSpawn()
    {
        return Clusters.A;
    }

    public void SpawnNewImage()
    {
        Immagine i = ChooseNewImage(ChooseClusterToSpawn());
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

    public void SwipeLeft()
    {
        Reject();

        RemoveImage(spawnedImage.cluster, spawnedImage);
    }

    public void SwipeRight()
    {
        Accept();
        spawnedImage.isSwappedRight = true;

        if (!(CountUnlockCardComplete(spawnedImage.cluster, spawnedImage.IDCompleteImage) > 2))
        {
            
        }else
        {
            UnlockCompletedImage();
        }

        RemoveImage(spawnedImage.cluster, spawnedImage);
    }

    public void UnlockCompletedImage()
    {   
        ImmagineCompleta icomp = GetCompletedImage(spawnedImage);
        panelImageUnlocked.gameObject.SetActive(true);
        filtroGO.gameObject.SetActive(icomp.hasFilter);
        unlockedImage.sprite = icomp.sprite;
        StartCoroutine(CountdownImgUnlocked());
    }

    IEnumerator CountdownImgUnlocked()
    {
        yield return new WaitForSeconds(3f);
        panelImageUnlocked.gameObject.SetActive(false);
        filtroGO.gameObject.SetActive(false);
    }

    public int CountUnlockCardComplete(Clusters c, string idCardComplete)
    {
        int num = 0;

        foreach(Immagine i in allImages)
        {
            if (i.IDCompleteImage == idCardComplete)
            {
                if (i.isSwappedRight)
                {
                    num++;
                    Debug.Log("n: " + num);
                    if (num > 2)
                    {
                        return num;
                    }
                }
            }
        }

        return num;
    }

    public ImmagineCompleta GetCompletedImage(Immagine i)
    {
        foreach(ImmagineCompleta iCompleted in allCompletedImages)
        {
            if(iCompleted.IDCompleteImage == i.IDCompleteImage)
            {
                return iCompleted;
            }
        }

        return null;

    }

}

[Serializable]
public class Immagine
{
    public Clusters cluster;
    public string IDCompleteImage;
    public ImageConfig imageConfig;
    public bool isAlreadySpawned;
    public bool isSwappedRight;
}

public class ImmagineCompleta
{
    public string IDCompleteImage;
    public Sprite sprite;
    public bool hasFilter;
}
