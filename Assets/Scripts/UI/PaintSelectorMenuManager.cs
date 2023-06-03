using JetBrains.Annotations;
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
    [FormerlySerializedAs("currentBrushPreview")] [SerializeField] private GameObject previewBrush;

    [FormerlySerializedAs("defaultBrush")]
    [Header("Default Paint")]
    [SerializeField] private Texture2D defaultPaint;
    [SerializeField] private Texture2D defaultDecal;
    [SerializeField] private Color defaultColor;
    [SerializeField] [Range(0, 1)] private float defaultStrength = 1f;
    [SerializeField] [Range(0, 1)] private float defaultSize = .5f;
    // The alpha of the color can be used as a strength 
    // [SerializeField] [Range(0, 1)] private float defaultStrength = 1f;
    [SerializeField] private bool defaultUseDecal;

    /// The scale of the preview brush when the game goes in runtime
    private Vector3 _originalPreviewBrushScale;

    public void ResetBrush()
    {
        paintGunScript.BrushSize = defaultSize;
        paintGunScript.BrushPaint = defaultPaint;
        paintGunScript.BrushColor = defaultColor;
        
        SetPreviewBrushPaint(defaultPaint);
        SetPreviewBrushColor(defaultColor);
        SetPreviewBrushSize(defaultSize);
    }

    public void ChangeBrushPaint([NotNull] RawImage rawImage)
    {
        paintGunScript.BrushPaint = (rawImage.mainTexture as Texture2D)!;
        SetPreviewBrushPaint(paintGunScript.BrushPaint);
    }

    public void ChangeBrushSize([NotNull] Slider slider)
    {
        paintGunScript.BrushSize = slider.value;
        SetPreviewBrushSize(paintGunScript.BrushSize);
    }
    
    public void ChangeColor([NotNull] RawImage rawImage)
    {
        paintGunScript.BrushColor = rawImage.color;
        SetPreviewBrushColor(paintGunScript.BrushColor);
    }

    private void SetPreviewBrushPaint([NotNull] Texture2D brushTexture)
    {
        var previewBrushImage = previewBrush.GetComponent<Image>();
        var pivot = new Vector2(0.5f, 0.5f);
        var tRect = new Rect(0, 0, brushTexture.width, brushTexture.height);
        previewBrushImage.overrideSprite = Sprite.Create(brushTexture, tRect, pivot); 
    }

    /// <param name="sizeScale">Value between 0 and 1, scale is calculated using <c>MaxBrushSizePc</c></param>
    private void SetPreviewBrushSize(float sizeScale)
    {
        var pTransform = previewBrush.GetComponent<RectTransform>();
        pTransform.localScale = new Vector3(
            _originalPreviewBrushScale.x * sizeScale,
            _originalPreviewBrushScale.y * sizeScale, 
            _originalPreviewBrushScale.z * sizeScale);
    }

    private void SetPreviewBrushColor(Color newColor)
    {
        var previewBrushImage = previewBrush.GetComponent<Image>();
        previewBrushImage.color = newColor;
    }
    
    private void Start()
    {
        _originalPreviewBrushScale = previewBrush.GetComponent<RectTransform>().localScale;
        
        SetPreviewBrushColor(paintGunScript.BrushColor);
        SetPreviewBrushPaint(paintGunScript.BrushPaint);
        SetPreviewBrushSize(paintGunScript.BrushSize);
    }
}
