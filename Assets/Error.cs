using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Error : MonoBehaviour
{
    [SerializeField]
    private Transform errorTrans;
    [SerializeField]
    private Text errorField;

    public void SetError(string e)
    {
        errorField.text = e;
    }

    public void RemoveError()
    {
        Transform t = transform;
        if (errorTrans)
        {
            t = errorTrans;
        }
        Destroy(t.gameObject);
    }
}
