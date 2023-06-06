using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PaintSelectorMenuManager : MonoBehaviour
{
    // TODO maybe it will be better to check if the player actually holds a paint gun and then use its reference but this is more convenient
    [FormerlySerializedAs("texturePainterScript")] [FormerlySerializedAs("paintGunScript")] [SerializeField] private OurTexturePainter ourTexturePainterScript;

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
        ourTexturePainterScript.BrushSize = defaultSize;
        // ourTexturePainterScript.BrushPaint = defaultPaint;
        ourTexturePainterScript.BrushColor = defaultColor;
        
        SetPreviewBrushPaint(defaultPaint);
        SetPreviewBrushColor(defaultColor);
        SetPreviewBrushSize(defaultSize);
    }

    public void ChangeBrushPaint([NotNull] RawImage rawImage)
    {
        // ourTexturePainterScript.BrushPaint = (rawImage.mainTexture as Texture2D)!;
        // SetPreviewBrushPaint(ourTexturePainterScript.BrushPaint);
    }

    public void ChangeBrushSize([NotNull] Slider slider)
    {
        ourTexturePainterScript.BrushSize = slider.value;
        SetPreviewBrushSize(ourTexturePainterScript.BrushSize);
    }
    
    public void ChangeColor([NotNull] RawImage rawImage)
    {
        ourTexturePainterScript.BrushColor = rawImage.color;
        SetPreviewBrushColor(ourTexturePainterScript.BrushColor);
    }

    private void SetPreviewBrushPaint([NotNull] Texture2D brushTexture)
    {
        var previewBrushImage = previewBrush.GetComponent<Image>();
        previewBrushImage.overrideSprite = TexturesUtil.Texture2DToSprite(brushTexture);
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
        
        SetPreviewBrushColor(ourTexturePainterScript.BrushColor);
        // TODO fetching the brush from the player is buggy for some reason, delay does not work
        // SetPreviewBrushPaint(ourTexturePainterScript.BrushPaint);
        SetPreviewBrushSize(ourTexturePainterScript.BrushSize);
    }
}
