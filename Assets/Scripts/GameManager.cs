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

    [Header("FinishExpPanel")]
    public GameObject panelFinished;

    [Header("Support")]
    private bool experienceFinished = false;
    private bool isCardUnlockedShown = false;
    private bool filterUnlocked = false;
    private bool reachedEnoughCard = false;
    private int numberSpawnedImages = 0;
    public int maxNumberReach; //20 da inspector



    public void Awake()
    {
        instance = this;
        Init();
    }
    public void Init()
    {
        filterUnlocked = false;
        reachedEnoughCard = false;
        experienceFinished = false;
        isCardUnlockedShown = false;
        numberSpawnedImages = 0;
        panelFinished.SetActive(false);

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
        Clusters c = ChooseClusterToSpawn();
        if (imgClusters[(int)c].Count != 0)
        {
            Immagine i = ChooseNewImage(c);
            spawnedImage = i;
            currentImage.sprite = i.imageConfig.image;
            numberSpawnedImages++;

        }
        else
        {
            Debug.Log("Non ci sono piu card da spawnare in " + c);
        }
    }

    public bool CheckFinishExperience()
    {
        if (numberSpawnedImages > maxNumberReach)
        {
            if (filterUnlocked)
            {
                return true;
            }
        }
        return false;
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
        if (CheckFinishExperience())
        {
            panelFinished.SetActive(true);
        }
    }

    public void SwipeRight()
    {
        Accept();
        spawnedImage.isSwappedRight = true;

        if ((CountUnlockCardComplete(spawnedImage.cluster, spawnedImage.IDCompleteImage) > 2))
        {
            UnlockCompletedImage();
        }

        RemoveImage(spawnedImage.cluster, spawnedImage);
        if (CheckFinishExperience())
        {
            experienceFinished = true;
            if (!isCardUnlockedShown)
            {
                panelFinished.SetActive(true);
            }
        }
    }

    public void UnlockCompletedImage()
    {   
        ImmagineCompleta icomp = GetCompletedImage(spawnedImage);
        panelImageUnlocked.gameObject.SetActive(true);
        unlockedImage.sprite = icomp.sprite;
        filtroGO.gameObject.SetActive(icomp.hasFilter);
        if (icomp.hasFilter)
        {
            UnlockFilter(icomp);
        }
        StartCoroutine(CountdownImgUnlocked());
    }

    IEnumerator CountdownImgUnlocked()
    {
        isCardUnlockedShown = true;
        yield return new WaitForSeconds(3f);
        panelImageUnlocked.gameObject.SetActive(false);
        filtroGO.gameObject.SetActive(false);
        isCardUnlockedShown = false;
        if (experienceFinished)
        {
            panelFinished.SetActive(true);
        }
    }

    public void UnlockFilter(ImmagineCompleta ic)
    {
        filterUnlocked = true;
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
