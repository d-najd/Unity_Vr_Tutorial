using UnityEngine;

public static class TexturesUtil
{
    /// <summary>
    /// Creates new instance of the texture and resizes it to the given size
    /// </summary>
    /// <param name="texture2D">The input texture, the texture to be resized</param>
    /// <param name="targetX">width of the output texture</param>
    /// <param name="targetY">height of the output texture</param>
    /// <returns>Resized clone of the original texture</returns>
    public static Texture2D Resize(Texture2D texture2D,int targetX,int targetY)
    {
        var rt = new RenderTexture(targetX, targetY,24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D,rt);
        var result = new Texture2D(targetX,targetY);
        result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        result.Apply();
        return result;
    }
    
    /// <summary>
    /// Clones the texture and returns the clone
    /// </summary>
    /// <param name="originalTexture">The input texture to be cloned</param>
    /// <returns>Clone of the original texture</returns>
   public static Texture2D CloneTexture2D(Texture2D originalTexture)
   {
      var newTexture = new Texture2D(originalTexture.width, originalTexture.height);
      newTexture.SetPixels(originalTexture.GetPixels());
      return newTexture;
   }
}