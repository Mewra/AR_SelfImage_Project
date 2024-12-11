using System;
using System.Text;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CheckOcclusion : MonoBehaviour
{
    public AROcclusionManager occlusionManager;
    public Material backgroundMaterial;
    public TMP_Text debug;

    void Update()
    {
        if (occlusionManager.environmentDepthTexture != null)
        {
            backgroundMaterial.SetTexture("_DepthTex", occlusionManager.environmentDepthTexture);
        }
        else
        {
            debug.text = "Non c'è la environmentDepthTexture";
        }
    }
}