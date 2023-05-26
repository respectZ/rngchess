using UnityEngine;

public class PieceBishop : BasePiece
{
    // Override IsValidMove
    public override bool IsValidMove(int x, int y, bool targetOccupied = false)
    {
        if (!base.IsValidMove(x, y))
            return false;

        if (Mathf.Abs(this.x - x) == Mathf.Abs(this.y - y))
        {
            return true;
        }

        return false;
    }
}