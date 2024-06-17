using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Meta.WitAi.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public enum CheckMoveState
{
    EmptyPosition, AllyPiece, EnemyPiece, ExitBoard
}

public class LogicManager
{
    private Board _board = new();
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

    public List<Vector2> SelectPiece(Vector2 position)
    {
        _currentPiece = new Vector2(position.x - 1, position.y - 1);
        _currentBitBoard.Clear();
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
                ManagePawn(piece.Color);
                break;
            
            case PieceType.Bishop:
                BitBoardCast(new Vector2(1, 1), _currentPiece);
                BitBoardCast(new Vector2(-1, 1), _currentPiece);
                BitBoardCast(new Vector2(1, -1), _currentPiece);
                BitBoardCast(new Vector2(-1, -1), _currentPiece);
                break;
            
            case PieceType.Knight:
                ManageKnight(piece.Color);
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
            
            default: throw new ArgumentOutOfRangeException();
        }

        return true;

    }

    private void ManagePawn(bool color)
    {
        var direction = (color) ? Vector2.right : Vector2.left;
        
        //Empty Pos in front
        if (CheckMove(_currentPiece + direction, color) == CheckMoveState.EmptyPosition)
        {
            BitBoardCast(direction, _currentPiece, 1);
            
            //Move two squares on first move
            if (CheckMove(_currentPiece + direction * 2, color) == CheckMoveState.EmptyPosition)
            {
                if (color ? (int) _currentPiece.x == 1 : (int) _currentPiece.x == Board.Size - 2)
                {
                    BitBoardCast(direction, _currentPiece, 2);
                }
            }
        }
        
        //EnemyPiece on diagonals
        if (CheckMove(_currentPiece + direction + Vector2.up, color) == CheckMoveState.EnemyPiece)
        {
            BitBoardCast(direction + Vector2.up, _currentPiece, 1);
        }
        if (CheckMove(_currentPiece + direction + Vector2.down, color) == CheckMoveState.EnemyPiece)
        {
            BitBoardCast(direction + Vector2.down, _currentPiece, 1);
        }
        
        //En Passant
        if (CheckMove(_currentPiece + direction, color) == CheckMoveState.EnemyPiece && 
            CheckMove(_currentPiece + direction + Vector2.down, color) == CheckMoveState.EmptyPosition &&
            CheckMove(_currentPiece + direction + Vector2.up, color) == CheckMoveState.EmptyPosition)
        {
            BitBoardCast(direction + Vector2.down, _currentPiece, 1);
            BitBoardCast(direction + Vector2.up, _currentPiece, 1);
            //TODO En Passant
        }
    }

    private void ManageKnight(bool color)
    {
        var knightPosList = new[]
        {
            new Vector2(2, 1),
            new Vector2(2, -1),
            new Vector2(-2, 1),
            new Vector2(-2, -1),
            
            new Vector2(1, 2),
            new Vector2(-1, 2),
            new Vector2(1, -2),
            new Vector2(-1, -2)
        };

        foreach (var pos in knightPosList)
        {
            UpdateBitBoard(CheckMove(_currentPiece + pos, color), _currentPiece + pos);
        }

    }
    
    private bool BitBoardCast(Vector2 direction, Vector2 startPosition, int length = -1)
    {
        // If the piece at start position don't exist
        var piece = _board.Get(startPosition);
        if (piece.Type == PieceType.None) return false;
        
        direction = GetDirectionalVector(direction);
        // Not a valid direction
        if (direction.Equals(Vector2.zero)) return false;
        
        // Cast logic 
        if (length < 0) length = Board.Size;
        var nextPosition = startPosition + direction;
        for (var i = 0; i < length; i++)
        {
            var nextState = CheckMove(nextPosition, piece.Color);
            if(UpdateBitBoard(nextState, nextPosition)) break;
            
            nextPosition += direction;
        }
        
        return true;
    }
    
    private CheckMoveState CheckMove(Vector2 position, bool color)
    {
        //check next position
        if (position.x < 0 || position.y < 0) return CheckMoveState.ExitBoard; //Top exit
        if (position.x >= Board.Size || position.y >= Board.Size) return CheckMoveState.ExitBoard; //Bottom exit  
        
        //check next piece
        var nextPiece = _board.Get(position);
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

    private static Vector2 GetDirectionalVector(Vector2 vector)
    {
        float x = vector.x < 0 ? -1 : vector.x > 0 ? 1 : 0;
        float y = vector.y < 0 ? -1 : vector.y > 0 ? 1 : 0;
        return new Vector2(x, y);
    }
    
    
    public IEnumerator GetNewChessChallenge(UnityEvent<BoardLayout.BoardSquareSetup[]> onBoardInit)
    {
        const string uri = "https://api.chess.com/pub/puzzle/random";
        using var webRequest = UnityWebRequest.Get(uri);
        
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();
        
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("Cannot get chess.com challenge, Error: " + webRequest.error);
                break;
            
            case UnityWebRequest.Result.Success:
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                
                //Parse result to json
                var request = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
                
                if (!request.ContainsKey("pgn"))
                {
                    Debug.LogError("Cannot get chess.com challenge, Error: PGN not found");
                    break;
                }
                
                CreateChallengeBoard(request["pgn"]); // Update Board with PGN
                onBoardInit.Invoke(GetBoardSquareSetup()); // Send new board to visual
                break;
            
            case UnityWebRequest.Result.InProgress: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void CreateChallengeBoard(string pgn)
    {
        var fen = GetFenFromPgn(pgn);
        var challengeBoard = new Board();
        
        var parts = fen.Split(' ');
        var rows = parts[0].Split('/');
        for (var row = 0; row < rows.Length; row++)
        {
            var col = 0;
            foreach (var c in rows[row])
            {
                if (char.IsDigit(c))
                {
                    col += (int)char.GetNumericValue(c);
                }
                else
                {
                    var isWhite = char.IsUpper(c);
                    var type = GetPieceTypeFromFenChar(char.ToLower(c));
                    var boardRow = 8 - row;
                    var boardCol = col + 1;
                    
                    challengeBoard.Set(boardRow - 1, boardCol - 1, new Piece(type, isWhite));
                    col++;
                }
            }
        }
        
        _board = challengeBoard;
    }

    private static PieceType GetPieceTypeFromFenChar(char c)
    {
        return char.ToLower(c) switch
        {
            'p' => PieceType.Pawn,
            'r' => PieceType.Rook,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => PieceType.None
        };
    }

    private static string GetFenFromPgn(string pgn)
    {
        const string pattern = @"\[FEN\s+\""(?<fen>.*?)\""\]";
        var match = Regex.Match(pgn, pattern);
        return match.Success ? match.Groups["fen"].Value : string.Empty;
    }
}
