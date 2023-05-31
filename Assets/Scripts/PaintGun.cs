using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintGun : MonoBehaviour
{
    [SerializeField] private GameObject brushContainer; // Container for the decals and brushes before they are saved to the texture
	[SerializeField] private RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
	[SerializeField] private Material baseMaterial; // The material of our base texture (Were we will save the painted texture)
    
    [SerializeField] private Transform paintExitPoint; // The exit of the barrel
    [SerializeField] private float paintDistance = 30f; // Paint distance (ray)
    
    private void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Paint);
    }

    private void Paint(ActivateEventArgs arg)
    {
		Vector3 uvWorldPosition=Vector3.zero;		
        if(HitUVPosition(ref uvWorldPosition))
        {
            
        }
    }
    
    private bool HitUVPosition(ref Vector3 uvWorldPosition)
    {
        if (!Physics.Raycast(paintExitPoint.position, paintExitPoint.forward, out var hit, paintDistance))
            return false;
        
        uvWorldPosition.x = hit.textureCoord.x;
        uvWorldPosition.y = hit.textureCoord.y;
        uvWorldPosition.z = 0;
        Debug.Log($"Ray Hitting {hit.transform.name} At Position: {hit.transform.position}, and uvWorldPos {uvWorldPosition}");
        return true;
    }

}
