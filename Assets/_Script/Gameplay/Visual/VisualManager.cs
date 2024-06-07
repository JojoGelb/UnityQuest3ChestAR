using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualManager : Singleton<VisualManager>
{
    [SerializeField] private List<TileVisual> tilesVisual = new List<TileVisual>();

    // i < 6 -> white
    // i > 5 -> black
    // 0 king
    // 1 queen
    // 2 rook
    // 3 knight
    // 4 bishop
    // 5 pawn
    [SerializeField] private List<GameObject> piecesPrefab = new List<GameObject>();
    [SerializeField] private Transform parentTransformPieceInstantiate;

    private List<TileVisual> illuminatedTile = new List<TileVisual>();

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.onBoardInit.AddListener(InitBoard);
        GameManager.Instance.onPieceSelected.AddListener(UpdateAccessibleTilesVisual);
    }

    private void ClearBoard()
    {
        foreach(TileVisual tile in tilesVisual)
        {
            Destroy(tile.CurrentPieceOnTile.gameObject);
            tile.ToggleHighlightVisual(false);
        }
    }

    private void InitBoard(BoardLayout.BoardSquareSetup[] board)
    {

        ClearBoard();

        foreach (BoardLayout.BoardSquareSetup boardSquare in board)
        {
            int index = boardSquare.TeamColor == TeamColor.White ? 0 : 6;
            switch (boardSquare.pieceType)
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
            new Vector2(boardSquare.position.x - 1, boardSquare.position.y - 1);

            p.MovePieceToTile(new Vector2(boardSquare.position.x, boardSquare.position.y));
        }
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

    public TileVisual GetTileVisualAtLocation(Vector2 location)
    {
        return tilesVisual[(int)(location.y * 8 + location.x)];
    }

}
