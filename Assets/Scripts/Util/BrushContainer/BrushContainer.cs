using UnityEngine;

public class BrushContainer : BaseBrushContainer
{
    public readonly RenderTexture RenderTexture;
    
    public BrushContainer(BaseBrushContainer baseBrushContainer, RenderTexture renderTexture) : 
        base(baseBrushContainer.Container, baseBrushContainer.RenderCamera)
    {
        this.RenderTexture = renderTexture;
    }
    public BrushContainer(GameObject container, Camera renderCamera, RenderTexture renderTexture) : 
        base(container, renderCamera)
    {
        this.RenderTexture = renderTexture;
    }
}
