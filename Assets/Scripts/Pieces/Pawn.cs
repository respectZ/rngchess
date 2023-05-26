using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePawn : BasePiece
{
    // Override IsValidMove
    public override bool IsValidMove(int x, int y, bool targetOccupied = false)
    {
        if (!base.IsValidMove(x, y))
            return false;
        if (isWhite)
        {
            // y + 2 for the first move
            if (y == this.y + 2 && x == this.x && !isMoved)
                return true;
            // y + 1 for the rest of the moves
            if (y == this.y + 1 && x == this.x)
                return true;
            // y + 1 and x + 1 for the attack
            if (y == this.y + 1 && x == this.x + 1 && targetOccupied)
                return true;
            // y + 1 and x - 1 for the attack
            if (y == this.y + 1 && x == this.x - 1 && targetOccupied)
                return true;
        }
        else
        {
            // y - 2 for the first move
            if (y == this.y - 2 && x == this.x && !isMoved)
                return true;
            // y - 1 for the rest of the moves
            if (y == this.y - 1 && x == this.x)
                return true;
            // y - 1 and x - 1 for the attack
            if (y == this.y - 1 && x == this.x - 1 && targetOccupied)
                return true;
            // y - 1 and x + 1 for the attack
            if (y == this.y - 1 && x == this.x + 1 && targetOccupied)
                return true;
        }
        return false;
    }
}