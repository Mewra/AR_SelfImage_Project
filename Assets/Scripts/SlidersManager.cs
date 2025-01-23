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
    public float baseValore;
    public Sprite piuSprite;
    public Sprite menoSprite;

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
                    StartCoroutine(ChangeSliderColor(img, new Color32(91, 220, 141, 255), sc, piuSprite));
                }
                else
                {
                    StartCoroutine(ChangeSliderColor(img, new Color32(220, 91, 128, 255), sc, menoSprite));
                }
                sc.slider.value += update.value;
            }
        }
    }

    private IEnumerator ChangeSliderColor(Image img, Color32 col, SliderClusterModel scm, Sprite valore)
    {
        img.color = col;
        scm.imgSegno.GetComponent<Image>().sprite = valore;
        scm.imgSegno.SetActive(true);
        yield return new WaitForSeconds(3f);
        scm.imgSegno.SetActive(false);
        img.color = Color.white;
    }

    public void ResetSliders()
    {
        foreach (SliderClusterModel sc in sliderClusters)
        {
            sc.slider.value = baseValore;//baseSliderValue;
        }
    }
}

