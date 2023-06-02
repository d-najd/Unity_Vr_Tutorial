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
	private Texture2D brushColor;

	/// <summary>
	/// the exit of the barrel of the paint gun
	/// </summary>
	[SerializeField] 
	private Transform paintGunBarrelExit; 
	[SerializeField]
	private float paintDistance = 30f;

	private bool _painting;
	private XRGrabInteractable _grabbable;

	private void Paint()
	{
		if (HitUVPosition(out var textureHitX, out var textureHitY, out var hitTexture) && hitTexture != null)
		{
			hitTexture.SetPixels(textureHitX, textureHitY, brushPaint.width, brushPaint.height, brushPaint.GetPixels());
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
		    hit.transform.gameObject.TryGetComponent(out Renderer renderer))
		{
			var textureHitCords = hit.textureCoord;

			hitTexture = renderer.material.mainTexture as Texture2D;
			textureHitX = (int)(textureHitCords.x * renderer.material.mainTexture.width);
			textureHitY = (int)(textureHitCords.y * renderer.material.mainTexture.height);
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
	
	private void Start()
	{
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
