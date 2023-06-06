using UnityEngine;

public class BaseBrushContainer
{
    public readonly GameObject Container;
    public readonly Camera RenderCamera;
    
    public BaseBrushContainer(
        GameObject container,
        Camera renderCamera
        )
    {
        this.Container = container;
        this.RenderCamera = renderCamera;
    }
}
