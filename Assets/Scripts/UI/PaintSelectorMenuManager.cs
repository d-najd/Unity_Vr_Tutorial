using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class PaintSelectorMenuManager : MonoBehaviour
{
    // TODO maybe it will be better to check if the player actually holds a paint gun and then use its reference but this is more convenient
    [SerializeField] private PaintGun paintGunScript;

    /// <summary>
    /// The preview brush in the paint selector menu
    /// </summary>
    /// <remarks>Must contain the components <c>Rect Transform</c> and <c>Raw Image</c></remarks>
    [SerializeField] private GameObject currentBrushPreview;
    
    public void ChangeBrushPaint(Texture2D newBrushTexture)
    {
        paintGunScript.BrushPaint = newBrushTexture;
    }
    
    public void ChangeColor(Color color)
    {
        paintGunScript.BrushColor = color;
    }

    /// <summary>
    /// Method for changing the preview brush, fields with null value won't be updated
    /// </summary>
    private void UpdatePreviewBrushColor([CanBeNull] Texture2D newBrushTexture, [CanBeNull] Color newColor)
    {
        if (newBrushTexture != null)
        {
            // currentBrushPreview
        }
    }
}
