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
        Set(sx, sy, Get(fx, fy));

        return result;
    }

    public void Clear()
    {
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i] = new Piece(PieceType.None, false);
        }
    }

}

