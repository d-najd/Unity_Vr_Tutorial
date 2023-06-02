using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintGun : MonoBehaviour
{
	/// <summary>
	/// The paint brush that will be used for painting
	/// </summary>
	[SerializeField] 
	private Texture2D brushPaint;
	[SerializeField] 
	private Color brushColor;
	/// <see cref="SetBrushColor"/> Method for changing the brush color
	/// <remarks>Texture changes on runtime from the inspector are not accounted for</remarks>
	private bool _isBrushColorChanged;
	/// <summary>
	/// Cached Brushed color pixels
	/// </summary>
	/// <see cref="Start"/> Pixels get generated here on start
	/// <see cref="BrushPaintPixelsInstance"/> Pixels get regenerated if necessary here
	private Color[] _cachedBrushColorPixels;

	/// <summary>
	/// the exit of the barrel of the paint gun
	/// </summary>
	[SerializeField] 
	private Transform paintGunBarrelExit; 
	[SerializeField]
	private float paintDistance = 30f;

	private bool _painting;
	private XRGrabInteractable _grabbable;

	/// <see cref="BrushPaintPixelsInstance"/>
	/// <see cref="GenerateBrushColorPixels"/>
	public void SetBrushColor(Color newColor)
	{
		brushColor = newColor;
		_isBrushColorChanged = true;
	}
	
	/// <summary>
	/// Painting on the texture was hitted.
	/// </summary>
	/// <see cref="SetBrushColor"/> For changing the color of the brush
	private void Paint()
	{
		if (HitUVPosition(out var textureHitX, out var textureHitY, out var hitTexture) && hitTexture != null)
		{
			hitTexture.SetPixels(textureHitX, textureHitY, brushPaint.width, brushPaint.height, BrushPaintPixelsInstance());
			hitTexture.Apply();
		}
	}

	/// <summary>
	/// Determines if the ray hits a paintable surface
	/// </summary>
	/// <param name="textureHitX">The x coordinate where the ray hit in the texture. defaults to -1</param>
	/// <param name="textureHitY">The y coordinate where the ray hit in the texture, defaults to -1</param>
	/// <param name="hitTexture">The texture of the component that got hit, defaults to null</param>
	/// <returns>true if the ray managed to hit a paintable surface false otherwise</returns>
	private bool HitUVPosition(
		out int textureHitX,
		out int textureHitY,
		[CanBeNull] out Texture2D hitTexture
	)
	{
		textureHitX = -1;
		textureHitY = -1;
		hitTexture = null;

		if (Physics.Raycast(paintGunBarrelExit.position, paintGunBarrelExit.forward, out var hit, paintDistance) &&
		    hit.collider is MeshCollider &&
		    hit.transform.gameObject.TryGetComponent(out Renderer hitRenderer))
		{
			var textureHitCords = hit.textureCoord;

			hitTexture = hitRenderer.material.mainTexture as Texture2D;
			textureHitX = (int)(textureHitCords.x * hitRenderer.material.mainTexture.width);
			textureHitY = (int)(textureHitCords.y * hitRenderer.material.mainTexture.height);
			return true;
		}

		return false;
	}
	
	/*
	private static IEnumerator SaveTextureToFile(Texture2D savedTexture)
	{
		string fullPath = System.IO.Directory.GetCurrentDirectory() + "\\UserCanvas\\";
		string fileName = "CanvasTexture.png";
		if (!System.IO.Directory.Exists(fullPath))
			System.IO.Directory.CreateDirectory(fullPath);
		var bytes = savedTexture.EncodeToPNG();
		System.IO.File.WriteAllBytes(fullPath + fileName, bytes);
		Debug.Log("<color=orange>Saved Successfully!</color>" + fullPath + fileName);
		yield return null;
	}
	*/
	
	/// <summary>
	/// Returns instance of pixel data
	/// </summary>
	/// <see cref="SetBrushColor"/> For changing the color of the brush
	/// <see cref="GenerateBrushColorPixels"/> For manually regenerating the pixels
	/// <returns>Cached or new instance (if the color was changed) of the brush pixel color data</returns>
	private Color[] BrushPaintPixelsInstance()
	{
		if (_isBrushColorChanged)
		{
			_cachedBrushColorPixels = GenerateBrushColorPixels();
		}
		
		return _cachedBrushColorPixels;
	}

	/// <summary>
	/// Generates pixel color data using the current brush and color
	/// </summary>
	/// <remarks>The color is copied from the brush paint and the alfa from the current pixel</remarks>
	/// <returns>New instance of the color pixels</returns>
	/// <see cref="BrushPaintPixelsInstance"/> Should be used whenever possible because it caches the data
	private Color[] GenerateBrushColorPixels()
	{
		var pixels = brushPaint.GetPixels();
		for (var p = 0; p < pixels.Length; p++)
		{
			var curPixel = pixels[p];
			pixels[p] = new Color(brushColor.r, brushColor.g, brushColor.b, curPixel.a);
		}

		return pixels;
	}
	
	private void Start()
	{
		// Generate Brush Color Pixels
		_cachedBrushColorPixels = GenerateBrushColorPixels();
		
		_grabbable = GetComponent<XRGrabInteractable>();
		_grabbable.activated.AddListener(StartPainting);
		_grabbable.deactivated.AddListener(StopPainting);
		_grabbable.selectExited.AddListener(SelectExited);
	}

	private void Update()
	{
		if (_painting)
		{
			Paint();
		}
	}

	private void StopPainting(DeactivateEventArgs arg0)
	{
		_painting = false;
	}

	private void StartPainting(ActivateEventArgs arg)
	{
		_painting = true;
	}
	
	private void SelectExited(SelectExitEventArgs arg0)
	{
		_painting = false;
	}
}
