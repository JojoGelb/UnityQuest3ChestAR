using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class TestChessGameController : MonoBehaviour
{
    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;

    private PieceCreator piecesCreator;
   

    private void Awake()
    {
        SetDependencies();
    }

    private void SetDependencies()
    {
        piecesCreator = GetComponent<PieceCreator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartNewGame();
    }

    private void StartNewGame() 
    {
        CreatePiecesFromLayout(startingBoardLayout);
    }

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++) 
        {
            Vector2Int squareCoords = layout.GetSquaresCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            Type type = layout.GetSquarePieceNameAtIndex(i);

            CreatePiecesAndInitialize(squareCoords, team, type);
        }
    }

    private void CreatePiecesAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = piecesCreator.CreatePiece(type).GetComponent<Piece>();

        // TODO set piece data
    }
}
