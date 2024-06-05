using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualManager : Singleton<VisualManager>
{
    public List<TileVisual> tilesVisual = new List<TileVisual>();

    // i < 6 -> white
    // i > 5 -> black
    // 0 king
    // 1 queen
    // 2 rook
    // 3 knight
    // 4 bishop
    // 5 pawn
    public List<GameObject> piecesPrefab = new List<GameObject>();
    public Transform parentTransformPieceInstantiate;
    
    public BoardLayout boardLayout;

    private List<TileVisual> illuminatedTile = new List<TileVisual>();

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

    public void MovePieceToTile(PieceVisual piece, Vector2 destination)
    {
        TileVisual oldTile = tilesVisual[(int)(piece.position.y * 8 + piece.position.x)];
        TileVisual newTile = tilesVisual[(int)(destination.y * 8 + destination.x)];
        piece.transform.parent = newTile.transform;
        piece.position = destination;
        piece.transform.localPosition = Vector3.zero;

        if(newTile.CurrentPieceOnTile != null)
        {
            Destroy(newTile.CurrentPieceOnTile.gameObject);
        }

        newTile.CurrentPieceOnTile = piece;
        oldTile.CurrentPieceOnTile = null;

    }

    [ContextMenu("Place Pieces on board")]
    public void PlacePieceOnBoard()
    {
        foreach(BoardLayout.BoardSquareSetup boardSquare in boardLayout.BoardSquares)
        {
            int index = boardSquare.TeamColor == TeamColor.White ? 0 : 6;
            switch(boardSquare.pieceType)
            {
                case PieceType.King:
                    break;
                case PieceType.Queen:
                    index += 1;
                    break;
                case PieceType.Rook:
                    index += 2;
                    break;
                case PieceType.Knight:
                    index += 3;
                    break;
                case PieceType.Bishop:
                    index += 4;
                    break;
                case PieceType.Pawn:
                    index += 5;
                    break;
            }
            GameObject g = Instantiate(piecesPrefab[index], Vector3.zero, Quaternion.identity, parentTransformPieceInstantiate);
            PieceVisual p = g.GetComponent<PieceVisual>();
            p.position = new Vector2(boardSquare.position.x - 1, boardSquare.position.y - 1);
            MovePieceToTile(p, p.position);
        }
    }
}
