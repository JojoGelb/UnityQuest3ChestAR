using System.Collections.Generic;
using UnityEngine;

public class LogicManager
{
    private readonly Board _board = new();
    private readonly BitBoard _currentBitBoard = new();
    private Vector2 _currentPiece;
    
    public Board GetBoard() { return _board; }
    public BitBoard GetCurrentBitBoard() { return _currentBitBoard; }
    public Vector2 GetCurrentPiece() { return _currentPiece; }

    public List<Vector2> SelectPiece(Vector2 position)
    {
        _currentPiece = position;
        //TODO Manage Valid Moves
        return _currentBitBoard.GetValidMoves();
    }
    
    public MoveState MoveTo(int x, int y)
    {
        var newPosition = new Vector2(x, y);
        
        if (_currentBitBoard.Get(newPosition))
        {
            return _board.Move(_currentPiece, newPosition) ? MoveState.Eaten : MoveState.Success;
        }

        return MoveState.Failed;
    }


   
}
