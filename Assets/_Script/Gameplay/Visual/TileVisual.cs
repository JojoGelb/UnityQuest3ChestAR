using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisual : MonoBehaviour
{
    [SerializeField]
    public GameObject visual;

    public Vector2 position;

    [Header("Algo de placement scene en mode editor")]

    public Vector3 distanceBetweenTiles;

    public bool IsIlluminated {  get; private set; }

    [ContextMenu("PlaceOnMap")]
    public void PlaceTileOnMap()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                GameObject g = Instantiate(gameObject,transform.position, Quaternion.identity,transform.parent);
                g.transform.localPosition = transform.localPosition + new Vector3(distanceBetweenTiles.x * i, distanceBetweenTiles.y * j,0);
                TileVisual tileVisual = g.GetComponent<TileVisual>();
                tileVisual.position = new Vector2(j, i);
                g.name = "Tile_X" + i + "_Y" + j;
            }
        }
    }

    public void ToggleHighlightVisual(bool On)
    {
        if (visual != null)
        {
            visual.SetActive(On);
            IsIlluminated = On;
        }
    }

    [ContextMenu("ToggleLight")]
    public void TestToggleLightOn()
    {
        ToggleHighlightVisual(true);
    }
}
