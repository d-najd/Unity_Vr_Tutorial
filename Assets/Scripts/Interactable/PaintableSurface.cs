using System;
using Unity.VisualScripting;
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

    private void Start()
    {
        if (SurfaceName.Length > 20)
            throw new InvalidOperationException("Surface name should be 20 characters max");
        
        // The texture is being replaced because unity does not refresh the textures when exiting playmode.
        var originalMaterial = GetComponent<Renderer>().material;
    }
}
