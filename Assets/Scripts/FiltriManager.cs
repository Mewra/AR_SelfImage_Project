using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FiltriManager : MonoBehaviour
{
    public static FiltriManager instance;
    public Volume GlobalCameraFilter;
    public GameObject facemesh;
    public GameObject faceARDefault;
    public Image UIfilter;

    //filtri definitivi
    private VolumeProfile defCameraFilter;
    private Material defFaceFilter;
    private GameObject defEffettoParticellare;
    private Image defUIFilter;
    private GameObject def3DObject;


    private void Awake()
    {
        instance = this;
    }

    public void UpdateFilters(FiltroModel filtro)
    {
        if (filtro.cameraFilter != null)
        {
            Debug.Log("Aggiungo CameraFilter");
            GlobalCameraFilter.profile = filtro.cameraFilter;
        }
        if (filtro.faceFilter != null)
        {
            Debug.Log("Aggiungo FaceFilter " + filtro.faceFilter.gameObject.name);
            Instantiate(filtro.faceFilter, faceARDefault.transform);
            //facemesh.GetComponent<SkinnedMeshRenderer>().material = filtro.faceFilter;
            //faceARDefault.GetComponent<MeshRenderer>().material = filtro.faceFilter;
        }
        if (filtro.effettoParticellare != null)
        {
            Debug.Log("Aggiungo EffPartic");
            Instantiate(filtro.effettoParticellare, faceARDefault.transform);
        }
        if (filtro.UIFilter != null)
        {
            Debug.Log("Aggiungo UI");
            UIfilter.sprite = filtro.UIFilter;
        }
        if(filtro._3DObject != null)
        {
            Debug.Log("Aggiungo 3D OBJ " + filtro._3DObject.gameObject.name);
            Instantiate(filtro._3DObject, faceARDefault.transform);
        }

    }
}
