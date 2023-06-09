using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class BrushContainerCreator
{
    private static readonly object Sync = new object();
    
    private static readonly Vector3 InitialOffset = new Vector3(0, -1000, 0);
    private static readonly Vector3 OffsetBetweenContainers = new Vector3(20, 0, 0);
    private static int _initializedContainers;
    private static int InitializedContainers
    {
        get
        {
            lock (Sync)
            {
                ++_initializedContainers;
                return _initializedContainers;
            }
        }
    }

    public const float CameraOrthographicSize = 0.5f;
    
    /// <param name="textureSprite">A sprite of the main texture (albedo)</param>
    /// <returns>
    /// Brush Container which initialize <paramref name="textureSprite"/> as sprite thus applying the
    /// texture to the render texture
    /// </returns>
    [NotNull] public static BrushContainer NewBrushContainer(Sprite textureSprite)
    {
        // Initializing base container
        var baseBrushContainer = NewBrushContainer();
        
        // Converting the given sprite (main texture) to game object making it child of the container
        var textureSpriteToGameObject = new GameObject()
        {
            isStatic = true,
            layer = (int) LayersEnum.RenderTexture,
            transform =
            {
                parent = baseBrushContainer.Container.transform,
                // The local position is not at z 0 because we want to use kind of a "layer" system. with the main 
                // texture behind everything else
                localPosition = new Vector3(0, 0, 2f), 
                localScale = new Vector3(0.1f, 0.1f, 0.1f)
            },
        };
        var spriteRenderer = textureSpriteToGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = textureSprite;
        
        // setting up the render camera
        var renderTexture = new RenderTexture(
            spriteRenderer.sprite.texture.width, 
            spriteRenderer.sprite.texture.height, 
            GraphicsFormat.R8G8B8_UNorm, 
            GraphicsFormat.D24_UNorm_S8_UInt
        );
        baseBrushContainer.RenderCamera.targetTexture = renderTexture;
        
        return new BrushContainer(baseBrushContainer, renderTexture);
    }

    /// <summary>
    /// Base brush container. NOTE the render camera IS NOT set up to render on a render texture.
    /// </summary>
    [NotNull] public static BaseBrushContainer NewBrushContainer()
    {
        var curInitializedContainer = InitializedContainers;
        
        // Creating the holder which will hold the render camera and the container for the brushes
        var holder = new GameObject($"_BrushContainerHolder {curInitializedContainer}")
        {
            isStatic = true,
            layer = (int) LayersEnum.RenderTexture,
            transform =
            {
                position = new Vector3(
                    InitialOffset.x + (OffsetBetweenContainers.x * curInitializedContainer),
                    InitialOffset.y,
                    InitialOffset.z + (OffsetBetweenContainers.z * curInitializedContainer)
                )
            }
        };
        
        // Creating the container for the brushes
        var container = new GameObject("_BrushContainer")
        {
            isStatic = true,
            layer = (int) LayersEnum.RenderTexture,
            transform =
            {
                parent = holder.transform,
                localPosition = Vector3.zero
            }
        };

        // Creating the render camera used to render camera for render texture
        var renderCamera = new GameObject("_RenderCamera")
        {
            isStatic = true,
            layer = (int) LayersEnum.RenderTexture,
            transform =
            {
                parent = holder.transform,
                localPosition = new Vector3(0, 0, -2),
            }
        };  
        
        // Adding the camera component and some settings
        renderCamera.AddComponent<Camera>();
        var cameraComponent = renderCamera.GetComponent<Camera>();
        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = CameraOrthographicSize;
        cameraComponent.nearClipPlane = .3f;
        cameraComponent.farClipPlane = 5f;
        cameraComponent.clearFlags = CameraClearFlags.Nothing; 
        // TODO find out why this wont work
        // cameraComponent.cullingMask = (int)LayersEnum.RenderTexture;

        // var extraCameraData = renderCamera.GetComponent<UniversalAdditionalCameraData>();

        return new BaseBrushContainer(
            container,
            renderCamera.GetComponent<Camera>()
        );
    }
}


