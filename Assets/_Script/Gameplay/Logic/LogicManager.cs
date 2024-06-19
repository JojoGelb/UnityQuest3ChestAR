using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Meta.WitAi.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public enum CheckMoveState
{
    EmptyPosition, AllyPiece, EnemyPiece, ExitBoard
}

public struct ChessMove
{
    public Vector2Int Start { get; }
    public Vector2Int End { get; }

    public bool DoesEat { get; }

    public ChessMove(Vector2Int start, Vector2Int end, bool doesEat = false)
    {
        Start = start;
        End = end;
        DoesEat = doesEat;
    }
}

public class LogicManager
{
    private Board _board = new();
    private readonly BitBoard _currentBitBoard = new();
    private Vector2 _currentPiece;
    private Vector2 _lastPieceMoved;

    private Queue<ChessMove> _nextChallengeMoves;
    private ChessMove _playerNextMove;
    private ChessMove _enemyLastMove;
    private bool _isWhiteTurn = true;
    private bool _isPlayerWhite = true;

    private Vector2Int _lastMovedPawn = new Vector2Int(-1, -1); 
    
    private BoardLayout _boardLayout;
    
    // Initiate Board with BoardSquares
    public void InitBoard(BoardLayout boardLayout)
    {
        _boardLayout = boardLayout;
        _board.ConvertBoardSquare(_boardLayout.BoardSquares);
    }
    
    // Get BoardSquares from board
    public BoardLayout.BoardSquareSetup[] GetBoardSquareSetup()
    {
        return _board.GetBoardSquareSetup();
    }
    
    public List<Vector2> SelectPiece(Vector2 position)
    {
        _currentPiece = new Vector2(position.x - 1, position.y - 1);
        ManageValidMoves(); //Update BitBoard
        
        return _currentBitBoard.GetValidMoves();
    }
    
    public MoveState MoveTo(int x, int y, UnityEvent<TeamColor> onPawnPromotion, bool force = false)
    {
        var newPosition = new Vector2(x - 1, y - 1);
        
        if (_currentBitBoard.Get(newPosition) || force)
        {
            //Check for Pawn Promotion
            var selectedPiece = _board.Get(_currentPiece);
            if (IsPawnPromoting(selectedPiece, x - 1))
            {
                onPawnPromotion.Invoke(selectedPiece.Color ? TeamColor.White : TeamColor.Black);
            }
            
            //Move Pawn
            _lastPieceMoved = newPosition;
            _isWhiteTurn = !_isWhiteTurn;
            return _board.Move(_currentPiece, newPosition) ? MoveState.Eaten : MoveState.Success;
        }

        return MoveState.Failed;
    }

    // Update the inner BitBoard with the valid move from the selected piece
    private void ManageValidMoves(bool forceTurn = true)
    {
        _currentBitBoard.Clear();
        var piece = _board.Get(_currentPiece);
        if (piece.Type == PieceType.None) return;
        if(piece.Color != _isWhiteTurn && forceTurn) return; 
        
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
                break;
            
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
            
            case PieceType.None: break;
            default: throw new ArgumentOutOfRangeException();
        }

    }
    
    // Manage Pawn valid moves logic
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
            //TODO Delete Front Pawn
        }
    }
    
    // Manage Knight valid moves logic
    private void ManageKnight(bool color)
    {
        var knightPosList = new[]
        {
            new Vector2(2, 1), new Vector2(2, -1), 
            new Vector2(-2, 1), new Vector2(-2, -1),
            new Vector2(1, 2), new Vector2(-1, 2), 
            new Vector2(1, -2), new Vector2(-1, -2)
        };

        foreach (var pos in knightPosList)
        {
            UpdateBitBoard(CheckMove(_currentPiece + pos, color), _currentPiece + pos);
        }
    }
    
    private void BitBoardCast(Vector2 direction, Vector2 startPosition, int length = -1)
    {
        // If the piece at start position don't exist
        var piece = _board.Get(startPosition);
        if (piece.Type == PieceType.None) return;
        
        direction = GetDirectionalVector(direction);
        // Not a valid direction
        if (direction.Equals(Vector2.zero)) return;
        
        // Cast logic 
        if (length < 0) length = Board.Size;
        var nextPosition = startPosition + direction;
        for (var i = 0; i < length; i++)
        {
            var nextState = CheckMove(nextPosition, piece.Color);
            if(UpdateBitBoard(nextState, nextPosition)) break;
            
            nextPosition += direction;
        }
    }
    
    // Check if this move is valid or not
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
    
    // Update Bit Board with the MoveState
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
    
    private static bool IsPawnPromoting(Piece selectedPiece, int x)
    {
        return selectedPiece.Type == PieceType.Pawn && (selectedPiece.Color ? x == Board.Size - 1 : x == 0);
    }

    public void PromotePawn(PieceType newType)
    {
        var pawn = _board.Get(_lastPieceMoved);
        if(!IsPawnPromoting(pawn, (int) _lastPieceMoved.x)) return;
        
        _board.Set(_lastPieceMoved, new Piece(newType, pawn.Color));
    }
    
    
    
    
    //Make a request to get a new challenge from chess.com
    public IEnumerator GetNewChessChallenge(
        UnityEvent<BoardLayout.BoardSquareSetup[]> onBoardInit,
        UnityEvent onChallengeBegin
    )
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

                if (!CreateChallengeBoard(request["pgn"])) // Update Board with PGN
                {
                    Debug.LogError("Cannot create moves, Error: challenge not supported");
                    
                    yield return new WaitForSeconds(15);
                    GameManager.Instance.GetNewChallenge();
                    break;
                } 
                onBoardInit.Invoke(GetBoardSquareSetup()); // Send new board to visual
                onChallengeBegin.Invoke();
                break;
            
            case UnityWebRequest.Result.InProgress: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
    // Update logic with a given pgn
    private bool CreateChallengeBoard(string pgn)
    {
        var fen = GetFenFromPgn(pgn);
        var fenBoard = GetBoardFromFen(fen);
        _board = new Board(fenBoard);
        
        // Get next color from PGN
        _isPlayerWhite = GetPlayerColorFromFen(fen);
        _isWhiteTurn = _isPlayerWhite;
        
        // Get next moves list from PGN
        _nextChallengeMoves = GetPgnMoves(pgn);
        if (!_nextChallengeMoves.Any()) return false;
        
        _playerNextMove = _nextChallengeMoves.Dequeue();
        _board = new Board(fenBoard);
        return true;
    }

    private static Board GetBoardFromFen(string fen)
    {
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
                    var boardRow = 7 - row;
                    var boardCol = col;
                    
                    challengeBoard.Set(boardRow, boardCol, new Piece(type, isWhite));
                    col++;
                }
            }
        }

        return challengeBoard;
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

    private static bool GetPlayerColorFromFen(string fen)
    {
        var fenParts = fen.Split(' ');
        if (fenParts.Length < 2)
        {
            return true;
        }
        
        var color = fenParts[1];
        return color switch
        {
            "w" => true,
            "b" => false,
            _ => true
        };
    }

    private Queue<ChessMove> GetPgnMoves(string pgn)
    {
        var queue = new Queue<ChessMove>();
        var moves = ExtractPgnMoves(pgn);
        var turn = _isWhiteTurn;
        
        foreach (var move in moves)
        {
            var type = GetPieceTypeFromAlgebraic(move);
            var end = GetEndFromAlgebraic(move);
            if (end.x < 0 || end.y < 0) continue;
            
            var start = FoundStartPosition(type, end, turn);
            if (start.x < 0 || start.y < 0)
            {
                return new Queue<ChessMove>();
            }
            
            queue.Enqueue(new ChessMove(start, end));
            _board.Move(start, end);
            
            turn = !turn;
        }

        return queue;
    }
    
    private static PieceType GetPieceTypeFromAlgebraic(string algebraicNotation)
    {
        if (char.IsLower(algebraicNotation[0])) return PieceType.Pawn;
        return algebraicNotation[0] switch
        {
            'R' => PieceType.Rook,
            'N' => PieceType.Knight,
            'B' => PieceType.Bishop,
            'Q' => PieceType.Queen,
            'K' => PieceType.King,
            _ => PieceType.None
        };
    }

    private static Vector2Int GetEndFromAlgebraic(string algebraicNotation)
    {
        const string pattern = @"[a-h][1-8]";
        var match = Regex.Matches(algebraicNotation, pattern);

        if (match.Count == 0) return new Vector2Int(-1, -1);
        
        var stringPos = match[^1].Value;
        
        return new Vector2Int(
            (int)char.GetNumericValue(stringPos[1]) - 1,
            stringPos[0] - 'a'
        );
    }

    private Vector2Int FoundStartPosition(PieceType type, Vector2Int end, bool turn)
    {
        foreach (var position in _board.GetAllPositions(type, turn))
        {
            _currentPiece = position;
            ManageValidMoves(false);
            
            if (_currentBitBoard.Get(end))
            {
                return position;
            }
        }
        
        
        Debug.Log("Fail to find Start End : " + end.x + "; " + end.y);
        return new Vector2Int(-1, -1);
    }
    
    private static List<string> ExtractPgnMoves(string pgn)
    {
        var moves = new List<string>();

        // Find the section of the PGN containing the moves
        const string pattern = @"\d+\.\s*(\S+)\s+(\S+)";
        var matches = Regex.Matches(pgn, pattern);

        foreach (Match match in matches)
        {
            moves.Add(match.Groups[1].Value);
            
            if (match.Groups[2].Success)
            {
                moves.Add(match.Groups[2].Value);
            }
        }

        return moves;
    }
    
    public MoveState CheckChallengeMove(int x, int y, UnityEvent<TeamColor> onPawnPromotion)
    {
        if(_isPlayerWhite != _isWhiteTurn) return MoveState.Failed;
        
        var position = new Vector2Int(x - 1, y - 1);
        if (_playerNextMove.Start == _currentPiece && _playerNextMove.End == position)
        {
            _isWhiteTurn = !_isWhiteTurn;
            return MoveTo(x, y, onPawnPromotion, true);
        }

        return MoveState.Failed;
    }

    public ChessMove GetNextChallengeMove()
    {
        if (_isWhiteTurn == _isPlayerWhite) return _enemyLastMove;

        var triggerEat = false;
        if (!IsChallengeFinish())
        {
            _enemyLastMove = _nextChallengeMoves.Dequeue();
            triggerEat = _board.Move(_enemyLastMove.Start, _enemyLastMove.End);
        }
        if(!IsChallengeFinish()) _playerNextMove = _nextChallengeMoves.Dequeue();
        
        return new ChessMove(
            _enemyLastMove.Start + new Vector2Int(1, 1),
            _enemyLastMove.End + new Vector2Int(1, 1),
            triggerEat
        );
    }

    public bool IsChallengeFinish()
    {
        return !_nextChallengeMoves.Any();
    }

    public bool GetPlayerColor()
    {
        return _isPlayerWhite;
    }
   
}
