using UnityEngine;

public class BrushContainerCreator
{
    private static readonly Vector3 InitialOffset = new Vector3(0, -1000, 0);
    private static readonly Vector3 OffsetBetweenContainers = new Vector3(20, 0, 0);
    private const int InitializedContainers = 0;

    public static BaseBrushContainer NewBrushContainer()
    {
        // Creating the holder which will hold the render camera and the container for the brushes
        var holder = new GameObject("_BrushContainerHolder1")
        {
            transform =
            {
                position = new Vector3(
                    InitialOffset.x + (OffsetBetweenContainers.x * InitializedContainers),
                    InitialOffset.y,
                    InitialOffset.z + (OffsetBetweenContainers.z * InitializedContainers)
                )
            }
        };
        
        // Creating the container for the brushes
        var container = new GameObject("_BrushContainer")
        {
            transform =
            {
                parent = holder.transform
            }
        };
        
        // Creating the render camera used to render camera for render texture
        var renderCamera = new GameObject("_RenderCamera")
        {
            
            transform =
            {
                position = new Vector3(0, 0, -2),
                parent = holder.transform
            }
            
        };
        
        // Adding the camera component and some settings
        renderCamera.AddComponent<Camera>();
        var cameraComponent = renderCamera.GetComponent<Camera>();
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = .5f;
        cameraComponent.nearClipPlane = .3f;
        cameraComponent.farClipPlane = 5f;

        return new BaseBrushContainer(
            container,
            renderCamera.GetComponent<Camera>()
        );
    }
}


