using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <remarks>Every GameObject that contains this script must also include <c>RawImage</c> with texture</remarks>
public class BrushItem : MonoBehaviour
{
    [SerializeField] private PaintSelectorMenuManager paintSelectorMenuManager;

    public void ChangeBrush()
    {
        paintSelectorMenuManager.ChangeBrushPaint((GetComponent<RawImage>().mainTexture as Texture2D)!);
    }
}
