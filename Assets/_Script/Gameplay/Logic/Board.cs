using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    None, Pawn, Bishop, Knight, Rook, Queen, King, 
}

public enum TeamColor
{ 
    Black, White
}

public struct Piece
{
    public Piece(PieceType t, bool c)
    {
        Type = t;
        Color = c;
    }

    public PieceType Type { get; }
    public bool Color { get; }
}

public class Board
{
    private readonly Piece[] _data;
    public const int Size = 8;
    
    public Board()
    {
        //Create the 8x8 Grid
        _data = new Piece[Size*Size];
        Clear();
    }
    
    public Board(Board board)
    {
        //Create the 8x8 Grid
        _data = new Piece[Size*Size];
        Clear();
        
        for (var x = 0; x < Board.Size; x++)
        {
            for (var y = 0; y < Board.Size; y++)
            {
                Set(x, y, board.Get(x, y));
            }
        }
    }
    
    public Piece Get(Vector2 pos) { return Get((int) pos.x,(int) pos.y); }
    public Piece Get(int x, int y)
    {
        return _data[y * Size + x];
    }
    
    public void Set(Vector2 pos, Piece data) { Set((int) pos.x,(int) pos.y, data); }
    public void Set(int x, int y, Piece data)
    {
        _data[y * Size + x] = data;
    }

    public bool Move(Vector2 start, Vector2 finish)
    {
        return Move((int) start.x,(int) start.y, (int) finish.x,(int) finish.y);
    }
    public bool Move(int sx, int sy, int fx, int fy)
    {
        var result = Get(fx, fy).Type != PieceType.None;
        
        //Move (sx, sy) to (fx, fy)
        Set(fx, fy, Get(sx, sy));
        Set(sx, sy, new Piece(PieceType.None, false));

        return result;
    }

    public void Clear()
    {
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i] = new Piece(PieceType.None, false);
        }
    }

    public BoardLayout.BoardSquareSetup[] GetBoardSquareSetup()
    {
        var boardSquares = new List<BoardLayout.BoardSquareSetup>();

        for (var x = 0; x < Board.Size; x++)
        {
            for (var y = 0; y < Board.Size; y++)
            {
                var piece = Get(x, y);
                if(piece.Type == PieceType.None) continue;

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

    public void ConvertBoardSquare(BoardLayout.BoardSquareSetup[] boardSquares)
    {
        Clear();
        foreach (var boardSquare in boardSquares)
        {
            Set(
                boardSquare.position - new Vector2(1, 1), 
                new Piece(boardSquare.pieceType, boardSquare.TeamColor == TeamColor.White)
            );
        }
    }

    public List<Vector2Int> GetAllPositions(PieceType type, bool turn)
    {
        var list = new List<Vector2Int>();

        for (var x = 0; x < Board.Size; x++)
        {
            for (var y = 0; y < Board.Size; y++)
            {
                var piece = Get(x, y);
                if(piece.Type != type || piece.Color != turn) continue;
                
                list.Add(new Vector2Int(x, y));
            }
        }

        return list;
    }
    
}

