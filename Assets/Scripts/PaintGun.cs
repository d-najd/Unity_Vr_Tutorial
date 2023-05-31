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
		Vector3 uvWorldPosition = Vector3.zero;
		if (HitUVPosition(ref uvWorldPosition))
		{
			//brushPaint.a=brushSize*2.0f; // Brushes have alpha to have a merging effect when painted over.

			brushPaint.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
			brushPaint.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
			brushPaint.transform.localScale = Vector3.one * 10f; //The size of the brush
		}

		SaveTexture();
	}

	void SaveTexture()
	{
		RenderTexture.active = canvasTexture;
		Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;
		baseMaterial.mainTexture = tex; //Put the painted texture as the base
		foreach (Transform child in brushContainer.transform)
		{
			//Clear brushes
			Destroy(child.gameObject);
		}

		StartCoroutine(SaveTextureToFile(tex));
	}


	private bool HitUVPosition(ref Vector3 uvWorldPosition)
	{
		if (!Physics.Raycast(paintExitPoint.position, paintExitPoint.forward, out var hit, paintDistance) &&
		    hit.collider is not MeshCollider)
			return false;

		uvWorldPosition.x = hit.textureCoord.x;
		uvWorldPosition.y = hit.textureCoord.y;
		uvWorldPosition.z = 0;
		// Debug.Log($"Ray Hitting {hit.transform.name} At Position: {hit.transform.position}, and uvWorldPos {uvWorldPosition}");
		return true;
	}

	IEnumerator SaveTextureToFile(Texture2D savedTexture)
	{
		string fullPath = System.IO.Directory.GetCurrentDirectory() + "\\UserCanvas\\";
		System.DateTime date = System.DateTime.Now;
		string fileName = "CanvasTexture.png";
		if (!System.IO.Directory.Exists(fullPath))
			System.IO.Directory.CreateDirectory(fullPath);
		var bytes = savedTexture.EncodeToPNG();
		System.IO.File.WriteAllBytes(fullPath + fileName, bytes);
		Debug.Log("<color=orange>Saved Successfully!</color>" + fullPath + fileName);
		yield return null;
	}


}
