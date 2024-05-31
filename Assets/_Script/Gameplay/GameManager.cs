using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{

    public UnityEvent<List<Vector2>> OnPieceSelected = new UnityEvent<List<Vector2>>();

    public void InstantiateBoard()
    {
        //Instantiate logic here: use string to tell the 
    }

    public void SelectPiece(int x, int y)
    {
        //call to logic to get available position on board


        OnPieceSelected.Invoke(new List<Vector2>());
    }
    
    public MoveState MoveTo(int x, int y)
    {
        //call to logic to know if move is possible

        if(true/*Test si result est eaten*/)
        {
            //update death row
        }

        return MoveState.Success;
    }
}
