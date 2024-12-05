using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public GameObject card;
    public CardLogic cl;
    float speed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && cl.isMouseOver)
        {
            Vector2 grabPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            card.transform.position = grabPoint;//new Vector2(pos.x,card.transform.position.y);
        }
        else
        {
            card.transform.position = Vector2.MoveTowards(card.transform.position, new Vector2(0,0), speed);
        }

        
    }
}
