using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkbox : MonoBehaviour
{
    [SerializeField]
    private Sprite spriteChecked;
    [SerializeField]
    private Sprite spriteUnchecked;

    [SerializeField]
    private bool isChecked = false;

    private UnityEngine.UI.Image image;

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }

    public void Click()
    {
        isChecked = !isChecked;
        image.sprite = (isChecked) ? spriteChecked : spriteUnchecked;
    }
}
