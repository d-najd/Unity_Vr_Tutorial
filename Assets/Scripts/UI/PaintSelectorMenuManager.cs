using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PaintSelectorMenuManager : MonoBehaviour
{
    // TODO maybe it will be better to check if the player actually holds a paint gun and then use its reference but this is more convenient
    [SerializeField] private OurTexturePainter texturePainter;

    /// <summary>
    /// The preview brush in the paint selector menu
    /// </summary>
    /// <remarks>Must contain the components <c>Rect Transform</c> and <c>Raw Image</c></remarks>
    [FormerlySerializedAs("currentBrushPreview")] [SerializeField] private GameObject previewBrush;

    [FormerlySerializedAs("defaultBrush")]
    [Header("Default Paint")]
    [SerializeField] private Sprite defaultPaint;
    [SerializeField] private Sprite defaultDecal;
    [SerializeField] private Color defaultColor;
    [SerializeField] [Range(0, 1)] private float defaultStrength = 1f;
    [SerializeField] [Range(0, 1)] private float defaultSize = .25f;
    [SerializeField] private bool defaultUseDecal;

    /// The scale of the preview brush image component when the game goes in runtime
    private Vector3 _previewBrushImageComponentScale;

    public void ResetBrush()
    {
        texturePainter.BrushSize = defaultSize;
        texturePainter.BrushSpriteRenderer.sprite = defaultPaint;
        texturePainter.BrushColor = defaultColor;
        
        SetPreviewBrushPaint(defaultPaint);
        SetPreviewBrushColor(defaultColor);
        SetPreviewBrushSize(defaultSize);
    }

    public void ChangeBrushPaint([NotNull] Image rawImage)
    {
        texturePainter.BrushSpriteRenderer.sprite = rawImage.sprite;
        SetPreviewBrushPaint(texturePainter.BrushSpriteRenderer.sprite);
    }

    public void ChangeBrushSize([NotNull] Slider slider)
    {
        texturePainter.BrushSize = slider.value;
        SetPreviewBrushSize(texturePainter.BrushSize);
    }
    
    public void ChangeColor([NotNull] Image rawImage)
    {
        texturePainter.BrushColor = rawImage.color;
        SetPreviewBrushColor(texturePainter.BrushColor);
    }

    private void SetPreviewBrushPaint([NotNull] Sprite sprite)
    {
        var previewBrushImage = previewBrush.GetComponent<Image>();
        previewBrushImage.overrideSprite = sprite;
    }

    /// <param name="sizeScale">must be value between 0 and 1</param>
    private void SetPreviewBrushSize(float sizeScale)
    {
        if (sizeScale < 0f || sizeScale > 1f ) throw new ArgumentOutOfRangeException(nameof(sizeScale), "out of bounds, expected range 0-1f");
        
        var rTransform = previewBrush.GetComponent<RectTransform>();
        rTransform.localScale = new Vector3(
            _previewBrushImageComponentScale.x * sizeScale,
            _previewBrushImageComponentScale.y * sizeScale, 
            _previewBrushImageComponentScale.z * sizeScale);
    }

    private void SetPreviewBrushColor(Color newColor)
    {
        var previewBrushImage = previewBrush.GetComponent<Image>();
        previewBrushImage.color = newColor;
    }
    
    private void Start()
    {
        _previewBrushImageComponentScale = previewBrush.GetComponent<RectTransform>().localScale;
        
        SetPreviewBrushColor(texturePainter.BrushColor);
        SetPreviewBrushPaint(texturePainter.BrushSpriteRenderer.sprite);
        SetPreviewBrushSize(texturePainter.BrushSize);
    }
}
