using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum Side { White, Black };
enum Piece
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public class GameManager : MonoBehaviour
{
    Side currentTurn = Side.White;

    [SerializeField]
    GameObject Tilemap;

    [SerializeField]
    bool IsDebug = false;
    [SerializeField]
    bool IsPlayingWithComputer = false;

    // Sprites
    [SerializeField]
    Sprite WhitePawnSprite;
    [SerializeField]
    Sprite BlackPawnSprite;
    [SerializeField]
    Sprite WhiteRookSprite;
    [SerializeField]
    Sprite BlackRookSprite;
    [SerializeField]
    Sprite WhiteKnightSprite;
    [SerializeField]
    Sprite BlackKnightSprite;
    [SerializeField]
    Sprite WhiteBishopSprite;
    [SerializeField]
    Sprite BlackBishopSprite;
    [SerializeField]
    Sprite WhiteQueenSprite;
    [SerializeField]
    Sprite BlackQueenSprite;
    [SerializeField]
    Sprite WhiteKingSprite;
    [SerializeField]
    Sprite BlackKingSprite;

    // Just white sprite square for highlight, attacked area
    [SerializeField]
    Sprite WhiteSquareSprite;

    // Other stuffs
    [SerializeField]
    GameObject TurnText;

    [SerializeField]
    GameObject MainMenuButton;

    private bool IsGameOver = false;
    public GameObject[,] Pieces = new GameObject[9, 9];
    public bool[,] AttackedArea = new bool[9, 9];
    private GameObject[,] AttackedAreaList = new GameObject[9, 9];
    private GameObject SelectedPawn;
    private GameObject SelectedHighlight;
    public int CountPieces(int x, int y, int xDest, int yDest)
    {
        int count = 0;
        int minX = Mathf.Min(x, xDest);
        int maxX = Mathf.Max(x, xDest);
        int minY = Mathf.Min(y, yDest);
        int maxY = Mathf.Max(y, yDest);

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                if (i == x && j == y)
                    continue;
                if (i == xDest && j == yDest)
                    continue;
                if (Pieces[i, j] != null)
                    count++;
            }
        }

        return count;
    }
    private bool CanCastle(GameObject king, GameObject rook)
    {
        if (king.GetComponent<BasePiece>().isMoved || rook.GetComponent<BasePiece>().isMoved)
            return false;
        // Get king position
        int x = king.GetComponent<BasePiece>().x;
        int y = king.GetComponent<BasePiece>().y;

        // Get rook position
        int xRook = rook.GetComponent<BasePiece>().x;
        int yRook = rook.GetComponent<BasePiece>().y;

        // Check if there is any piece between king and rook
        if (CountPieces(x, y, xRook, yRook) > 0)
            return false;

        // Check if there's attacked area between king and rook
        int minX = Mathf.Min(x, xRook);
        int maxX = Mathf.Max(x, xRook);
        for (int i = minX; i <= maxX; i++)
        {
            if (AttackedArea[i, y])
                return false;
        }

        return true;
    }
    private bool DoCastle(GameObject king, GameObject rook)
    {
        if (!CanCastle(king, rook))
            return false;
        // Get king position
        int x = king.GetComponent<BasePiece>().x;
        int y = king.GetComponent<BasePiece>().y;

        // Get rook position
        int xRook = rook.GetComponent<BasePiece>().x;

        // Move king
        king.GetComponent<BasePiece>().Move(xRook, y, isCheck: false);
        // Move rook
        rook.GetComponent<BasePiece>().Move(x, y, isCheck: false);

        SelectedPawn = null;
        Pieces[xRook, y] = king;
        Pieces[x, y] = rook;
        currentTurn = currentTurn == Side.White ? Side.Black : Side.White;
        PostMove();
        // Reset Highlight
        SelectedHighlight.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f, 0f);

        return true;
    }
    private bool IsOnBoard(int x, int y)
    {
        // X is start from 0 to 7
        // Y is start from 1 to 8
        return !(x < 0 || x > 7 || y < 1 || y > 8);
    }
    private void DrawAttackedArea(int x, int y)
    {
        if (AttackedArea[x, y])
            return;
        AttackedArea[x, y] = true;
        if (!IsDebug)
            return;
        GameObject go = new GameObject("Highlight");
        go.AddComponent<SpriteRenderer>();
        go.GetComponent<SpriteRenderer>().sprite = WhiteSquareSprite;
        // Scale to 1f
        go.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f, 1f);
        // Set color to red
        go.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 0.5f);
        go.transform.SetParent(Tilemap.transform);
        // Position
        go.transform.localPosition = new Vector3(x * 5.12f - (5.12f / 2), y * 5.12f - (5.12f / 2), 0);
        AttackedAreaList[x, y] = go;
    }
    private void RefreshAttackedArea()
    {
        // Reset
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                AttackedArea[i, j] = false;
                if (AttackedAreaList[i, j] != null)
                    Destroy(AttackedAreaList[i, j]);
            }
        }
        // Loop Pieces
        foreach (GameObject piece in Pieces)
        {
            if (piece == null)
                continue;
            if (piece.GetComponent<BasePiece>().isWhite != (currentTurn != Side.White))
                continue;
            // Pawn
            PiecePawn pawn = piece.GetComponent<PiecePawn>();
            if (pawn != null)
            {
                if (pawn.isWhite)
                {
                    if (IsOnBoard(pawn.x + 1, pawn.y + 1))
                        DrawAttackedArea(pawn.x + 1, pawn.y + 1);
                    if (IsOnBoard(pawn.x - 1, pawn.y + 1))
                        DrawAttackedArea(pawn.x - 1, pawn.y + 1);
                }
                else
                {
                    if (IsOnBoard(pawn.x + 1, pawn.y - 1))
                        DrawAttackedArea(pawn.x + 1, pawn.y - 1);
                    if (IsOnBoard(pawn.x - 1, pawn.y - 1))
                        DrawAttackedArea(pawn.x - 1, pawn.y - 1);
                }
            }
            // Rook
            PieceRook rook = piece.GetComponent<PieceRook>();
            if (rook != null)
            {
                // Top
                for (int i = rook.y + 1; i <= 8; i++)
                {
                    if (IsOnBoard(rook.x, i))
                        DrawAttackedArea(rook.x, i);
                    if (Pieces[rook.x, i] != null)
                        break;
                }
                // Bot
                for (int i = rook.y - 1; i >= 1; i--)
                {
                    if (IsOnBoard(rook.x, i))
                        DrawAttackedArea(rook.x, i);
                    if (Pieces[rook.x, i] != null)
                        break;
                }
                // Right
                for (int i = rook.x + 1; i <= 8; i++)
                {
                    if (IsOnBoard(i, rook.y))
                        DrawAttackedArea(i, rook.y);
                    if (Pieces[i, rook.y] != null)
                        break;
                }
                // Left
                for (int i = rook.x - 1; i >= 1; i--)
                {
                    if (IsOnBoard(i, rook.y))
                        DrawAttackedArea(i, rook.y);
                    if (Pieces[i, rook.y] != null)
                        break;
                }
            }
            // Knight
            PieceKnight knight = piece.GetComponent<PieceKnight>();
            if (knight != null)
            {
                if (IsOnBoard(knight.x + 1, knight.y + 2))
                    DrawAttackedArea(knight.x + 1, knight.y + 2);
                if (IsOnBoard(knight.x + 1, knight.y - 2))
                    DrawAttackedArea(knight.x + 1, knight.y - 2);
                if (IsOnBoard(knight.x - 1, knight.y + 2))
                    DrawAttackedArea(knight.x - 1, knight.y + 2);
                if (IsOnBoard(knight.x - 1, knight.y - 2))
                    DrawAttackedArea(knight.x - 1, knight.y - 2);
                if (IsOnBoard(knight.x + 2, knight.y + 1))
                    DrawAttackedArea(knight.x + 2, knight.y + 1);
                if (IsOnBoard(knight.x + 2, knight.y - 1))
                    DrawAttackedArea(knight.x + 2, knight.y - 1);
                if (IsOnBoard(knight.x - 2, knight.y + 1))
                    DrawAttackedArea(knight.x - 2, knight.y + 1);
                if (IsOnBoard(knight.x - 2, knight.y - 1))
                    DrawAttackedArea(knight.x - 2, knight.y - 1);
            }
            // Bishop
            PieceBishop bishop = piece.GetComponent<PieceBishop>();
            if (bishop != null)
            {
                // Top Right
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(bishop.x + i, bishop.y + i))
                    {
                        DrawAttackedArea(bishop.x + i, bishop.y + i);
                        if (Pieces[bishop.x + i, bishop.y + i] != null)
                            break;
                    }
                    else
                        break;
                }
                // Top Left
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(bishop.x - i, bishop.y + i))
                    {
                        DrawAttackedArea(bishop.x - i, bishop.y + i);
                        if (Pieces[bishop.x - i, bishop.y + i] != null)
                            break;
                    }
                    else
                        break;
                }
                // Bot Right
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(bishop.x + i, bishop.y - i))
                    {
                        DrawAttackedArea(bishop.x + i, bishop.y - i);
                        if (Pieces[bishop.x + i, bishop.y - i] != null)
                            break;
                    }
                    else
                        break;
                }
                // Bot Left
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(bishop.x - i, bishop.y - i))
                    {
                        DrawAttackedArea(bishop.x - i, bishop.y - i);
                        if (Pieces[bishop.x - i, bishop.y - i] != null)
                            break;
                    }
                    else
                        break;
                }
            }
            // Queen
            PieceQueen queen = piece.GetComponent<PieceQueen>();
            if (queen != null)
            {
                // Rook
                // Top
                for (int i = queen.y + 1; i <= 8; i++)
                {
                    if (IsOnBoard(queen.x, i))
                        DrawAttackedArea(queen.x, i);
                    if (Pieces[queen.x, i] != null)
                        break;
                }
                // Bot
                for (int i = queen.y - 1; i >= 1; i--)
                {
                    if (IsOnBoard(queen.x, i))
                        DrawAttackedArea(queen.x, i);
                    if (Pieces[queen.x, i] != null)
                        break;
                }
                // Right
                for (int i = queen.x + 1; i <= 8; i++)
                {
                    if (IsOnBoard(i, queen.y))
                        DrawAttackedArea(i, queen.y);
                    if (Pieces[i, queen.y] != null)
                        break;
                }
                // Left
                for (int i = queen.x - 1; i >= 1; i--)
                {
                    if (IsOnBoard(i, queen.y))
                        DrawAttackedArea(i, queen.y);
                    if (Pieces[i, queen.y] != null)
                        break;
                }

                // Bishop
                // Top Right
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(queen.x + i, queen.y + i))
                    {
                        DrawAttackedArea(queen.x + i, queen.y + i);
                        if (Pieces[queen.x + i, queen.y + i] != null)
                            break;
                    }
                    else
                        break;
                }
                // Top Left
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(queen.x - i, queen.y + i))
                    {
                        DrawAttackedArea(queen.x - i, queen.y + i);
                        if (Pieces[queen.x - i, queen.y + i] != null)
                            break;
                    }
                    else
                        break;
                }
                // Bot Right
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(queen.x + i, queen.y - i))
                    {
                        DrawAttackedArea(queen.x + i, queen.y - i);
                        if (Pieces[queen.x + i, queen.y - i] != null)
                            break;
                    }
                    else
                        break;
                }
                // Bot Left
                for (int i = 1; i <= 8; i++)
                {
                    if (IsOnBoard(queen.x - i, queen.y - i))
                    {
                        DrawAttackedArea(queen.x - i, queen.y - i);
                        if (Pieces[queen.x - i, queen.y - i] != null)
                            break;
                    }
                    else
                        break;
                }
            }
            // King
            PieceKing king = piece.GetComponent<PieceKing>();
            if (king != null)
            {
                if (IsOnBoard(king.x + 1, king.y + 1))
                    DrawAttackedArea(king.x + 1, king.y + 1);
                if (IsOnBoard(king.x + 1, king.y - 1))
                    DrawAttackedArea(king.x + 1, king.y - 1);
                if (IsOnBoard(king.x - 1, king.y + 1))
                    DrawAttackedArea(king.x - 1, king.y + 1);
                if (IsOnBoard(king.x - 1, king.y - 1))
                    DrawAttackedArea(king.x - 1, king.y - 1);
                if (IsOnBoard(king.x + 1, king.y))
                    DrawAttackedArea(king.x + 1, king.y);
                if (IsOnBoard(king.x - 1, king.y))
                    DrawAttackedArea(king.x - 1, king.y);
                if (IsOnBoard(king.x, king.y + 1))
                    DrawAttackedArea(king.x, king.y + 1);
                if (IsOnBoard(king.x, king.y - 1))
                    DrawAttackedArea(king.x, king.y - 1);
            }
        }
    }
    private int CountPiecesDiagonal(int x, int y, int xDest, int yDest)
    {
        int count = 0;

        // Top Move = yDest > y
        // Bot Move = yDest < y
        // Right Move = xDest > x
        // Left Move = xDest < x

        // Top Right Move = yDest > y && xDest > x
        if (yDest > y && xDest > x)
        {
            for (int i = 1; i < yDest - y; i++)
            {
                if (Pieces[x + i, y + i] != null)
                    count++;
            }
        }

        // Top Left Move = yDest > y && xDest < x
        if (yDest > y && xDest < x)
        {
            for (int i = 1; i < yDest - y; i++)
            {
                if (Pieces[x - i, y + i] != null)
                    count++;
            }
        }

        // Bot Right Move = yDest < y && xDest > x
        if (yDest < y && xDest > x)
        {
            for (int i = 1; i < y - yDest; i++)
            {
                if (Pieces[x + i, y - i] != null)
                    count++;
            }
        }

        // Bot Left Move = yDest < y && xDest < x
        if (yDest < y && xDest < x)
        {
            for (int i = 1; i < y - yDest; i++)
            {
                if (Pieces[x - i, y - i] != null)
                    count++;
            }
        }


        return count;
    }
    private Sprite GetPawnSpriteFromClass<T>(bool isWhite) where T : BasePiece
    {
        Sprite sprite = null;
        if (typeof(T) == typeof(PiecePawn))
        {
            if (isWhite)
                sprite = WhitePawnSprite;
            else
                sprite = BlackPawnSprite;
        }
        else if (typeof(T) == typeof(PieceRook))
        {
            if (isWhite)
                sprite = WhiteRookSprite;
            else
                sprite = BlackRookSprite;
        }
        else if (typeof(T) == typeof(PieceKnight))
        {
            if (isWhite)
                sprite = WhiteKnightSprite;
            else
                sprite = BlackKnightSprite;
        }
        else if (typeof(T) == typeof(PieceBishop))
        {
            if (isWhite)
                sprite = WhiteBishopSprite;
            else
                sprite = BlackBishopSprite;
        }
        else if (typeof(T) == typeof(PieceQueen))
        {
            if (isWhite)
                sprite = WhiteQueenSprite;
            else
                sprite = BlackQueenSprite;
        }
        else if (typeof(T) == typeof(PieceKing))
        {
            if (isWhite)
                sprite = WhiteKingSprite;
            else
                sprite = BlackKingSprite;
        }
        return sprite;
    }

    private void AddPawn<T>(int x, int y, bool isWhite) where T : BasePiece
    {
        GameObject go = new GameObject();
        go.AddComponent<SpriteRenderer>();
        go.GetComponent<SpriteRenderer>().sprite = GetPawnSpriteFromClass<T>(isWhite);
        // Scale to 0.8
        go.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        // Set Layer to 10
        go.GetComponent<SpriteRenderer>().sortingOrder = 10;
        go.transform.SetParent(Tilemap.transform);
        go.AddComponent<T>();
        go.GetComponent<T>().Init(isWhite, x, y);
        go.GetComponent<T>().isWhite = isWhite;
        Pieces[x, y] = go;
    }
    private void DeletePawn(int x, int y)
    {
        Destroy(Pieces[x, y]);
        Pieces[x, y] = null;
    }
    private void Initialize()
    {
        // Initialize the board
        for (int i = 0; i < 8; i++)
        {
            // Initialize the white pieces
            AddPawn<PiecePawn>(i, 2, true);
            // Initialize the black pieces
            AddPawn<PiecePawn>(i, 7, false);
        }
        // Initialize Rook
        foreach (int x in new int[] { 0, 7 })
        {
            // Initialize the white rooks
            AddPawn<PieceRook>(x, 1, true);
            // Initialize the black rooks
            AddPawn<PieceRook>(x, 8, false);
        }
        // Initialize Knight
        foreach (int x in new int[] { 1, 6 })
        {
            // Initialize the white knights
            AddPawn<PieceKnight>(x, 1, true);
            // Initialize the black knights
            AddPawn<PieceKnight>(x, 8, false);
        }
        // Initialize Bishop
        foreach (int x in new int[] { 2, 5 })
        {
            // Initialize the white bishops
            AddPawn<PieceBishop>(x, 1, true);
            // Initialize the black bishops
            AddPawn<PieceBishop>(x, 8, false);
        }
        // Initialize Queen
        // Initialize the white queen
        AddPawn<PieceQueen>(3, 1, true);
        // Initialize the black queen
        AddPawn<PieceQueen>(3, 8, false);
        // Initialize King
        // Initialize the white king
        AddPawn<PieceKing>(4, 1, true);
        // Initialize the black king
        AddPawn<PieceKing>(4, 8, false);

    }
    private void MainMenuClick()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
    private void Start()
    {
        Debug.Log("Current turn is " + currentTurn);
        Initialize();
        ChangeTurnText();
        // Add listener to button
        MainMenuButton.GetComponent<Button>().onClick.AddListener(MainMenuClick);

        // foreach (GameObject piece in Pieces)
        // {
        //     if (piece == null)
        //         continue;
        //     Debug.Log(piece.GetComponent<BasePiece>().isWhite + " x: " + piece.GetComponent<BasePiece>().x + " y: " + piece.GetComponent<BasePiece>().y);
        // }

    }

    private void MovePiece(GameObject piece, int x, int y, GameObject targetPiece = null)
    {
        // if (SelectedPawn == null)
        //     return;

        int origX = (int)Mathf.Floor(piece.GetComponent<BasePiece>().transform.localPosition.x / 5.12f) + 1;
        int origY = (int)Mathf.Floor(piece.GetComponent<BasePiece>().transform.localPosition.y / 5.12f) + 1;
        // Debug.Log("Moving from " + origX + "," + origY + " to " + x + "," + y);
        // Debug.Log("x: " + piece.GetComponent<BasePiece>().x + " y: " + piece.GetComponent<BasePiece>().y);

        // Check for obstructed path
        // Rook
        if (piece.GetComponent<PieceRook>() != null)
        {
            if (CountPieces(piece.GetComponent<BasePiece>().x, piece.GetComponent<BasePiece>().y, x, y) > 0)
            {
                Debug.Log("Invalid move");
                return;
            }
        }
        // Bishop
        else if (piece.GetComponent<PieceBishop>() != null)
        {
            if (CountPiecesDiagonal(piece.GetComponent<BasePiece>().x, piece.GetComponent<BasePiece>().y, x, y) > 0)
            {
                Debug.Log("Invalid move count diagonal");
                return;
            }
        }
        // Queen
        else if (piece.GetComponent<PieceQueen>() != null)
        {
            // Check if rook move
            if (origX == x && origY != y || origX != x && origY == y)
            {
                if (CountPieces(piece.GetComponent<BasePiece>().x, piece.GetComponent<BasePiece>().y, x, y) > 0)
                {
                    Debug.Log("Invalid move");
                    return;
                }
            }
            else
            {
                // Diagonal move
                if (CountPiecesDiagonal(piece.GetComponent<BasePiece>().x, piece.GetComponent<BasePiece>().y, x, y) > 0)
                {
                    Debug.Log("Invalid move count diagonal");
                    return;
                }
            }
        }
        bool ok = piece.GetComponent<BasePiece>().Move(x, y, targetOccupied: targetPiece != null);
        if (!ok)
        {
            Debug.Log("Invalid move");
            return;
        }
        if (targetPiece != null)
        {
            Debug.Log("hap");
            DeletePawn(x, y);
        }
        SelectedPawn = null;
        Pieces[origX, origY] = null;
        Pieces[x, y] = piece;
        currentTurn = currentTurn == Side.White ? Side.Black : Side.White;
        PostMove();
        // Reset Highlight
        SelectedHighlight.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f, 0f);
    }

    private void ChangeTurnText()
    {
        TurnText.GetComponent<Text>().text = currentTurn == Side.White ? "White Turn" : "Black Turn";
        if (currentTurn == Side.White)
        {
            TurnText.transform.localPosition = new Vector3(1036.2f, -665.85f, 0f);
        }
        else
        {
            TurnText.transform.localPosition = new Vector3(1036.2f, 665.85f, 0f);
        }
        TurnText.GetComponent<Text>().color = currentTurn == Side.White ? Color.white : Color.black;
        // Camera background too
        // Camera.main.backgroundColor = currentTurn == Side.White ? Color.white : Color.black;
    }

    private void CheckForCheckmate()
    {
        GameObject king = null;
        foreach (GameObject piece in Pieces)
        {
            if (piece == null)
                continue;
            if (piece.GetComponent<PieceKing>() != null)
            {
                if (piece.GetComponent<BasePiece>().isWhite && currentTurn == Side.Black)
                {
                    king = piece;
                }
                else if (!piece.GetComponent<BasePiece>().isWhite && currentTurn == Side.White)
                {
                    king = piece;
                }
            }
        }
        // Dummy solution for now
        currentTurn = currentTurn == Side.White ? Side.Black : Side.White;
        RefreshAttackedArea();
        // Check for checkmate
        if (AttackedArea[king.GetComponent<BasePiece>().x, king.GetComponent<BasePiece>().y])
        {
            IsGameOver = true;
            // Enable MainMenuButton
            MainMenuButton.SetActive(true);
            Debug.Log("Checkmate");
            // Show checkmate text
            TurnText.GetComponent<Text>().text = currentTurn == Side.White ? "Black Wins" : "White Wins";
            TurnText.GetComponent<Text>().color = currentTurn == Side.White ? Color.black : Color.white;
        }
        currentTurn = currentTurn == Side.White ? Side.Black : Side.White;
    }

    private void PostMove()
    {
        // Do something
        // Refresh attack area
        RefreshAttackedArea();
        // Change turn text
        ChangeTurnText();
        // Check for checkmate
        CheckForCheckmate();
        RefreshAttackedArea();
    }
    private void SelectPawn(GameObject pawn)
    {
        SelectedPawn = pawn;
        float x = SelectedPawn.transform.localPosition.x;
        float y = SelectedPawn.transform.localPosition.y;
        if (SelectedHighlight == null)
        {
            SelectedHighlight = new GameObject("Highlight");
            SelectedHighlight.AddComponent<SpriteRenderer>();
            SelectedHighlight.GetComponent<SpriteRenderer>().sprite = WhiteSquareSprite;
            // Scale to 1f
            SelectedHighlight.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f, 1f);
            // Set color to Y
            SelectedHighlight.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f, 0.5f);
            SelectedHighlight.transform.SetParent(Tilemap.transform);
            // Position
            SelectedHighlight.transform.localPosition = new Vector3(x, y, 0);
        }
        else
        {
            SelectedHighlight.transform.localPosition = new Vector3(x, y, 0);
            SelectedHighlight.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f, 1f);
        }
    }

    void Update()
    {
        if (currentTurn == Side.Black && IsPlayingWithComputer)
        {
            // GameObject[,] Clone = Util.ClonePieces(Pieces);
            List<Vector2Int> Move = GetComponent<ChessAI>().GenerateMove(Pieces);
            if (Move == null)
            {
                currentTurn = Side.White;
                return;
            }
            Vector2Int from = Move[0];
            Vector2Int to = Move[1];

            // Raycast
            GameObject target = Pieces[to.x, to.y];
            // Debug.Log(new Vector2(from.x * 5.12f, from.y * 5.12f));
            // if (hit.collider == null)
            // {
            //     currentTurn = Side.White;
            //     return;
            // }
            GameObject piece = Pieces[from.x, from.y];

            // foreach (GameObject piece in Pieces)
            // {
            //     if (piece == null)
            //         continue;
            //     Debug.Log(piece.GetComponent<BasePiece>().isWhite + " x: " + piece.GetComponent<BasePiece>().x + " y: " + piece.GetComponent<BasePiece>().y);
            // }
            // currentTurn = Side.White;
            // return;


            MovePiece(piece, to.x, to.y, target);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (IsGameOver)
                return;
            Vector3 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPosition = new Vector2(clickPoint.x, clickPoint.y);
            Vector2 roundedPosition = new Vector2(Mathf.Floor(clickPosition.x / 5.12f) + 1, Mathf.Floor(clickPosition.y / 5.12f) + 1);
            // Debug.Log("Clicked on " + clickPosition);
            int x = (int)roundedPosition.x;
            int y = (int)roundedPosition.y;
            // Check if the clicked position is a piece
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);
            // Debug.Log("Clicked on " + x + ", " + y);
            if (hit.collider != null)
            {
                BasePiece piece = hit.collider.gameObject.GetComponent<BasePiece>();
                // Check for castle
                if (SelectedPawn != null)
                {
                    if ((SelectedPawn.GetComponent<PieceKing>() != null && hit.collider.gameObject.GetComponent<PieceRook>()) || (SelectedPawn.GetComponent<PieceRook>() != null && hit.collider.gameObject.GetComponent<PieceKing>()))
                    {
                        if (DoCastle(SelectedPawn, hit.collider.gameObject))
                        {
                            return;
                        }
                    }
                }
                if (currentTurn == Side.White && piece.isWhite)
                {
                    SelectPawn(hit.collider.gameObject);
                }
                else if (currentTurn == Side.Black && !piece.isWhite)
                {
                    SelectPawn(hit.collider.gameObject);
                }
                else
                {
                    // Check if selected pawn & valid = eat
                    if (SelectedPawn == null)
                    {
                        Debug.Log("It's not your turn!");
                        return;
                    }
                    // Check if selectedpawn & target is different color
                    if (SelectedPawn.GetComponent<BasePiece>().isWhite == piece.isWhite)
                    {
                        Debug.Log("Invalid move");
                        return;
                    }
                    MovePiece(SelectedPawn, x, y, targetPiece: hit.collider.gameObject);
                }
            }
            else
            {
                if (SelectedPawn)
                {
                    MovePiece(SelectedPawn, x, y, targetPiece: null);
                }
                else
                {
                    SelectedPawn = null;
                    SelectedHighlight.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f, 0f);
                }
            }
        }
    }
}