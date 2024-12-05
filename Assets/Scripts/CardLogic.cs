using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardLogic : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public bool isMouseOver = false;
    private RectTransform rectTransform;
    private Image image;
    public Canvas canvas;
    float speed = 0.2f;
    private float lerpTime = 0f;
    public Vector2 targetPosition = Vector2.zero;
    public bool endDrag = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }
    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Trascinamento iniziato");
        endDrag = false;
        targetPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Sposta l'oggetto in base alla posizione del mouse (per UI)
        Vector2 newPosition = rectTransform.anchoredPosition;
        newPosition.x += eventData.delta.x / canvas.scaleFactor;
        rectTransform.anchoredPosition = newPosition;


        if (rectTransform.anchoredPosition.x > 100)
        {
            image.color = Color.green;
        }
        else
        if (rectTransform.anchoredPosition.x < -100)
        {
            image.color = Color.red;
        }
        else
        {
            image.color = Color.white;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endDrag = true;
        lerpTime = 0;
        //targetPosition = Vector2.zero;
        //RIGHT SIDE
        if (rectTransform.anchoredPosition.x > 100)
        {
            GameManager.instance.Accept();
            targetPosition = new Vector2(450, 0);
        }else

        if (rectTransform.anchoredPosition.x < -100)
        {
            GameManager.instance.Reject();
            targetPosition = new Vector2(-450, 0);
        }

        //rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, new Vector2(0, 0), speed*Time.deltaTime);
        

    }

    private void Update()
    {
        if (endDrag)
        {
            lerpTime += Time.deltaTime * speed;

            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPosition,
                lerpTime
            );
            
            if (targetPosition.x != 0)
            {
                Debug.Log(targetPosition);
                if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 0.01f)
                {

                    GameManager.instance.SpawnNewImage();
                    targetPosition = Vector2.zero;
                    rectTransform.anchoredPosition = targetPosition;
                    image.color = Color.white;

                }
            }
            
        }
    }
}
