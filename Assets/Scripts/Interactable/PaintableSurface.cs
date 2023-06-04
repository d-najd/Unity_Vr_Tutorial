using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Marker class for paintable surfaces
/// </summary>
/// <remarks>
/// Paintable surfaces must be <c>Mesh Colliders</c>, and have main material which has an <c>albedo</c>, this texture
/// must be of type <c>Texture 2D</c>, have <c>read & write</c> enabled and use <c>RGBA 32 bit</c> format (other formats except default which
/// does not work are not tested)
/// </remarks>
/// TODO rename this to PaintableSurface
public class PaintableSurface : MonoBehaviour
{
    /// <summary>
    /// name for the paintable surface, should not exceed 20 characters
    /// </summary>
    [SerializeField] private string surfaceName;
    public string SurfaceName => surfaceName;
    private Texture2D _clonedOriginalTexture;

    private void Start()
    {
        if (surfaceName.Length > 20)
            throw new InvalidOperationException("Surface name should be 20 characters max");
        
        // The texture is being replaced because unity does not refresh the textures when exiting playmode.
        var originalMaterial = GetComponent<Renderer>().material;
        var originalTexture = originalMaterial.mainTexture;

        // Textures smaller than 1024 pixels need to be resized
        if (originalTexture.width < 1024 || originalTexture.height < 1024)
        {
            var resizeRatio = 1024 / Math.Min(originalTexture.width, originalTexture.height);
            _clonedOriginalTexture = TexturesUtil.Resize(
                originalTexture as Texture2D,
                originalTexture.width * resizeRatio,
                originalTexture.height * resizeRatio
            );
        }
        else
        {
            // The texture is being cloned here because unity doesn't refresh the textures when exiting playmode
            _clonedOriginalTexture = TexturesUtil.CloneTexture2D(originalTexture as Texture2D);
        }

        originalMaterial.mainTexture = TexturesUtil.CloneTexture2D(_clonedOriginalTexture);
    }
}
