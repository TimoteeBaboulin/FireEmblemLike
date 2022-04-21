using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITracking : MonoBehaviour
{
    private Camera Camera;
    private RectTransform Rt;
    
    public GameObject Character;

    private void Awake()
    {
        Camera = Camera.main;
        Rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Character != null) {
            Vector2 pos = new Vector2();
            pos = RectTransformUtility.WorldToScreenPoint(Camera, Character.transform.position);
            if (pos.x > Screen.width * 3 / 4)
                pos.x -= Rt.rect.width;
            if (pos.y > Screen.height * 2 / 3)
                pos.y -= Rt.rect.height;
            Rt.anchoredPosition = pos;
        }
    }
}
