using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class flower : MonoBehaviour
{
    void OnMouseDown()
    {
        GamePanel.Instance.ClickFlower(transform);
    }

}
