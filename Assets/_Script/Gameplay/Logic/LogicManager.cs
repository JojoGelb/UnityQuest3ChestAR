using System.Collections.Generic;
using UnityEngine;

public enum CheckMoveState
{
    EmptyPosition, AllyPiece, EnemyPiece, ExitBoard
}

public class LogicManager
{
    private readonly Board _board = new();
    private readonly BitBoard _currentBitBoard = new();
    private Vector2 _currentPiece;
    
    private BoardLayout _boardLayout;
    
    public Board GetBoard() { return _board; }
    public BitBoard GetCurrentBitBoard() { return _currentBitBoard; }
    public Vector2 GetCurrentPiece() { return _currentPiece; }

    public void InitBoard(BoardLayout boardLayout)
    {
        _boardLayout = boardLayout;
        _board.Clear();
        foreach (var boardSquare in _boardLayout.BoardSquares)
        {
            _board.Set(
                boardSquare.position - new Vector2(1, 1), 
                new Piece(boardSquare.pieceType, boardSquare.TeamColor == TeamColor.White)
            );
        }
    }

    public BoardLayout.BoardSquareSetup[] GetBoardSquareSetup()
    {
        var boardSquares = new List<BoardLayout.BoardSquareSetup>();

        for (var x = 0; x < Board.Size; x++)
        {
            for (var y = 0; y < Board.Size; y++)
            {
                var piece = _board.Get(x, y);
                var square = new BoardLayout.BoardSquareSetup
                {
                    position = new Vector2Int(x + 1, y + 1),
                    pieceType = piece.Type,
                    TeamColor = piece.Color ? TeamColor.White : TeamColor.Black
                };

                boardSquares.Add(square);
            }
        }

        return boardSquares.ToArray();
    }

    public List<Vector2> SelectPiece(Vector2 position)
    {
        _currentPiece = position - new Vector2(1, 1);
        ManageValidMoves(); //Update BitBoard
        
        return _currentBitBoard.GetValidMoves();
    }
    
    public MoveState MoveTo(int x, int y)
    {
        var newPosition = new Vector2(x - 1, y - 1);
        
        if (_currentBitBoard.Get(newPosition))
        {
            return _board.Move(_currentPiece, newPosition) ? MoveState.Eaten : MoveState.Success;
        }

        return MoveState.Failed;
    }

    private bool ManageValidMoves()
    {
        var piece = _board.Get(_currentPiece);
        if (piece.Type == PieceType.None) return false;

        switch (piece.Type)
        {
            case PieceType.Pawn:
                BitBoardCast(
                   (piece.Color) ? Vector2.up : Vector2.down, 
                   _currentPiece, 1
                );
                break;
            
            case PieceType.Bishop:
                BitBoardCast(new Vector2(1, 1), _currentPiece);
                BitBoardCast(new Vector2(-1, 1), _currentPiece);
                BitBoardCast(new Vector2(1, -1), _currentPiece);
                BitBoardCast(new Vector2(-1, -1), _currentPiece);
                break;
            
            case PieceType.Knight:
                break; //TODO Check One Case
            
            case PieceType.Rook:
                BitBoardCast(Vector2.up , _currentPiece);
                BitBoardCast(Vector2.down, _currentPiece);
                BitBoardCast(Vector2.right, _currentPiece);
                BitBoardCast(Vector2.left, _currentPiece);
                break;
            
            case PieceType.Queen:
                BitBoardCast(Vector2.up , _currentPiece);
                BitBoardCast(Vector2.down, _currentPiece);
                BitBoardCast(Vector2.right, _currentPiece);
                BitBoardCast(Vector2.left, _currentPiece);
                BitBoardCast(new Vector2(1, 1), _currentPiece);
                BitBoardCast(new Vector2(-1, 1), _currentPiece);
                BitBoardCast(new Vector2(1, -1), _currentPiece);
                BitBoardCast(new Vector2(-1, -1), _currentPiece);
                break;

            case PieceType.King:
                BitBoardCast(Vector2.up , _currentPiece, 1);
                BitBoardCast(Vector2.down, _currentPiece, 1);
                BitBoardCast(Vector2.right, _currentPiece, 1);
                BitBoardCast(Vector2.left, _currentPiece, 1);
                BitBoardCast(new Vector2(1, 1), _currentPiece, 1);
                BitBoardCast(new Vector2(-1, 1), _currentPiece, 1);
                BitBoardCast(new Vector2(1, -1), _currentPiece, 1);
                BitBoardCast(new Vector2(-1, -1), _currentPiece, 1);
                break;

            default:
                break;
        }

        return true;

    }

    private bool BitBoardCast(Vector2 direction, Vector2 startPosition, int length = -1)
    {
        // If the piece at start position don't exist
        var piece = _board.Get(startPosition);
        if (piece.Type == PieceType.None) return false;
        
        direction.Normalize();
        // Not a valid direction
        if (direction.Equals(Vector2.zero)) return false;
        
        // Cast logic 
        if (length < 0) length = Board.Size;
        var nextPosition = startPosition + direction;
        for (var i = 0; i < length; i++)
        {
            var nextState = CheckCastMove(direction, nextPosition, piece.Color);
            if(UpdateBitBoard(nextState, nextPosition)) break;
            
            nextPosition += direction;
        }
        
        return true;
    }
    
    private CheckMoveState CheckCastMove(Vector2 direction, Vector2 position, bool color)
    {
        direction.Normalize();
        
        //check next position
        var nextPosition = position + direction;
        if (nextPosition.x < 0 || nextPosition.y < 0) return CheckMoveState.ExitBoard; //Top exit
        if (nextPosition.x > Board.Size || nextPosition.y > Board.Size) return CheckMoveState.ExitBoard; //Bottom exit  
        
        //check next piece
        var nextPiece = _board.Get(nextPosition);
        //If the next position is empty
        if (nextPiece.Type == PieceType.None) return CheckMoveState.EmptyPosition;
        
        //If the next piece is the same color
        return (nextPiece.Color == color) ? CheckMoveState.AllyPiece : CheckMoveState.EnemyPiece;
    }

    private bool UpdateBitBoard(CheckMoveState state, Vector2 nextPosition)
    {
        switch (state)
        {
            case CheckMoveState.EmptyPosition:
                _currentBitBoard.Set(nextPosition, true);
                return false;
            
            case CheckMoveState.EnemyPiece:
                _currentBitBoard.Set(nextPosition, true); //Can eat
                return true;
            
            case CheckMoveState.AllyPiece: return true;
            case CheckMoveState.ExitBoard: return true;
            default: return true;
        }
    }


}
