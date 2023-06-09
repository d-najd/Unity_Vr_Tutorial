using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OurTexturePainter : MonoBehaviour
{
	/// <summary>The paint brush that will be used for painting</summary>
	/// <remarks>Should not be modified in runtime</remarks>
	[SerializeField]
	private GameObject brushEntity;
	public GameObject BrushEntity
	{
		get => brushEntity;
		set
		{
			brushEntity = value;
			BrushSpriteRenderer = value.GetComponent<SpriteRenderer>();
		}
	}

	private SpriteRenderer _brushSpriteRenderer;
	public SpriteRenderer BrushSpriteRenderer
	{
		get
		{
			if (_brushSpriteRenderer == null)
			{
				_brushSpriteRenderer = brushEntity.GetComponent<SpriteRenderer>();
			}

			return _brushSpriteRenderer;
		}
		private set => _brushSpriteRenderer = value;
	}

	/// <summary>The decal that will be used for painting</summary>
	/// <remarks>Should not be edited in runtime</remarks>
	[SerializeField] private GameObject decalEntity;

	private SpriteRenderer _decalSpriteRenderer;
	public SpriteRenderer DecalSpriteRenderer
	{
		get
		{
			if (_decalSpriteRenderer == null)
			{
				_decalSpriteRenderer = brushEntity.GetComponent<SpriteRenderer>();
			}

			return _decalSpriteRenderer;
		}
		private set => _decalSpriteRenderer = value;
	}
	
	[SerializeField]
	private Color brushColor;
	/// <summary>The color of the brush used for painting, only applies to the paint brush not the decal</summary>
	/// <remarks>Should not be edited in runtime</remarks>
	public Color BrushColor
	{
		get => brushColor;
		set
		{
			brushColor = value;
			particleSystemManager.ChangeParticleSystemColor(value);
		}
	}
	
	// starting brush size
	[SerializeField]
	[Range(0,1)]
	private float brushSize = .25f;
	/// <summary>
	/// <value>Range 0.001 to 1</value>
	/// <remarks>the brush size threshold is currently set at 0.001, that means any values below that will be converted to
	/// 0.001. This is because a game object with size of 0 is buggy in best case scenario</remarks>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">if the value is not between 0-1f</exception>
	public float BrushSize 
	{
		get => brushSize;
		set
		{
			if (value < 0f || value > 1f ) throw new ArgumentOutOfRangeException(nameof(value), "out of bounds, expected range 0-1f");
			
			// if brush size is 0 then it will cause an error
			if (value < .001) value = .001f;
			brushSize = value;
		}
	}

	// starting brush strength
	[SerializeField]
	[Range(0,1)]
	private float brushStrength = 1f;

	/// <summary>
	/// <value>Range 0 to 1</value>
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">if the value is not between 0-1f</exception>
	public float BrushStrength
	{
		get => brushStrength;
		set
		{
			if (value < 0f || value > 1f ) throw new ArgumentOutOfRangeException(nameof(value), "out of bounds, expected range 0-1f");
			brushStrength = value;
		}
	}

	/// <summary>
	/// The local scale of the brush, the default brush scale is too big so we have to scale it down to acceptable size
	/// </summary>
	private const float LocalBrushSize = 0.05f;

	// TODO move this to another script and create "observers" for when the state of the paint gun will change thus notifying them
	#pragma warning disable
	[SerializeField] 
	private PaintGunParticleSystemManager particleSystemManager;
	
	/// <summary>
	/// the exit of the barrel of the paint gun
	/// </summary>
	[SerializeField] 
	private Transform paintGunBarrelExit; 
	[SerializeField]
	private float paintDistance = 30f;

	private bool _painting;

	private bool Painting
	{
		get => _painting;
		set
		{
			if (value)
			{
				particleSystemManager.StartParticleSystem();
			}
			else
			{
				_currentlyPaintedContainer?.ContainerHolder.SetActive(false);
				_currentlyPaintedContainer = null;
				particleSystemManager.StopParticleSystem();
			}
			_painting = value;
		}
	}
	private BrushContainer _currentlyPaintedContainer;
	private XRGrabInteractable _grabbable;
	
	/// <summary>
	/// Painting on the texture on hit.
	/// </summary>
	private void Paint()
	{
		if (HitUVPosition(out var uvHitPos, out var brushContainer))
		{
			if (_currentlyPaintedContainer != brushContainer)
			{
				_currentlyPaintedContainer?.ContainerHolder.SetActive(false);

				_currentlyPaintedContainer = brushContainer;
				_currentlyPaintedContainer.ContainerHolder.SetActive(true);
			}
			var brushObj = Instantiate(BrushEntity, brushContainer.Container.transform, true);
			brushObj.layer = (int)LayersEnum.RenderTexture;
			brushObj.transform.localPosition =uvHitPos; //The position of the brush (in the UVMap)
			brushObj.transform.localScale = Vector3.one * (LocalBrushSize * BrushSize);
			brushObj.GetComponent<SpriteRenderer>().color = new Color(BrushColor.r, BrushColor.g, BrushColor.b, BrushStrength);
		}
	}

	/// <summary>
	/// Determines if the ray hits a paintable surface
	/// </summary>
	/// <param name="uvHitPos">The position where the ray hit the texture, defaults to <c>Vector2.zero</c></param>
	/// <param name="brushContainer">container for the brush strokes on a given gameobject's texture, defaults to <c>null</c></param>
	/// <returns>
	/// True if the ray managed to hit a paintable surface otherwise false
	/// </returns>
	private bool HitUVPosition(
		out Vector2 uvHitPos,
		out BrushContainer brushContainer
	) 
	{
		uvHitPos = Vector2.zero;
		brushContainer = null;
		if (
			Physics.Raycast(paintGunBarrelExit.position, paintGunBarrelExit.forward, out var hit, paintDistance) &&
			hit.transform.gameObject.TryGetComponent(out PaintableSurface paintableSurface) &&
			paintableSurface.isActiveAndEnabled
		    )
		{
			// to center uv
			uvHitPos.x = hit.textureCoord.x - BrushContainerCreator.CameraOrthographicSize;
			uvHitPos.y = hit.textureCoord.y - BrushContainerCreator.CameraOrthographicSize;
			
			brushContainer = paintableSurface.BrushContainer;
			return true;
		}
		return false;
	}
	
	private void Start()
	{
		_grabbable = GetComponent<XRGrabInteractable>();
		_grabbable.activated.AddListener(StartPainting);
		_grabbable.deactivated.AddListener(StopPainting);
		_grabbable.selectExited.AddListener(SelectExited);
		
		particleSystemManager.ChangeParticleSystemColor(BrushColor);
	}

	private void FixedUpdate()
	{
		if (Painting)
		{
			Paint();
		}
	}

	private void StopPainting(DeactivateEventArgs arg0)
	{
		Painting = false;
	}
	
	private void SelectExited(SelectExitEventArgs arg0)
	{
		Painting = false;
	}

	private void StartPainting(ActivateEventArgs arg)
	{
		Painting = true;
	}
}
