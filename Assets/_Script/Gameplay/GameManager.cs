using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum MoveState
{
    Failed, Success, Eaten
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private VisualManager visualManager;
    private readonly LogicManager _logicManager = new();
    
    public UnityEvent<List<Vector2>> onPieceSelected = new ();
    public UnityEvent<BoardLayout.BoardSquareSetup[]> onBoardInit = new ();
    public UnityEvent<TeamColor> onPawnPromotion = new();
    
    public UnityEvent<Vector2Int> onEnPassant = new(); //Pawn to delete
    public UnityEvent<Vector2Int, Vector2Int> onRook = new(); //Old Rook Pos, New Rook Pos
    
    public UnityEvent onChallengeBegin = new();
    public UnityEvent onChallengeWin = new();
    
    //Temporary
    [SerializeField] private BoardLayout boardLayoutFromInspector;


    private void Start()
    {
        //Initiate logicBoard
        _logicManager.InitBoard(boardLayoutFromInspector);
        
        //Call event to notify visualManager
        onBoardInit.Invoke(_logicManager.GetBoardSquareSetup());
    }

    public void SelectPiece(Vector2 position)
    {
        Debug.Log("MANAGER: Piece selected by visual = " + position);
        //call to logic to get available position on board
        var pieces = _logicManager.SelectPiece(position);
        Debug.Log("MANAGER: NumTiles returned by logic = " + pieces.Count);

        //Call event to notify visualManager
        onPieceSelected.Invoke(pieces);
    }
    
    public MoveState MoveTo(int x, int y)
    {
        
        //call to logic to move piece on board
        return _logicManager.MoveTo(x, y, onPawnPromotion);
        
        //Update visual according to returned enum
    }

    public void PromotePawn(PieceType newType)
    {
        _logicManager.PromotePawn(newType);
    }

    public void GetNewChallenge()
    {
        StartCoroutine(_logicManager.GetNewChessChallenge(onBoardInit, onChallengeBegin));
    }

    public ChessMove GetNextChallengeMove()
    {
        return _logicManager.GetNextChallengeMove();
    }

    public MoveState CheckChallengeMove(int x, int y)
    {
        var state = _logicManager.CheckChallengeMove(x, y, onPawnPromotion);
        if(state != MoveState.Failed && _logicManager.IsChallengeFinish()) onChallengeWin.Invoke();
        return state;
    }

    public TeamColor GetPlayerColor()
    {
        return _logicManager.GetPlayerColor() ? TeamColor.White : TeamColor.Black;
    }
    
    
}
