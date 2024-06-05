using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisual : MonoBehaviour
{
    [SerializeField]
    public GameObject visual;
    public Color possibleMoveTileColor = Color.yellow;
    public Color selectedTileColor = Color.green;

    public Vector2 position;

    [Header("Algo de placement scene en mode editor")]

    public Vector3 distanceBetweenTiles;

    public PieceVisual CurrentPieceOnTile;

    public bool IsIlluminated {  get; private set; }

    // Testing line: enable movement on whole board
    private void Start()
    {
        ToggleHighlightVisual(true);
    }

    public void ToggleHighlightVisual(bool On)
    {
        if (visual != null)
        {
            if (On == false)
            {
                visual.GetComponent<Renderer>().material.color = possibleMoveTileColor;
            }

            visual.SetActive(On);
            IsIlluminated = On;
        }
    }

    public void ChangeLightVisual(bool isSelected)
    {
        if(isSelected)
        {
            visual.GetComponent<Renderer>().material.color = selectedTileColor;
        }else
        {
            visual.GetComponent<Renderer>().material.color = possibleMoveTileColor;
        }
    }

    /*Code below not relevant: speed up implementation and test process*/

    [ContextMenu("PlaceOnMap")]
    public void PlaceTileOnMap()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject g = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
                g.transform.localPosition = transform.localPosition + new Vector3(distanceBetweenTiles.x * j, distanceBetweenTiles.y * i, 0);
                TileVisual tileVisual = g.GetComponent<TileVisual>();
                tileVisual.position = new Vector2(j, i);
                g.name = "Tile_X" + j + "_Y" + i;
            }
        }
    }

    [ContextMenu("ToggleLight")]
    public void TestToggleLightOn()
    {
        ToggleHighlightVisual(true);
    }
}
