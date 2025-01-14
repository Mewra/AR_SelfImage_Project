using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static ImagesConfig;

public class SlidersManager : MonoBehaviour
{
    public static SlidersManager instance;
    // Start is called before the first frame update
    public List<SliderClusterModel> sliderClusters;

    public void Awake()
    {
        instance = this;
    }

    public void UpdateSliders(ValuesImage update)
    {
        foreach(SliderClusterModel sc in sliderClusters)
        {
            if(sc.cluster == update.cluster)
            {

                Image img = sc.slider.fillRect.GetComponent<Image>();
                if (update.value > 0)
                {
                    StartCoroutine(ChangeSliderColor(img, Color.green));
                }
                else
                {
                    StartCoroutine(ChangeSliderColor(img, Color.red));
                }
                sc.slider.value += update.value;
            }
        }
    }

    private IEnumerator ChangeSliderColor(Image img, Color col)
    {
        img.color = col;
        yield return new WaitForSeconds(3f);
        img.color = Color.white;
    }

    public void ResetSliders()
    {
        foreach (SliderClusterModel sc in sliderClusters)
        {
            sc.slider.value = 50;//baseSliderValue;
        }
    }
}

