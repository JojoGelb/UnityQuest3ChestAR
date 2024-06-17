using System;
using System.Collections;
using System.Collections.Generic;
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
        StartCoroutine(_logicManager.GetNewChessChallenge(onBoardInit));
    }
}
