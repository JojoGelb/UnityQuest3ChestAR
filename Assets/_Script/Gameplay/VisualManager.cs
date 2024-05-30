using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : Singleton<VisualManager>
{
    public List<float/*Visuel des Cases*/> tilesVisual = new List<float>();
    public List<float/*Visuel des Cases*/> illuminatedTile = new List<float>();

    public List<float /*Visuel des pieces*/> piecesVisual = new List<float>();

    private void Start()
    {
        GameManager.Instance.OnPieceSelected.AddListener(UpdateAccessibleTilesVisual);

        //function to sort tilesVisual
    }

    private void UpdateAccessibleTilesVisual(List<Vector2> accessibleTiles)
    {
        //clear illuminated tiles

        //toggle and shift new illuminated tiles
    }
}
