using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintGun : MonoBehaviour
{
	/// <see cref="BrushPaint"/>
	[SerializeField]
	private Texture2D brushPaint;
	private Texture2D _brushPaintUnscaled;
	/// <summary>
	/// The paint brush that will be used for painting
	/// </summary>
	/// <remarks>
	/// The texture must be of type <c>Texture 2d</c>, have <c>read & write</c> enabled and use <c>RGBA 32 bit</c> format
	/// </remarks>
	/// <see cref="BrushPaintPixelsInstance"/>
	/// <see cref="GenerateBrushColorPixels"/>
	public Texture2D BrushPaint
	{
		get
		{
			if (_brushResizeNecessary)
			{
				brushPaint = TexturesUtil.Resize(
					_brushPaintUnscaled, 
					(int) (MaxBrushSizePx * BrushSize),
					(int) (MaxBrushSizePx * BrushSize)
					);
				_brushResizeNecessary = false;
			}
			return brushPaint;
		}
		set
		{
			_brushPaintUnscaled = value;
			brushPaint = TexturesUtil.Resize(
				value, 
				(int) (MaxBrushSizePx * BrushSize),
				(int) (MaxBrushSizePx * BrushSize)
				);
			_brushResizeNecessary = false;
			_isBrushAltered = true;
		}
	}
	private bool _brushResizeNecessary = true;

	private const int MaxBrushSizePx = 150;
	private float _brushSize = 1f;
	/// <summary>
	/// <value>Range 0 to 1 in other words a scale</value>
	/// </summary>
	public float BrushSize
	{
		get => _brushSize;
		set
		{
			if (value < .05) value = .05f;
			_brushSize = value;
			_brushResizeNecessary = true;
		}
	}

	/// <see cref="BrushColor"/>
	[SerializeField]
	private Color brushColor;
	/// <summary>
	/// The color of the brush used for painting
	/// </summary>
	/// <see cref="BrushPaintPixelsInstance"/>
	/// <see cref="GenerateBrushColorPixels"/>
	public Color BrushColor
	{
		get => brushColor;
		set
		{
			brushColor = value;
			_isBrushAltered = true;
		}
	}
	
	/// <see cref="BrushColor"/> Method for changing the brush color
	/// <remarks>Texture changes on runtime from the inspector are not accounted for</remarks>
	private bool _isBrushAltered;
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

	/// <summary>
	/// Painting on the texture on hit.
	/// </summary>
	/// <see cref="BrushColor"/> For changing the color of the brush
	private void Paint()
	{
		if (HitUVPosition(out var textureHitX, out var textureHitY, out var hitTexture) && hitTexture != null)
		{
			var hitTexturePixels = hitTexture.GetPixels(textureHitX, textureHitY, BrushPaint.width, BrushPaint.height);
			var brushPaintPixels = BrushPaintPixelsInstance();
			
			// This could be improved with threading
			for (var curP = 0; curP < hitTexturePixels.Length; curP++)
			{
				var curHitPixel = hitTexturePixels[curP];
				var curBrushPixel = brushPaintPixels[curP];

				// The alpha does not work the way I imagined it would but removing it messes the painting....
				hitTexturePixels[curP] = new Color(
					curHitPixel.r * Math.Abs(curBrushPixel.a - 1f) + curBrushPixel.r * curBrushPixel.a,
					curHitPixel.g * Math.Abs(curBrushPixel.a - 1f) + curBrushPixel.g * curBrushPixel.a,
					curHitPixel.b * Math.Abs(curBrushPixel.a - 1f) + curBrushPixel.b * curBrushPixel.a
				);
			}

			hitTexture.SetPixels(textureHitX, textureHitY, BrushPaint.width, BrushPaint.height, hitTexturePixels);
			hitTexture.Apply();
		}
	}

	// ReSharper disable Unity.PerformanceAnalysis
	/// <summary>
	/// Determines if the ray hits a paintable surface
	/// </summary>
	/// <param name="textureHitX">The x coordinate where the ray hit in the texture. defaults to -1</param>
	/// <param name="textureHitY">The y coordinate where the ray hit in the texture, defaults to -1</param>
	/// <param name="hitTexture">The texture of the component that got hit, defaults to null</param>
	/// <returns>
	/// True if the ray managed to hit a paintable surface and the bounds of the painting wont be outisde then
	/// true will be returned
	/// </returns>
	private bool HitUVPosition(
		out int textureHitX,
		out int textureHitY,
		[CanBeNull] out Texture2D hitTexture
	)
	{
		textureHitX = -1;
		textureHitY = -1;
		hitTexture = null;
		
		if (
			Physics.Raycast(paintGunBarrelExit.position, paintGunBarrelExit.forward, out var hit, paintDistance) &&
			hit.transform.gameObject.TryGetComponent(out PaintableSurface paintableSurface) &&
			paintableSurface.isActiveAndEnabled &&
		    hit.transform.gameObject.TryGetComponent(out Renderer hitRenderer)  
		    )
		{
			var textureHitCords = hit.textureCoord;

			hitTexture = (hitRenderer.material.mainTexture as Texture2D)!;
			textureHitX = (int)(textureHitCords.x * hitTexture.width);
			textureHitY = (int)(textureHitCords.y * hitTexture.height);
			
			// I am aware that this is not the best place for this but better than recalculating stuff
			if (textureHitX + BrushPaint.width > hitTexture.width ||
			    textureHitY + BrushPaint.height > hitTexture.height)
				return false;
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
	/// <see cref="BrushColor"/> For changing the color of the brush
	/// <see cref="GenerateBrushColorPixels"/> For manually regenerating the pixels
	/// <returns>Cached or new instance (if the color was changed) of the brush pixel color data</returns>
	private Color[] BrushPaintPixelsInstance()
	{
		if (_isBrushAltered)
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
		var pixels = BrushPaint.GetPixels();
		for (var p = 0; p < pixels.Length; p++)
		{
			var curPixel = pixels[p];
			pixels[p] = new Color(BrushColor.r, BrushColor.g, BrushColor.b, curPixel.a);
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

	private void FixedUpdate()
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
