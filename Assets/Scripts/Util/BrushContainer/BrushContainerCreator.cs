using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class BrushContainerCreator
{
    private static readonly Vector3 InitialOffset = new Vector3(0, -1000, 0);
    private static readonly Vector3 OffsetBetweenContainers = new Vector3(20, 0, 0);
    // TODO create an accessor to this value which will be synchronized, nothing else has to be synchronized.
    private const int InitializedContainers = 0;
    
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
            transform =
            {
                parent = baseBrushContainer.Container.transform
            }
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


