using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO refactor pretty much the whole script
public class PaintStatsMenuManager : MonoBehaviour
{
    [SerializeField] private int selectedIndex = 0;
    [SerializeField] private GameObject[] paintableGameObjects;
    [SerializeField] private Material[] startingMaterials;
    /// <summary>
    /// The texture that we want the user to paint
    /// </summary>
    /// <see cref="startingMaterials"/>
    [SerializeField] private Texture2D[] targetTextures;

    /// <summary>
    /// Black and white texture, where white represents places that are not paintable in the texture and white places
    /// that are paintable. this data is used to calculate teh coverage and progress.
    /// </summary>
    [SerializeField] private Texture2D[] coverageMaps;
    
    /// <summary>
    /// The max error of a single color value (rgb) for it to be considered an wrongly painted pixel
    /// </summary>
    [Header("Paint Error")] 
    [SerializeField] 
    private float maxSingleColorValueError = .1f;

    /// <summary>
    /// The max error of a all color values (rgb) for it to be considered an wrongly painted pixel
    /// </summary>
    [SerializeField] private float maxCombinedColorValueError = .25f;
    
    /// <summary>
    /// Used to determine whether the menu is active and if it is to update it
    /// </summary>
    [Header("Display Setup")] [SerializeField] private GameObject menu;
    [SerializeField] private TextMeshProUGUI currentlyPainting;
    [SerializeField] private TextMeshProUGUI paintProgress;
    [SerializeField] private TextMeshProUGUI paintCoverage;
    
    [SerializeField] private InputActionProperty showButton;


    private readonly Dictionary<int, PaintableDataHolder> _paintableDataHolders = new Dictionary<int, PaintableDataHolder>();
    
    // TODO add validity checks for texture sizes (if they match)
    private void Start()
    {
        if (
            paintableGameObjects.Length != startingMaterials.Length ||
            startingMaterials.Length != targetTextures.Length ||
            targetTextures.Length != coverageMaps.Length
            )
        {
            throw new InvalidOperationException(
                "Um having more of paintable GameObjects, starting materials or target textures than the rest seems dumb and will probably crush the game");
        }
    }

    private void FixedUpdate()
    {
        if (showButton.action.WasPressedThisFrame())
            menu.SetActive(!menu.activeSelf);

        // a lot of caching can be done here
        if (menu.activeSelf)
        {
            var paintableDataHolder = PaintableDataHolderInstance();
            // updating the current object that is being painted
            currentlyPainting.text = $"Painting: {paintableDataHolder.CurName}";

            // updating coverage
            var curTexturePixels = paintableDataHolder.CurTexturePixels;
            var paintedPixelsCount = 0;
            foreach (var p in paintableDataHolder.CoverageMapPixels)
            {
                if (paintableDataHolder.StartingTexturePixels[p] != curTexturePixels[p])
                {
                    paintedPixelsCount++;
                }
            }
            paintCoverage.text = $"Paint Coverage: {Math.Round((100f / paintableDataHolder.PaintablePixelsCount) * paintedPixelsCount)}%";
        }
    }

    /// <summary>
    /// Gets coverage map pixels for <c>selectedIndex</c>
    /// </summary>
    /// <returns>Coverage map pixels for <c>selectedIndex</c></returns>
    /// TODO this method NEEDS refactoring
    private PaintableDataHolder PaintableDataHolderInstance()
    {
        if (_paintableDataHolders.TryGetValue(selectedIndex, out var paintableObjectDataHolder))
            return paintableObjectDataHolder;

        var coverageMap = coverageMaps[selectedIndex];
        var coverageMapPixels = coverageMap.GetPixels();
        var paintablePixelsCount = 0;
        var paintablePixelsPositions = new HashSet<int>();
        
        for (var p = 0; p < coverageMapPixels.Length; p++)
        {
            if (coverageMapPixels[p] != Color.black)
            {
                paintablePixelsCount++;
                paintablePixelsPositions.Add(p);
            }
        }

        var startingTexture = startingMaterials[selectedIndex].mainTexture as Texture2D;
        
        var curMaterial = paintableGameObjects[selectedIndex].GetComponent<Renderer>().material;
        var curPaintableSurface = paintableGameObjects[selectedIndex].GetComponent<PaintableSurface>();
        var curName = curPaintableSurface.SurfaceName;

        var paintablePixelsObjectDataHolder = new PaintableDataHolder(
            paintablePixelsPositions,
            startingTexture!.GetPixels(),
            paintablePixelsCount,
            curMaterial,
            curPaintableSurface,
            curName
        );

        _paintableDataHolders.Add(
            selectedIndex,
            paintablePixelsObjectDataHolder);
        
        return paintablePixelsObjectDataHolder;
    }
    
    private class PaintableDataHolder
    {
        public HashSet<int> CoverageMapPixels { get; }
        public Color[] StartingTexturePixels { get; }
        public int PaintablePixelsCount { get; }

        private Material CurMaterial { get; }

        public Color[] CurTexturePixels => (CurMaterial.mainTexture as Texture2D)!.GetPixels();

        public PaintableSurface CurPaintableSurface { get; }
        public string CurName { get; }

        public PaintableDataHolder(
            HashSet<int> coverageMapPixels, 
            Color[] startingTexturePixels, 
            int paintablePixelsCount,
            Material curMaterial,
            PaintableSurface curPaintableSurface,
            string curName
            )
        {
            this.CoverageMapPixels = coverageMapPixels;
            this.StartingTexturePixels = startingTexturePixels;
            this.PaintablePixelsCount = paintablePixelsCount;
            this.CurMaterial = curMaterial;
            this.CurPaintableSurface = curPaintableSurface;
            this.CurName = curName;
        }
    }
}