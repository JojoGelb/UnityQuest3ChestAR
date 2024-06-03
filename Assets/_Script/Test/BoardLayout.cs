using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.Rendering;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Board/Layout")]
public class BoardLayout : ScriptableObject
{
    [Serializable]
    private class BoardSquareSetup
    {
        public Vector2Int position;
        public Type pieceType;
        public TeamColor TeamColor;
    }

    [SerializeField] private BoardSquareSetup[] boardSquares;

    public int GetPiecesCount() 
    {
        return boardSquares.Length;
    }

    public Vector2Int GetSquaresCoordsAtIndex(int index) 
    {
        if (boardSquares.Length <= index) 
        {
            Debug.LogError("Index of piece is out of range");
            return new Vector2Int(-1, -1);
        }
        return new Vector2Int(boardSquares[index].position.x - 1, boardSquares[index].position.y - 1);
    }

    public Type GetSquarePieceNameAtIndex(int index) 
    {
        if (boardSquares.Length <= index) {
            Debug.LogError("Index of piece is out of range");
            return Type.None;
        }
        return boardSquares[index].pieceType;
    }

    public TeamColor GetSquareTeamColorAtIndex(int index) 
    {
        if (boardSquares.Length <= index)
        {
            Debug.LogError("Index of piece is out of range");
            return TeamColor.Black;
        }
        return boardSquares[index].TeamColor;
    }
}
