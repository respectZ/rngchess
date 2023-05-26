using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChessAI : MonoBehaviour
{
    const int MAX_DEPTH = 3;
    const int MIN_DEPTH = 2;
    IDictionary<string, int> ScoreDict = new Dictionary<string, int>();
    private void Start()
    {
        ScoreDict.Add("p", -1);
        ScoreDict.Add("n", -3);
        ScoreDict.Add("b", -3);
        ScoreDict.Add("r", -5);
        ScoreDict.Add("q", -9);
        ScoreDict.Add("k", 0);
        ScoreDict.Add("P", 1);
        ScoreDict.Add("N", 3);
        ScoreDict.Add("B", 3);
        ScoreDict.Add("R", 5);
        ScoreDict.Add("Q", 9);
        ScoreDict.Add("K", 0);
    }
    private int EvalBoard(GameObject[,] Pieces)
    {
        int Score = 0;
        foreach (GameObject p in Pieces)
        {
            if (p == null)
                continue;
            BasePiece piece = p.GetComponent<BasePiece>();

            string letter = "";
            // Pawn
            if (piece.GetType() == typeof(PiecePawn))
            {
                letter = "p";
            }
            // Knight
            else if (piece.GetType() == typeof(PieceKnight))
            {
                letter = "n";
            }
            // Bishop
            else if (piece.GetType() == typeof(PieceBishop))
            {
                letter = "b";
            }
            // Rook
            else if (piece.GetType() == typeof(PieceRook))
            {
                letter = "r";
            }
            // Queen
            else if (piece.GetType() == typeof(PieceQueen))
            {
                letter = "q";
            }
            // King
            else if (piece.GetType() == typeof(PieceKing))
            {
                letter = "k";
            }

            if (piece.isWhite)
            {
                letter = letter.ToUpper();
            }

            Score += ScoreDict[letter];

        }
        return Score;
    }

    private IDictionary<Vector2Int, List<Vector2Int>> GetLegalMoves(GameObject[,] Pieces, bool isWhite)
    {
        IDictionary<Vector2Int, List<Vector2Int>> Moves = new Dictionary<Vector2Int, List<Vector2Int>>();

        foreach (GameObject piece in Pieces)
        {
            if (piece == null)
                continue;
            BasePiece basePiece = piece.GetComponent<BasePiece>();
            int origX = basePiece.x;
            int origY = basePiece.y;

            if (!Moves.ContainsKey(new Vector2Int(origX, origY)))
            {
                Moves.Add(new Vector2Int(origX, origY), new List<Vector2Int>());
            }
            // Pawn
            if (piece.GetComponent<PiecePawn>() != null)
            {
                // Valid moves are 1 forward, 2 forward, and diagonal
                // 1 forward
                if (basePiece.isWhite && isWhite)
                {
                    if (origY + 1 < 8 && Pieces[origX, origY + 1] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, origY + 1));
                    }
                }
                else if (!basePiece.isWhite && !isWhite)
                {
                    if (origY - 1 >= 0 && Pieces[origX, origY - 1] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, origY - 1));
                    }
                }
                // 2 forwawrd, isMoved = false
                if (!basePiece.isMoved)
                {
                    if (basePiece.isWhite && isWhite)
                    {
                        if (origY + 2 < 8 && Pieces[origX, origY + 2] == null)
                        {
                            Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, origY + 2));
                        }
                    }
                    else if (!basePiece.isWhite && !isWhite)
                    {
                        if (origY - 2 >= 0 && Pieces[origX, origY - 2] == null)
                        {
                            Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, origY - 2));
                        }
                    }
                }
                // Diagonal, if enemy piece
                if (basePiece.isWhite && isWhite)
                {
                    if (origX + 1 < 8 && origY + 1 < 8 && Pieces[origX + 1, origY + 1] != null && Pieces[origX + 1, origY + 1].GetComponent<BasePiece>().isWhite == false)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + 1, origY + 1));
                    }
                    if (origX - 1 >= 0 && origY + 1 < 8 && Pieces[origX - 1, origY + 1] != null && Pieces[origX - 1, origY + 1].GetComponent<BasePiece>().isWhite == false)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - 1, origY + 1));
                    }
                }
                else if (!basePiece.isWhite && !isWhite)
                {
                    if (origX + 1 < 8 && origY - 1 >= 0 && Pieces[origX + 1, origY - 1] != null && Pieces[origX + 1, origY - 1].GetComponent<BasePiece>().isWhite == true)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + 1, origY - 1));
                    }
                    if (origX - 1 >= 0 && origY - 1 >= 0 && Pieces[origX - 1, origY - 1] != null && Pieces[origX - 1, origY - 1].GetComponent<BasePiece>().isWhite == true)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - 1, origY - 1));
                    }
                }
            }
            // Rook
            if (piece.GetComponent<PieceRook>() != null)
            {
                // TODO: Add white
                if (basePiece.isWhite)
                    continue;
                // Horizontal To left
                for (int i = origX; i > 0; i--)
                {
                    if (i != origX && Pieces[i, origY] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                    }
                    else if (i != origX && Pieces[i, origY] != null && Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                        break;
                    }
                    else if (i != origX && Pieces[i, origY] != null && !Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
                // Horizontal To right
                for (int i = origX; i <= 7; i++)
                {
                    if (i != origX && Pieces[i, origY] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                    }
                    else if (i != origX && Pieces[i, origY] != null && Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                        break;
                    }
                    else if (i != origX && Pieces[i, origY] != null && !Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
                // Vertical to top
                for (int i = origY; i <= 8; i++)
                {
                    if (i != origY && Pieces[origX, i] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                    }
                    else if (i != origY && Pieces[origX, i] != null && Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                        break;
                    }
                    else if (i != origY && Pieces[origX, i] != null && !Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
                // Vertical to bot
                for (int i = origY; i >= 1; i--)
                {
                    if (i != origY && Pieces[origX, i] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                    }
                    else if (i != origY && Pieces[origX, i] != null && Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                        break;
                    }
                    else if (i != origY && Pieces[origX, i] != null && !Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
            }
            // Knight
            if (piece.GetComponent<PieceKnight>() != null)
            {
                // TODO: Add white bot
                if (basePiece.isWhite)
                    continue;

                if (origX + 2 < 8 && origY + 1 < 9 && (Pieces[origX + 2, origY + 1] == null || Pieces[origX + 2, origY + 1].GetComponent<BasePiece>().isWhite))
                {
                    Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + 2, origY + 1));
                }
                if (origX + 2 < 8 && origY - 1 >= 0 && (Pieces[origX + 2, origY - 1] == null || Pieces[origX + 2, origY - 1].GetComponent<BasePiece>().isWhite))
                {
                    Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + 2, origY - 1));
                }
                if (origX - 2 >= 0 && origY + 1 < 9 && (Pieces[origX - 2, origY + 1] == null || Pieces[origX - 2, origY + 1].GetComponent<BasePiece>().isWhite))
                {
                    Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - 2, origY + 1));
                }
                if (origX - 2 >= 0 && origY - 1 >= 0 && (Pieces[origX - 2, origY - 1] == null || Pieces[origX - 2, origY - 1].GetComponent<BasePiece>().isWhite))
                {
                    Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - 2, origY - 1));
                }
            }
            // Bishop
            if (piece.GetComponent<PieceBishop>() != null)
            {
                // TODO: Add white bot
                if (basePiece.isWhite)
                    continue;

                for (int i = 1; i <= 8; i++)
                {
                    // Top Right
                    if (origX + i < 8 && origY + i < 9 && (Pieces[origX + i, origY + i] == null || Pieces[origX + i, origY + i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + i, origY + i));
                    }
                    else if (origX + i < 8 && origY + i < 9 && !Pieces[origX + i, origY + i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                    // Bot Right
                    if (origX + i < 8 && origY - i >= 0 && (Pieces[origX + i, origY - i] == null || Pieces[origX + i, origY - i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + i, origY - i));
                    }
                    else if (origX + i < 8 && origY - i >= 0 && !Pieces[origX + i, origY - i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                    // Top Left
                    if (origX - i >= 0 && origY + i < 9 && (Pieces[origX - i, origY + i] == null || Pieces[origX - i, origY + i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - i, origY + i));
                    }
                    else if (origX - i >= 0 && origY + i < 9 && !Pieces[origX - i, origY + i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                    // Bot Left
                    if (origX - i >= 0 && origY - i >= 0 && (Pieces[origX - i, origY - i] == null || Pieces[origX - i, origY - i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - i, origY - i));
                    }
                    else if (origX - i >= 0 && origY - i >= 0 && !Pieces[origX - i, origY - i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
            }
            // Queen
            if (piece.GetComponent<PieceQueen>() != null)
            {
                // TODO: Add white bot
                if (basePiece.isWhite)
                    continue;

                // Rook Moves
                // Horizontal To left
                for (int i = origX; i > 0; i--)
                {
                    if (i != origX && Pieces[i, origY] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                    }
                    else if (i != origX && Pieces[i, origY] != null && Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                        break;
                    }
                    else if (i != origX && Pieces[i, origY] != null && !Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
                // Horizontal To right
                for (int i = origX; i <= 7; i++)
                {
                    if (i != origX && Pieces[i, origY] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                    }
                    else if (i != origX && Pieces[i, origY] != null && Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(i, origY));
                        break;
                    }
                    else if (i != origX && Pieces[i, origY] != null && !Pieces[i, origY].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
                // Vertical to top
                for (int i = origY; i <= 8; i++)
                {
                    if (i != origY && Pieces[origX, i] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                    }
                    else if (i != origY && Pieces[origX, i] != null && Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                        break;
                    }
                    else if (i != origY && Pieces[origX, i] != null && !Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
                // Vertical to bot
                for (int i = origY; i >= 1; i--)
                {
                    if (i != origY && Pieces[origX, i] == null)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                    }
                    else if (i != origY && Pieces[origX, i] != null && Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX, i));
                        break;
                    }
                    else if (i != origY && Pieces[origX, i] != null && !Pieces[origX, i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }

                // Bishop Moves
                for (int i = 1; i <= 8; i++)
                {
                    // Top Right
                    if (origX + i < 8 && origY + i < 9 && (Pieces[origX + i, origY + i] == null || Pieces[origX + i, origY + i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + i, origY + i));
                    }
                    else if (origX + i < 8 && origY + i < 9 && !Pieces[origX + i, origY + i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                    // Bot Right
                    if (origX + i < 8 && origY - i >= 0 && (Pieces[origX + i, origY - i] == null || Pieces[origX + i, origY - i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX + i, origY - i));
                    }
                    else if (origX + i < 8 && origY - i >= 0 && !Pieces[origX + i, origY - i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                    // Top Left
                    if (origX - i >= 0 && origY + i < 9 && (Pieces[origX - i, origY + i] == null || Pieces[origX - i, origY + i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - i, origY + i));
                    }
                    else if (origX - i >= 0 && origY + i < 9 && !Pieces[origX - i, origY + i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                    // Bot Left
                    if (origX - i >= 0 && origY - i >= 0 && (Pieces[origX - i, origY - i] == null || Pieces[origX - i, origY - i].GetComponent<BasePiece>().isWhite))
                    {
                        Moves[new Vector2Int(origX, origY)].Add(new Vector2Int(origX - i, origY - i));
                    }
                    else if (origX - i >= 0 && origY - i >= 0 && !Pieces[origX - i, origY - i].GetComponent<BasePiece>().isWhite)
                    {
                        break;
                    }
                }
            }
        }
        return Moves;
    }

    // Minimax
    private int Minimax(GameObject[,] Pieces, int depth, int maxDepth, bool isMaximizing)
    {
        if (depth == 0)
        {
            return EvalBoard(Pieces);
        }

        if (isMaximizing)
        {
            int bestScore = -9999;
            // Src, dests
            IDictionary<Vector2Int, List<Vector2Int>> Moves = GetLegalMoves(Pieces, false);
            foreach (KeyValuePair<Vector2Int, List<Vector2Int>> move in Moves)
            {
                foreach (Vector2Int dest in move.Value)
                {
                    GameObject[,] PiecesCopy = Util.ClonePieces(Pieces);
                    GameObject srcPiece = PiecesCopy[move.Key.x, move.Key.y];
                    GameObject destPiece = PiecesCopy[dest.x, dest.y];
                    PiecesCopy[move.Key.x, move.Key.y] = null;
                    PiecesCopy[dest.x, dest.y] = srcPiece;
                    srcPiece.GetComponent<BasePiece>().x = dest.x;
                    srcPiece.GetComponent<BasePiece>().y = dest.y;
                    if (destPiece != null)
                    {
                        PiecesCopy[dest.x, dest.y] = null;
                    }
                    int score = Minimax(PiecesCopy, depth - 1, maxDepth, false);
                    if (score > bestScore)
                    {
                        bestScore = score;
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = 9999;
            // Src, dests
            IDictionary<Vector2Int, List<Vector2Int>> Moves = GetLegalMoves(Pieces, false);
            foreach (KeyValuePair<Vector2Int, List<Vector2Int>> move in Moves)
            {
                foreach (Vector2Int dest in move.Value)
                {
                    GameObject[,] PiecesCopy = Util.ClonePieces(Pieces);
                    GameObject srcPiece = PiecesCopy[move.Key.x, move.Key.y];
                    GameObject destPiece = PiecesCopy[dest.x, dest.y];
                    PiecesCopy[move.Key.x, move.Key.y] = null;
                    PiecesCopy[dest.x, dest.y] = srcPiece;
                    srcPiece.GetComponent<BasePiece>().x = dest.x;
                    srcPiece.GetComponent<BasePiece>().y = dest.y;
                    if (destPiece != null)
                    {
                        PiecesCopy[dest.x, dest.y] = null;
                    }
                    int score = Minimax(PiecesCopy, depth - 1, maxDepth, true);
                    if (score < bestScore)
                    {
                        bestScore = score;
                    }
                }
            }
            return bestScore;
        }
    }

    public List<Vector2Int> GenerateMove(GameObject[,] Pieces, bool isWhite = false)
    {
        // foreach (GameObject piece in Pieces)
        // {
        //     if (piece == null)
        //         continue;
        //     Debug.Log(piece.GetComponent<BasePiece>().isWhite + " x: " + piece.GetComponent<BasePiece>().x + " y: " + piece.GetComponent<BasePiece>().y);
        // }
        List<Vector2Int> ReturnMove = new List<Vector2Int>();
        // src, dests
        IDictionary<Vector2Int, List<Vector2Int>> Moves = GetLegalMoves(Pieces, isWhite);
        List<int> Scores = new List<int>();

        List<Vector2Int> BestMoveDest = new List<Vector2Int>();
        List<Vector2Int> BestMoveSrc = new List<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, List<Vector2Int>> move in Moves)
        {
            foreach (Vector2Int dest in move.Value)
            {
                // Debug.Log("src: " + move.Key + " dest: " + dest);
                // GameObject[,] PiecesCopy = Pieces.Clone() as GameObject[,];
                GameObject[,] PiecesCopy = Util.ClonePieces(Pieces);
                GameObject srcPiece = PiecesCopy[move.Key.x, move.Key.y];
                GameObject destPiece = PiecesCopy[dest.x, dest.y];
                PiecesCopy[move.Key.x, move.Key.y] = null;
                PiecesCopy[dest.x, dest.y] = srcPiece;
                srcPiece.GetComponent<BasePiece>().x = dest.x;
                srcPiece.GetComponent<BasePiece>().y = dest.y;
                if (destPiece != null)
                {
                    PiecesCopy[dest.x, dest.y] = null;
                }
                // int score = Minimax(PiecesCopy, 3, 3, false);
                int score = EvalBoard(PiecesCopy);
                Scores.Add(score);
                BestMoveDest.Add(dest);
                BestMoveSrc.Add(move.Key);

                // Destroy piecescopy
                foreach (GameObject piece in PiecesCopy)
                {
                    if (piece != null)
                    {
                        Destroy(piece);
                    }
                }
            }
        }
        // return null;

        // Find best move
        int bestScore = -9999;
        int bestMoveIndex = 0;
        for (int i = 0; i < Scores.Count; i++)
        {
            if (Scores[i] > bestScore)
            {
                bestScore = Scores[i];
                bestMoveIndex = i;
            }
            else if (Scores[i] == bestScore)
            {
                // Randomly choose between two moves with same score
                if (Random.Range(0, 2) == 0)
                {
                    bestScore = Scores[i];
                    bestMoveIndex = i;
                }
            }
        }
        Debug.Log("best move is from: " + BestMoveSrc[bestMoveIndex] + " to " + BestMoveDest[bestMoveIndex] + " with score: " + bestScore);

        // Make best move
        // ReturnMove.Add(new Vector2Int(BestMoveSrc[bestMoveIndex].GetComponent<BasePiece>().x, BestMoveSrc[bestMoveIndex].GetComponent<BasePiece>().y));
        ReturnMove.Add(BestMoveSrc[bestMoveIndex]);
        ReturnMove.Add(BestMoveDest[bestMoveIndex]);
        return ReturnMove;
    }
}