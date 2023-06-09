using UnityEngine;

public class BaseBrushContainer
{
    /// <summary>
    /// Holder for the brush container and render camera
    /// </summary>
    public readonly GameObject ContainerHolder;
    /// <summary>
    /// Container for the brushes
    /// </summary>
    public readonly GameObject Container;
    public readonly Camera RenderCamera;
    
    public BaseBrushContainer(
        GameObject containerHolder,
        GameObject container,
        Camera renderCamera
        )
    {
        this.ContainerHolder = containerHolder;
        this.Container = container;
        this.RenderCamera = renderCamera;
    }
}
