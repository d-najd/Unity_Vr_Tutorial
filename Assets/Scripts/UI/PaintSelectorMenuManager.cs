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

    [Header("Setup")] 
    [SerializeField] 
    private Slider brushSizeSlider;
    [SerializeField] 
    private Slider brushStrengthSlider;

    /// The scale of the preview brush image component when the game goes in runtime
    private Vector3 _previewBrushImageComponentScale;

    public void ResetBrush()
    {
        texturePainter.BrushSize = defaultSize;
        texturePainter.BrushSpriteRenderer.sprite = defaultPaint;
        texturePainter.BrushColor = defaultColor;
        texturePainter.BrushStrength = defaultStrength;
        
        SetPreviewBrushPaint(defaultPaint);
        SetPreviewBrushColor(defaultColor);
        SetPreviewBrushSize(defaultSize);
        SetPreviewBrushStrength(defaultStrength);

        brushSizeSlider.value = defaultSize;
        brushStrengthSlider.value = defaultStrength;
    }

    public void ChangeBrushPaint([NotNull] Image rawImage)
    {
        texturePainter.BrushSpriteRenderer.sprite = rawImage.sprite;
        SetPreviewBrushPaint(texturePainter.BrushSpriteRenderer.sprite);
    }

    public void ChangeBrushSize()
    {
        texturePainter.BrushSize = brushSizeSlider.value;
        SetPreviewBrushSize(texturePainter.BrushSize);
    }
    
    public void ChangeStrength()
    {
        texturePainter.BrushStrength = brushStrengthSlider.value;
        SetPreviewBrushStrength(texturePainter.BrushStrength);
    }
    
    public void ChangeBrushColor([NotNull] Image rawImage)
    {
        texturePainter.BrushColor = rawImage.color;
        SetPreviewBrushColor(texturePainter.BrushColor);
    }

    private void SetPreviewBrushPaint([NotNull] Sprite sprite)
    {
        var previewBrushImage = previewBrush.GetComponent<Image>();
        previewBrushImage.overrideSprite = sprite;
    }

    /// <param name="size">must be value between 0 and 1</param>
	/// <exception cref="ArgumentOutOfRangeException">if the value is not between 0-1f</exception>
    private void SetPreviewBrushSize(float size)
    {
        if (size < 0f || size > 1f ) throw new ArgumentOutOfRangeException(nameof(size), "out of bounds, expected range 0-1f");
        
        var rTransform = previewBrush.GetComponent<RectTransform>();
        rTransform.localScale = new Vector3(
            _previewBrushImageComponentScale.x * size,
            _previewBrushImageComponentScale.y * size, 
            _previewBrushImageComponentScale.z * size);
    }

    private void SetPreviewBrushStrength(float strength)
    {
        if (strength < 0f || strength > 1f ) throw new ArgumentOutOfRangeException(nameof(strength), "out of bounds, expected range 0-1f");

        var previewBrushImage = previewBrush.GetComponent<Image>();
        previewBrushImage.color = new Color(previewBrushImage.color.r, previewBrushImage.color.g, previewBrushImage.color.b, strength);
    }

    private void SetPreviewBrushColor(Color newColor)
    {
        var previewBrushImage = previewBrush.GetComponent<Image>();
        previewBrushImage.color = new Color(newColor.r, newColor.g, newColor.b, previewBrushImage.color.a);
    }
    
    private void Start()
    {
        _previewBrushImageComponentScale = previewBrush.GetComponent<RectTransform>().localScale;
        
        SetPreviewBrushColor(texturePainter.BrushColor);
        SetPreviewBrushPaint(texturePainter.BrushSpriteRenderer.sprite);
        SetPreviewBrushSize(texturePainter.BrushSize);
        SetPreviewBrushStrength(texturePainter.BrushStrength);
    }
}
