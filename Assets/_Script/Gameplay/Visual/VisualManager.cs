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

    public TeamColor playerColor = TeamColor.White;

    public bool IsPromoting = false;
    private bool challengeFinished = false;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.onBoardInit.AddListener(InitBoard);
        GameManager.Instance.onPieceSelected.AddListener(UpdateAccessibleTilesVisual);
        GameManager.Instance.onEnPassant.AddListener(UpdateEnPassant);
        GameManager.Instance.onRook.AddListener(UpdateRook);
        GameManager.Instance.onChallengeWin.AddListener(OnChallengeWin);

        //To remove later on
        GameManager.Instance.onPawnPromotion.AddListener(TEST);
    }
    private void OnChallengeWin()
    {
        Debug.Log("CHALLENGE WON");
        challengeFinished = true;

    }

    private void TEST(TeamColor arg0)
    {
        Debug.Log("ON PAWN PROMOTION TRIGGERED");
    }

    public void PromotePieceTo(Vector2 position, PieceType pieceType, TeamColor color)
    {  
        int index = color == TeamColor.White ? 0 : 6;
            switch (pieceType)
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

        p.MovePieceToTile(position);
        IsPromoting = false;
        GameManager.Instance.PromotePawn(pieceType);
        g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        if(color == playerColor)
            EndChallengePlayerTurn();

    }

    public void EndChallengePlayerTurn() {
        if(IsPromoting) return;
        if(challengeFinished) {
            GameManager.Instance.GetNewChallenge();
            challengeFinished = false;
            return;
        }
        ChessMove move = GameManager.Instance.GetNextChallengeMove();
        TileVisual tile = GetTileVisualAtLocation(move.Start);
        Debug.Log("Start: " + move.Start + " End: " + move.End);
        Debug.Log(" OK: " + tile.CurrentPieceOnTile != null);
        PieceVisual piece = tile.CurrentPieceOnTile;
        piece.MovePieceToTile(move.End);
    }

    private void UpdateRook(Vector2Int initialRookPos, Vector2Int newRookPos)
    {
        TileVisual tile = GetTileVisualAtLocation(initialRookPos);
        PieceVisual piece = tile.CurrentPieceOnTile;
        piece.MovePieceToTile(newRookPos);
    }

    private void UpdateEnPassant(Vector2Int deletedPiecePosition)
    {
        TileVisual tile = GetTileVisualAtLocation(deletedPiecePosition);
        Destroy(tile.CurrentPieceOnTile.gameObject);
        tile.CurrentPieceOnTile=null;
    }

    private void ClearBoard()
    {
        foreach(TileVisual tile in tilesVisual)
        {
            if(tile.CurrentPieceOnTile != null)
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

            p.MovePieceToTile(new Vector2(boardSquare.position.x, boardSquare.position.y));
            g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        playerColor = GameManager.Instance.GetPlayerColor();
    }

    public void CleanAccessibleTileVisual()
    {
        foreach (var tile in illuminatedTile)
        {
            tile.ToggleHighlightVisual(false);
        }
        illuminatedTile.Clear();
    }

    private void UpdateAccessibleTilesVisual(List<Vector2> accessibleTiles)
    {
        CleanAccessibleTileVisual();

        //toggle and shift new illuminated tiles
        foreach (Vector2 v in accessibleTiles)
        {
            TileVisual tile = tilesVisual[(int)(v.y * 8 + v.x)];
            tile.ToggleHighlightVisual(true);
            illuminatedTile.Add(tile);
        }
    }

    public TileVisual GetTileVisualAtLocation(Vector2 location)
    {
        if(location.x == 0 || location.y == 0) return null;
        return tilesVisual[(int)((location.y-1) * 8 + (location.x-1))];
    }

}
