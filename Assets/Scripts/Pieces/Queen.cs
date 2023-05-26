using UnityEngine;

public class PieceQueen : BasePiece
{
    // Override IsValidMove
    public override bool IsValidMove(int x, int y, bool targetOccupied = false)
    {
        if (!base.IsValidMove(x, y))
            return false;

        // Rook Move
        if (this.x == x && this.y != y)
        {
            return true;
        }
        else if (this.x != x && this.y == y)
        {
            return true;
        }

        // Bishop Move
        if (Mathf.Abs(this.x - x) == Mathf.Abs(this.y - y))
        {
            return true;
        }

        return false;
    }
}