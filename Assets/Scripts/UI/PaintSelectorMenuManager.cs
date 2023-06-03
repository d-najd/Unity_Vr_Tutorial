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
    
    public void ChangeBrushPaint([NotNull] Texture2D newBrushTexture)
    {
        paintGunScript.BrushPaint = newBrushTexture;
        SetPreviewBrushPaint(newBrushTexture);
    }
    
    public void ChangeColor(Color color)
    {
        paintGunScript.BrushColor = color;
        SetPreviewBrushColor(color);
    }

    private void SetPreviewBrushPaint([NotNull] Texture2D newBrushTexture)
    {
        var previewBrushImage = currentBrushPreview.GetComponent<Image>();
        var pivot = new Vector2(0.5f, 0.5f);
        var tRect = new Rect(0, 0, newBrushTexture.width, newBrushTexture.height);
        previewBrushImage.overrideSprite = Sprite.Create(newBrushTexture, tRect, pivot); 
    }

    private void SetPreviewBrushColor(Color newColor)
    {
        var previewBrushImage = currentBrushPreview.GetComponent<Image>();
        previewBrushImage.color = newColor;
    }
    
    private void Start()
    {
        SetPreviewBrushColor(paintGunScript.BrushColor);
        SetPreviewBrushPaint(paintGunScript.BrushPaint);
    }
}
