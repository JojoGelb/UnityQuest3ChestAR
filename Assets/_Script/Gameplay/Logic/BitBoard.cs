using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitBoard
{
    private readonly bool[] _data;
    private const int Size = 8; 
    
    public BitBoard()
    {
        //Create the 8x8 Grid
        _data = new bool[Size*Size];
        for (var i = 0; i < Size*Size; i++)
        {
            _data[i] = false;
        }
    }
    
    public bool Get(Vector2 pos) { return Get((int) pos.x,(int) pos.y); }
    public bool Get(int x, int y)
    {
        return _data[y * Size + x];
    }
    
    public void Set(Vector2 pos, bool data) { Set((int) pos.x,(int) pos.y, data); }
    public void Set(int x, int y, bool data)
    {
        _data[y * Size + x] = data;
    }

}

