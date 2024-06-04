using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : Singleton<VisualManager>
{
    public List<TileVisual> tilesVisual = new List<TileVisual>();
    public List<TileVisual> illuminatedTile = new List<TileVisual>();

    public List<PieceVisual> piecesVisual = new List<PieceVisual>();

    private void Start()
    {
        GameManager.Instance.onPieceSelected.AddListener(UpdateAccessibleTilesVisual);
    }

    private void UpdateAccessibleTilesVisual(List<Vector2> accessibleTiles)
    {
        //clear illuminated tiles
        foreach (var tile in illuminatedTile)
        {
            tile.ToggleHighlightVisual(false);
        }
        illuminatedTile.Clear();

        //toggle and shift new illuminated tiles
        foreach(Vector2 v in accessibleTiles)
        {
            TileVisual tile = tilesVisual[(int)(v.y * 8 + v.x)];
            tile.ToggleHighlightVisual(true);
            illuminatedTile.Add(tile);
        }
    }

    public void MovePieceToTile(Transform piece, Vector2 destination)
    {
        Debug.Log((int)(destination.y * 8 + destination.x));
        TileVisual t = tilesVisual[(int)(destination.y * 8 + destination.x)];
        piece.transform.parent = t.transform;
        piece.transform.localPosition = Vector3.zero;
    }
}
