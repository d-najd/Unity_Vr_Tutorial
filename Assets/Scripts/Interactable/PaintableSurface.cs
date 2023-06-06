using System;
using UnityEngine;

/// <summary>
/// Marker class for paintable surfaces
/// </summary>
/// <remarks>
/// Paintable surfaces must be <c>Mesh Colliders</c>, and have main material which has an <c>albedo</c>
/// </remarks>
public class PaintableSurface : MonoBehaviour
{
    /// <summary>
    /// name for the paintable surface, should not exceed 20 characters
    /// </summary>
    [SerializeField] private string surfaceName;
    public string SurfaceName => surfaceName;
    public BrushContainer BrushContainer { get; private set; }
    /// <summary>
    /// This texture will be used as the starting texture for the model. If not set manually the script will check if
    /// there is a main texture (albedo) and if there is it will attempt to use it for the render texture but is not
    /// guaranteed to succeed
    /// </summary>
    [SerializeField] private Sprite startingTextureAsSprite;

    private void Start()
    {
        if (surfaceName.Length > 20)
            throw new InvalidOperationException("Surface name should be 20 characters max");

        var mainMaterial = GetComponent<Renderer>().material;
        if (startingTextureAsSprite != null)
        {
            BrushContainer = BrushContainerCreator.NewBrushContainer(startingTextureAsSprite);
            mainMaterial.mainTexture = BrushContainer.RenderTexture;
        }
        else if (mainMaterial.mainTexture == null)
        {
            throw new InvalidOperationException(
                "Um painting on a object that does not have a texture seems dumb doesn't it?");
        }
        else if (mainMaterial.mainTexture is not Texture2D mainTexture2D)
        {
            throw new InvalidCastException(
                "Failed to use the main texture as the starter texture for the paintable surface");
        }
        else
        {
            startingTextureAsSprite = TexturesUtil.Texture2DToSprite(mainTexture2D);
            BrushContainer = BrushContainerCreator.NewBrushContainer(startingTextureAsSprite);
            mainMaterial.mainTexture = BrushContainer.RenderTexture;
        }
    }
}
