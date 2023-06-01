using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintGun : MonoBehaviour
{
	[SerializeField]
	private GameObject brushContainer; // Container for the decals and brushes before they are saved to the texture

	[SerializeField]
	private RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes

	[SerializeField]
	private Material baseMaterial; // The material of our base texture (Were we will save the painted texture)

	[SerializeField] private GameObject brushPaint; // The paint that we want to use

	[SerializeField] private Transform paintExitPoint; // The exit of the barrel
	[SerializeField] private float paintDistance = 30f; // Paint distance (ray)

	private void Start()
	{
		XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
		grabbable.activated.AddListener(Paint);
	}

	private void Paint(ActivateEventArgs arg)
	{
		if (HitUVPosition(out var textureHitX, out var textureHitY, out var raycastHit))
		{
			//brushPaint.a=brushSize*2.0f; // Brushes have alpha to have a merging effect when painted over.
			
			// var brushPaintInstance = Instantiate(brushPaint, brushContainer.transform, true); //Add the brush to our container to be wiped later
			// brushPaintInstance.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
			// brushPaintInstance.transform.localScale = Vector3.one * 10f; //The size of the brush
			
			var spriteRenderer = brushPaint.GetComponent<SpriteRenderer>();
			var tex = baseMaterial.mainTexture as Texture2D;
			//var tex = DuplicateTexture(baseMaterial.mainTexture as Texture2D);
			tex.SetPixels(textureHitX, textureHitY, spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height, spriteRenderer.sprite.texture.GetPixels());
			tex.Apply();
			// StartCoroutine(SaveTextureToFile(tex));
			// var spriteRenderer = brushPaintInstance.GetComponent<SpriteRenderer>();
			// var tex = new Texture2D(baseMaterial.mainTexture.width, baseMaterial.mainTexture.height);
			// spriteRenderer.sprite.texture.GetPixels()
			// SaveTexture();
		}

	}

	/**
	 * return
	 */
	private Texture2D DuplicateTexture(Texture2D originalTexture)
	{
		var copyTexture = new Texture2D(originalTexture.width, originalTexture.height);
		copyTexture.SetPixels(originalTexture.GetPixels());
		copyTexture.Apply();
		
		return copyTexture;
	}

	private void SaveTexture()
	{
		
		
		/*
		RenderTexture.active = canvasTexture;
		Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
		for (var x = 0; x < 100; x++)
		{
			for (var y = 0; y < 100; y++)
			{
				tex.SetPixel(x, y, Color.green);
			}
		}
		
		tex.Apply();
		RenderTexture.active = null;


		baseMaterial.mainTexture = tex; //Put the painted texture as the base
		foreach (Transform child in brushContainer.transform)
		{
			//Clear brushes
			// Destroy(child.gameObject);
		}
		*/

		//StartCoroutine(SaveTextureToFile(tex));
	}

	private bool HitUVPosition(out int textureHitX, out int textureHitY, out RaycastHit raycastHit)
	{
		textureHitX = 0;
		textureHitY = 0;
		raycastHit = new RaycastHit();
		
		if (!Physics.Raycast(paintExitPoint.position, paintExitPoint.forward, out var hit, paintDistance) &&
		    hit.collider is not MeshCollider)
			return false;
		
		//var originalTexture = hit.transform.GetComponent<MeshRenderer>().material.mainTexture;
		var originalTexture = baseMaterial.mainTexture;
		var textureCoord = hit.textureCoord;
		
		raycastHit = hit;
		textureHitX = (int) (textureCoord.x * originalTexture.width);
		textureHitY = (int) (textureCoord.y * originalTexture.height);
		Debug.Log($"Ray Hitting {hit.transform.name} At Position: {hit.transform.position}, and uvWorldPos {textureHitX} {textureHitY}");
		return true;
	}

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
}
