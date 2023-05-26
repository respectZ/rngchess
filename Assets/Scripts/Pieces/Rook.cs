using UnityEngine;

public class PieceRook : BasePiece
{
    // Override IsValidMove
    public override bool IsValidMove(int x, int y, bool targetOccupied = false)
    {
        if (!base.IsValidMove(x, y))
            return false;
        if (this.x == x && this.y != y)
        {
            return true;
        }
        else if (this.x != x && this.y == y)
        {
            return true;
        }

        return false;
    }
}