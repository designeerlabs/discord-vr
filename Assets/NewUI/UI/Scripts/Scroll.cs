using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Scroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform rt;
    private bool scroll = false;
    [SerializeField]
    bool up = true;
    [SerializeField]
    float speed = 2f;
    [SerializeField]
    bool unconstrained = false;

    float maxScrollUp;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        scroll = true;
        maxScrollUp = rt.parent.GetComponent<RectTransform>().sizeDelta.y;
    }

    private void Update()
    {
        if (scroll)
        {
            if (up)
            {
                rt.anchoredPosition = new Vector2(
                    rt.anchoredPosition.x,
                    Mathf.Clamp(
                        rt.anchoredPosition.y + speed,
                        -Mathf.Infinity,
                        unconstrained ? Mathf.Infinity : rt.sizeDelta.y - maxScrollUp
                    )
                );
            }
            else
            {
                rt.anchoredPosition = new Vector2(
                    rt.anchoredPosition.x,
                    Mathf.Clamp(
                        rt.anchoredPosition.y - speed,
                        0f,
                        Mathf.Infinity
                    )
                );
            }
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    
    {
        scroll = false;
    }
}
