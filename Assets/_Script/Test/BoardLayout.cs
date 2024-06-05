using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Board/Layout")]
public class BoardLayout : ScriptableObject
{
    [Serializable]
    public class BoardSquareSetup
    {
        public Vector2Int position;
        public PieceType pieceType;
        public TeamColor TeamColor;
    }

    [SerializeField] private BoardSquareSetup[] boardSquares;

    [HideInInspector]
    public BoardSquareSetup[] BoardSquares { get { return boardSquares; }}
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

    public string GetSquarePieceNameAtIndex(int index) 
    {
        if (boardSquares.Length <= index) {
            Debug.LogError("Index of piece is out of range");
            return "";
        }
        return boardSquares[index].pieceType.ToString();
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
