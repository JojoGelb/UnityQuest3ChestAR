using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceVisual : MonoBehaviour
{
    public Vector2 position;

    List<TileVisual> tiles = new List<TileVisual>();

    //AR logic for grab

    public void OnPieceSelected()
    {
        GameManager.Instance.SelectPiece(position);
    }

    [ContextMenu("OnPieceDrop")]
    public void OnPieceDrop()
    {
        //get position player want to move to
        if(tiles.Count == 0)
        {
            //Failed: no tile in range
            VisualManager.Instance.MovePieceToTile(transform, position);
        }

        int indexClosestIlluminatedTile = -1;
        float minDistance = float.MaxValue;
        for(int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].IsIlluminated)
            {
                float distance = Vector3.Distance(tiles[i].transform.position, transform.position);
                if (distance<minDistance)
                {
                    indexClosestIlluminatedTile = i;
                    minDistance = distance;
                }
            }
        }

        //Could be interesting to fetch closest non illuminated tile if no illuminated tile found
        if(indexClosestIlluminatedTile == -1)
        {
            //Failed: no illuminated tile in range
            VisualManager.Instance.MovePieceToTile(transform, position);
            return;
        }

        Vector2 destination = tiles[indexClosestIlluminatedTile].position;



        //ask manager if move is possible
        MoveState result = MoveState.Success; //  GameManager.Instance.MoveTo((int)destination.x,(int)destination.y);

        switch(result)
        {
            case MoveState.Success:
                VisualManager.Instance.MovePieceToTile(transform, destination);
                position= destination;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out TileVisual t))
        {
            tiles.Add(t);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out TileVisual t))
        {
            tiles.Remove(t);
        }
    }
}
