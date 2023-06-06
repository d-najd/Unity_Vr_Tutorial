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
		set => brushEntity = value;
	}

	/// <summary>The decal that will be used for painting</summary>
	/// <remarks>Should not be edited in runtime</remarks>
	[SerializeField] private GameObject decalEntity;
	public GameObject DecalEntity
	{
		get => decalEntity;
		set => decalEntity = value;
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
	
	private float _brushSize = 1f;
	/// <summary>
	/// <value>Range 0.001 to 1</value>
	/// <remarks>the brush size threshold is currently set at 0.001, that means any values below that will be converted to
	/// 0.001. This is because a game object with size of 0 is buggy in best case scenario</remarks>
	/// </summary>
	public float BrushSize 
	{
		get => _brushSize;
		set
		{
			// if brush size is 0 then it will cause an error
			if (value < .001) value = .001f;
			_brushSize = value;
		}
	}

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
	private XRGrabInteractable _grabbable;
	
	/// <summary>
	/// Painting on the texture on hit.
	/// </summary>
	/// <see cref="BrushColor"/> For changing the color of the brush
	private void Paint()
	{
		if (HitUVPosition(out var uvHitPos, out var brushContainer))
		{
			var brushObj = Instantiate(BrushEntity, brushContainer.Container.transform, true);
			brushObj.transform.localPosition=uvHitPos; //The position of the brush (in the UVMap)
			// brushObj.transform.localScale=Vector3.one*BrushSize;//The size of the brush
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
			uvHitPos = hit.textureCoord;
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
		if (_painting)
		{
			Paint();
		}
	}

	private void StopPainting(DeactivateEventArgs arg0)
	{
		_painting = false;
		particleSystemManager.StopParticleSystem();
	}
	
	private void SelectExited(SelectExitEventArgs arg0)
	{
		_painting = false;
		particleSystemManager.StopParticleSystem();
	}

	private void StartPainting(ActivateEventArgs arg)
	{
		_painting = true;
		particleSystemManager.StartParticleSystem();
	}
}
