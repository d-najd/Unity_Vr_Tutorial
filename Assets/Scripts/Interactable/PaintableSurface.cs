using UnityEngine;

/// <summary>
/// Marker class for paintable surfaces
/// </summary>
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
