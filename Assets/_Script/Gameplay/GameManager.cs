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
    private readonly Board _board = new();
    private readonly BitBoard _currentBitBoard = new();
    private Vector2 _currentPiece;
    
    public UnityEvent<List<Vector2>> onPieceSelected = new ();

    public void InstantiateBoard()
    {
        //Instantiate logic here: use string to tell the 
    }

    public void SelectPiece(int x, int y)
    {
        _currentPiece = new Vector2(x, y);
        
        //call to logic to get available position on board
        
        onPieceSelected.Invoke(_currentBitBoard.GetValidMoves());
    }
    
    public MoveState MoveTo(int x, int y)
    {
        var newPosition = new Vector2(x, y);
        
        if(_currentBitBoard.Get(newPosition))
        {
            return _board.Move(_currentPiece, newPosition) ? MoveState.Eaten : MoveState.Success;
        }

        return MoveState.Failed;
    }

    public Board GetBoard() { return _board; }
    public BitBoard GetCurrentBitBoard() { return _currentBitBoard; }
    public Vector2 GetCurrentPiece() { return _currentPiece; }
}
