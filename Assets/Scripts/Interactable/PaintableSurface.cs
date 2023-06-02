using UnityEngine;

/// <summary>
/// Marker class for paintable surfaces
/// </summary>
/// <remarks>
/// Paintable surfaces must be <c>Mesh Colliders</c>, and have main material which has an <c>albedo</c>, this texture
/// must be of type <c>Texture 2D</c>, have <c>read & write</c> enabled and use <c>RGBA 32 bit</c> format (other formats except default which
/// does not work are not tested)
/// </remarks>
public class PaintableSurface : MonoBehaviour
{
   private void Start()
   {
      // The texture is being replaced because unity does not refresh the textures when exiting playmode.
      var originalMaterial = GetComponent<Renderer>().material;
      originalMaterial.mainTexture = CloneTexture2D(originalMaterial.mainTexture as Texture2D);
   }

   private static Texture2D CloneTexture2D(Texture2D originalTexture)
   {
      var newTexture = new Texture2D(originalTexture.width, originalTexture.height);
      newTexture.SetPixels(originalTexture.GetPixels());
      return newTexture;
   }
}
