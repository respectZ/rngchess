using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PawnType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public class BasePiece : MonoBehaviour
{
    [SerializeField]
    public GameObject Highlight;
    private GameObject[] HighlightTiles = new GameObject[64];
    public int x;
    public int y;
    public bool isWhite;
    public bool isKing;
    public bool isMoved;
    public bool isProtectCheck = false; // Protect the king from check
    PawnType pawnType = PawnType.Pawn;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void CreateHighlight()
    {

    }
    public void ClearHighlight()
    {
    }

    public virtual bool IsValidMove(int x, int y, bool targetOccupied = false)
    {
        if (x < 0 || x > 8 || y < 0 || y > 8)
            return false;
        return !isProtectCheck;
    }

    public bool Move(int x, int y, bool isCheck = true, bool targetOccupied = false)
    {
        if (!IsValidMove(x, y, targetOccupied) && isCheck)
        {
            Debug.Log("Invalid move");
            return false;
        }
        this.x = x;
        this.y = y;
        transform.localPosition = new Vector3(x * 5.12f - (5.12f / 2), y * 5.12f - (5.12f / 2), 0);
        if (isCheck)
            isMoved = true;
        return true;
    }

    // Initialize the piece
    public void Init(bool isWhite, int x, int y)
    {
        this.isWhite = isWhite;
        this.x = x;
        this.y = y;
        isKing = false;
        isMoved = false;
        // Add CollisionBox2D
        BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.size = new Vector2(5.12f, 5.12f);
        boxCollider2D.isTrigger = true;
        // Add Rigidbody2D
        // Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        // rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        // Add White Background Sprite
        // SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        // spriteRenderer.sprite = Resources.Load<Sprite>("Square");

        Move(x, y, false);
    }

    public void OnClick()
    {
        Debug.Log("Clicked on " + gameObject.name);
    }
}
