using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoDebug : MonoBehaviour
{
    [SerializeField] bool showPlayerPosition = true;

    private void OnGUI()
    {
        if (showPlayerPosition)
        {
            Vector3 pos = gameObject.transform.position;
            GUI.Label(new Rect(10f, 10f, 400f, 100f), "Distance from origin: " + Math.Round(pos.x, 2) + ", " + Math.Round(pos.y, 2) + ", " + Math.Round(pos.z, 2));
        }
    }
}
