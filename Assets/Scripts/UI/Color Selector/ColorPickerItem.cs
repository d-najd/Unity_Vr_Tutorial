using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <remarks>Every GameObject that contains this script must also include <c>RawImage</c></remarks>
// TODO this could be replaced with a on click action which accepts image
public class ColorPickerItem : MonoBehaviour
{
    [SerializeField] private PaintSelectorMenuManager paintSelectorMenuManager;

    public void ChangeColor()
    {
        paintSelectorMenuManager.ChangeColor(GetComponent<RawImage>().color);
    }
}
