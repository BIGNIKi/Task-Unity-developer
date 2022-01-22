using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardState : MonoBehaviour
{
    private bool isOpened = false;

    public bool IsOpened()
    {
        return isOpened;
    }

    public void SetIsOpened(bool val)
    {
        isOpened = val;
    }

    public void OnClickCallback1()
    {
        isOpened = !isOpened;
    }
}
