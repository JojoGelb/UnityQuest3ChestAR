using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class TestChessGameController : MonoBehaviour
{
    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private VisualBoard board;

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
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);

            CreatePiecesAndInitialize(squareCoords, team, type);
        }
    }

    private void CreatePiecesAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        VisualPiece newPiece = piecesCreator.CreatePiece(type).GetComponent<VisualPiece>();
        newPiece.SetData(squareCoords, team, board);
        
        Material teamMaterial = piecesCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);
    }
}
