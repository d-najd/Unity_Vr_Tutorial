using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPaintStatsMenuManager : MonoBehaviour
{
    [SerializeField] private PaintableSurface paintableSurface;
    [SerializeField] private Texture2D startingMaterial;
    [SerializeField] private Texture2D targetMaterial;
    /// <summary>
    /// Used to determine whether the menu is active and if it is to update it
    /// </summary>
    [SerializeField] private GameObject menu;

    private void FixedUpdate()
    {
        if (menu.activeSelf)
        {
            
        }
    }
}
