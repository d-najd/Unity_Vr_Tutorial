using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
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

    [FormerlySerializedAs("defaultBrush")]
    [Header("Default Paint")]
    [SerializeField] private Texture2D defaultPaint;
    [SerializeField] private Texture2D defaultDecal;
    [SerializeField] private Color defaultColor;
    [SerializeField] [Range(0, 1)] private float defaultStrength = 1f;
    [SerializeField] [Range(0, 1)] private float defaultSize = .5f;
    // The alpha of the color can be used as a strength 
    // [SerializeField] [Range(0, 1)] private float defaultStrength = 1f;
    [SerializeField] private bool defaultUseDecal = false;

    public void ResetBrush()
    {
        paintGunScript.BrushPaint = defaultPaint;
        paintGunScript.BrushColor = defaultColor;
        
        SetPreviewBrushPaint(defaultPaint);
        SetPreviewBrushColor(defaultColor);
    }

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
