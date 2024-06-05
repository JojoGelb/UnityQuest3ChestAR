using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;

public class PieceVisual : MonoBehaviour
{
    public Vector2 position;

    List<TileVisual> tiles = new List<TileVisual>();

    TileVisual lastClosestIlluminatedTile;

    //AR logic for grab
    public bool isGrabbed = false;

    [SerializeField, Interface(typeof(IInteractableView))]
    private UnityEngine.Object _interactableView;

    public IInteractableView InteractableView;

    private void Awake()
    {
        InteractableView = _interactableView as IInteractableView;
    }

    private void Start()
    {
        InteractableView.WhenStateChanged += OnStateChanged;
    }

    private void OnStateChanged(InteractableStateChangeArgs args)
    {
        switch (InteractableView.State)
        {
            case InteractableState.Normal:
                OnPieceDrop();
                break;
            case InteractableState.Hover:
                Debug.Log("It's Hover anakin");
                break;
            case InteractableState.Select:
                OnPieceSelected();
                break;
            case InteractableState.Disabled:
                Debug.Log("Legumed");
                OnPieceDrop();
                break;
        }
    }

    public void OnPieceSelected()
    {
        GameManager.Instance.SelectPiece(position);
        isGrabbed=true;
    }

    [ContextMenu("OnPieceDrop")]
    public void OnPieceDrop()
    {
        isGrabbed = false;
        transform.rotation = Quaternion.identity;
        //get position player want to move to
        if(tiles.Count == 0)
        {
            //Failed: no tile in range
            VisualManager.Instance.MovePieceToTile(this, position);

            return;
        }

        int indexClosestIlluminatedTile = GetClosestIlluminatedTile();

        if (indexClosestIlluminatedTile == -1)
        {
            //Failed: no illuminated tile in range
            VisualManager.Instance.MovePieceToTile(this, position);
            return;
        }

        Vector2 destination = tiles[indexClosestIlluminatedTile].position;

        //ask manager if move is possible
        //MoveState result = GameManager.Instance.MoveTo((int)destination.x,(int)destination.y);
        // Debug line: allow to place piece everytime
        MoveState result = MoveState.Success;

        switch (result)
        {
            case MoveState.Success:
            case MoveState.Eaten:
                VisualManager.Instance.MovePieceToTile(this, destination);
                lastClosestIlluminatedTile = null;
                break;

            case MoveState.Failed:
                VisualManager.Instance.MovePieceToTile(this, position);
                lastClosestIlluminatedTile = null;
                break;
        }
    }

    private void Update()
    {
        if (isGrabbed)
        {
            int indexClosestIlluminatedTile = GetClosestIlluminatedTile();
            if (indexClosestIlluminatedTile == -1) return;
            if (lastClosestIlluminatedTile != tiles[indexClosestIlluminatedTile])
            {
                lastClosestIlluminatedTile?.ChangeLightVisual(false);
                lastClosestIlluminatedTile = tiles[indexClosestIlluminatedTile];
                lastClosestIlluminatedTile.ChangeLightVisual(true);
            }
        }
    }
    private int GetClosestIlluminatedTile()
    {
        int indexClosestIlluminatedTile = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].IsIlluminated)
            {
                float distance = Vector3.Distance(tiles[i].transform.position, transform.position);
                if (distance < minDistance)
                {
                    indexClosestIlluminatedTile = i;
                    minDistance = distance;
                }
            }
        }

        return indexClosestIlluminatedTile;
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
