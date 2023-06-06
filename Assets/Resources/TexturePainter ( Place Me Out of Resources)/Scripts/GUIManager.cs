using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GUIManager : MonoBehaviour {
	public Text guiTextMode;
	public Slider sizeSlider;
	public TexturePainterExternalAsset painterExternalAsset;

	public void SetBrushMode(int newMode){
		Painter_BrushMode brushMode =newMode==0? Painter_BrushMode.DECAL:Painter_BrushMode.PAINT; //Cant set enums for buttons :(
		string colorText=brushMode==Painter_BrushMode.PAINT?"orange":"purple";	
		guiTextMode.text="<b>Mode:</b><color="+colorText+">"+brushMode.ToString()+"</color>";
		painterExternalAsset.SetBrushMode (brushMode);
	}
	public void UpdateSizeSlider(){
		painterExternalAsset.SetBrushSize (sizeSlider.value);
	}
}
