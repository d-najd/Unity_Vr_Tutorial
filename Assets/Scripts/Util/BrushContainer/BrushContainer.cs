using UnityEngine;

public class BrushContainer : BaseBrushContainer
{
    public readonly RenderTexture RenderTexture;
    
    public BrushContainer(BaseBrushContainer baseBrushContainer, RenderTexture renderTexture) : 
        base(baseBrushContainer.ContainerHolder, baseBrushContainer.Container, baseBrushContainer.RenderCamera)
    {
        this.RenderTexture = renderTexture;
    }
    public BrushContainer(GameObject containerHolder, GameObject container, Camera renderCamera, RenderTexture renderTexture) : 
        base(containerHolder, container, renderCamera)
    {
        this.RenderTexture = renderTexture;
    }
}
