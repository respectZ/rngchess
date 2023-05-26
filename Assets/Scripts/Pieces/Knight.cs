using UnityEngine;

public class PieceKnight : BasePiece
{
    // Override IsValidMove
    public override bool IsValidMove(int x, int y, bool targetOccupied = false)
    {
        if (!base.IsValidMove(x, y))
            return false;

        if (Mathf.Abs(this.x - x) == 2 && Mathf.Abs(this.y - y) == 1)
        {
            return true;
        }
        else if (Mathf.Abs(this.x - x) == 1 && Mathf.Abs(this.y - y) == 2)
        {
            return true;
        }

        return false;
    }
}