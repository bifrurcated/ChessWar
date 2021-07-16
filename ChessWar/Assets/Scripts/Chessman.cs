using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessRules;

public abstract class Chessman : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }

    public bool isWhite;

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        Chess chess = BoardManager.Instance.ChessEngine;

        string figure = chess.GetFigureAt(CurrentX, CurrentY).ToString();
        foreach (string move in chess.YieldValidMoves()) // Pe2e4
        {
            if (figure == move[0].ToString() &&
                CurrentX == (move[1] - 'a') &&
                CurrentY == (move[2] - '1'))
            {
                int x = move[3] - 'a';
                int y = move[4] - '1';
                r[x, y] = true;
            }
        }

        return r;
    }
}
