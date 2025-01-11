using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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

    [Header("JoinScene")]
    public TMP_InputField inputNickname;
    public TMP_InputField inputRoomCode;
    public TMP_Text ErrorMessage;

    [Header("Swiper")]
    public Image currentImage;
    public Image nextImage;

    [Header("Unlocked")]
    public Image unlockedImage;
    public GameObject panelImageUnlocked;
    public GameObject filtroText;

    [Header("Scene")]
    public GameObject loginScene;
    public GameObject mainScene;
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
        for(int i = 0; i<resourcesImage.Count;i++)
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
                imgCompl.filtro = cic.filtro;
                imgCompl.isUnlocked = false;
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
        int clusterCasuale = 99;
        do
        {
            clusterCasuale = (int)UnityEngine.Random.Range(0, 6);
        } while (imgClusters[(int)clusterCasuale].Count < 0);


        return (Clusters)clusterCasuale;
    }

    public void SpawnNewImage()
    {
        Clusters c = ChooseClusterToSpawn();
        Immagine i = ChooseNewImage(c);
        spawnedImage = i;
        currentImage.sprite = i.imageConfig.image;
        numberSpawnedImages++;
    }

    public bool CheckFinishExperience()
    {
        if (numberSpawnedImages > maxNumberReach)
        {
            if (filterUnlocked)
            {
                LinkAPIManager.instance.AddClusterValues();
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

        LinkAPIManager.instance.AddInteraction(spawnedImage, false);
    }

    public void Accept()
    {
        foreach (ValuesImage VI in spawnedImage.imageConfig.AcceptedValues)
        {
            SlidersManager.instance.UpdateSliders(VI);
        }

        LinkAPIManager.instance.AddInteraction(spawnedImage, true);
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
        if (!icomp.isUnlocked)
        {
            panelImageUnlocked.gameObject.SetActive(true);
            unlockedImage.sprite = icomp.sprite;
            filtroText.gameObject.SetActive(icomp.hasFilter);
            icomp.isUnlocked = true;
            if (icomp.hasFilter)
            {

                UnlockFilter(icomp);
                LinkAPIManager.instance.AddUnlockedFilters(icomp.filtro.id_nome);

            }
            LinkAPIManager.instance.AddUnlockedImage(icomp.IDCompleteImage);
            StartCoroutine(CountdownImgUnlocked());
        }
    }

    IEnumerator CountdownImgUnlocked()
    {
        isCardUnlockedShown = true;
        yield return new WaitForSeconds(3f);
        panelImageUnlocked.gameObject.SetActive(false);
        filtroText.gameObject.SetActive(false);
        isCardUnlockedShown = false;
        if (experienceFinished)
        {
            panelFinished.SetActive(true);
        }
    }

    public void UnlockFilter(ImmagineCompleta ic)
    {
        filterUnlocked = true;
        FiltriManager.instance.UpdateFilters(ic.filtro); //controll
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

    public void OpenScreenshootScene()
    {
        mainScene.SetActive(false);
        ARManager.instance.fotoScene.SetActive(true);
        FiltriManager.instance.UIfilter.gameObject.SetActive(true);
    }

    public void OnClickJoinRoom()
    {
        if (inputNickname.text == "" || inputRoomCode.text == "")
        {
            StartCoroutine(ShowErrorMessage("Compila i campi vuoti"));
        }
        else
        {
            Debug.Log("Nick: " + inputNickname.text);
            Debug.Log("RoomCode: " + inputRoomCode.text);
            //LinkAPIManager.instance.SendRequestJoinRoom(inputNickname.text, inputRoomCode.text);
            StartGame();
        }
    }

    public void StartGame()
    {
        loginScene.SetActive(false);
        mainScene.SetActive(true);
        Init();
    }

    public IEnumerator ShowErrorMessage(string error)
    {
        ErrorMessage.text = error;
        yield return new WaitForSeconds(5f);
        ErrorMessage.text = "";

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

[Serializable]
public class ImmagineCompleta
{
    public string IDCompleteImage;
    public Sprite sprite;
    public bool hasFilter;
    public bool isUnlocked;
    public FiltroModel filtro;
}
