using UnityEngine;

class Util
{
    public static void DeepCopy<T>(T[,] src, T[,] dst)
    {
        for (int i = 0; i < src.GetLength(0); i++)
            for (int j = 0; j < src.GetLength(1); j++)
                dst[i, j] = src[i, j];
    }
    public static GameObject[,] ClonePieces(GameObject[,] originalArray)
    {
        int rows = originalArray.GetLength(0);
        int columns = originalArray.GetLength(1);

        // Create a new array with the same dimensions
        GameObject[,] newArray = new GameObject[rows, columns];

        // Iterate over the original array
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject originalObject = originalArray[i, j];
                GameObject newObject = null;

                // Perform a deep copy if the original element is not null
                if (originalObject != null)
                {
                    newObject = new GameObject();
                    newObject.transform.position = originalObject.transform.position;
                    // Pawn
                    if (originalObject.GetComponent<PiecePawn>() != null)
                    {
                        newObject.AddComponent<PiecePawn>();
                    }
                    // Rook
                    else if (originalObject.GetComponent<PieceRook>() != null)
                    {
                        newObject.AddComponent<PieceRook>();
                    }
                    // Knight
                    else if (originalObject.GetComponent<PieceKnight>() != null)
                    {
                        newObject.AddComponent<PieceKnight>();
                    }
                    // Bishop
                    else if (originalObject.GetComponent<PieceBishop>() != null)
                    {
                        newObject.AddComponent<PieceBishop>();
                    }
                    // Queen
                    else if (originalObject.GetComponent<PieceQueen>() != null)
                    {
                        newObject.AddComponent<PieceQueen>();
                    }
                    // King
                    else if (originalObject.GetComponent<PieceKing>() != null)
                    {
                        newObject.AddComponent<PieceKing>();
                    }
                    newObject.GetComponent<BasePiece>().x = originalObject.GetComponent<BasePiece>().x;
                    newObject.GetComponent<BasePiece>().y = originalObject.GetComponent<BasePiece>().y;
                    newObject.GetComponent<BasePiece>().isWhite = originalObject.GetComponent<BasePiece>().isWhite;
                    newObject.GetComponent<BasePiece>().isKing = originalObject.GetComponent<BasePiece>().isKing;
                    newObject.GetComponent<BasePiece>().isMoved = originalObject.GetComponent<BasePiece>().isMoved;
                    newObject.GetComponent<BasePiece>().isProtectCheck = originalObject.GetComponent<BasePiece>().isProtectCheck;

                }

                // Assign the new object to the corresponding position in the new array
                newArray[i, j] = newObject;
            }
        }

        return newArray;
    }
}