using UnityEngine;

public static class TexturesUtil
{
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
}