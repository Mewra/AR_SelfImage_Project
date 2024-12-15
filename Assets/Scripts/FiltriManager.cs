using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FiltriManager : MonoBehaviour
{
    public static FiltriManager instance;
    public Volume GlobalCameraFilter;
    public GameObject facemesh;
    public GameObject faceARDefault;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateFilters(VolumeProfile cameraFilter, Material faceFilter)
    {
        GlobalCameraFilter.profile = cameraFilter;
        facemesh.GetComponent<SkinnedMeshRenderer>().material = faceFilter;
        faceARDefault.GetComponent<MeshRenderer>().material = faceFilter;
    }
}
