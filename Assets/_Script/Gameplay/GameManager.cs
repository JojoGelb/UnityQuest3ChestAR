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

    //private LogicManager logicManager;


    //TO IMPLEMENT IN LOGIC ----------------
    private readonly Board _board = new();
    private readonly BitBoard _currentBitBoard = new();
    private Vector2 _currentPiece;
    // --------------------------------
    
    public UnityEvent<List<Vector2>> onPieceSelected = new ();

    private void Start()
    {
        //logicManager = new LogicManager();
    }

    public void SelectPiece(Vector2 position)
    {
        _currentPiece = position;

        //call to logic to get available position on board => logicManager.GetValidMoves(positionSelectedPiece);
        List<Vector2> pieces = _currentBitBoard.GetValidMoves();

        //Call event to notify visualManager
        onPieceSelected.Invoke(pieces);
    }
    
    public MoveState MoveTo(int x, int y)
    {
        var newPosition = new Vector2(x, y);
        //call to logic to move piece on board => return logicManager.MoveTo(newPosition);
        //Update visual according to returned enum

        if (_currentBitBoard.Get(newPosition))
        {
            return _board.Move(_currentPiece, newPosition) ? MoveState.Eaten : MoveState.Success;
        }

        return MoveState.Failed;
    }


    //TO IMPLEMENT IN LOGIC ----------------
    public Board GetBoard() { return _board; }
    public BitBoard GetCurrentBitBoard() { return _currentBitBoard; }
    public Vector2 GetCurrentPiece() { return _currentPiece; }
    // --------------------------------
}
