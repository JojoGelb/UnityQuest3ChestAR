using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;

public class PieceVisual : MonoBehaviour
{
    //From 1 to 8
    public Vector2 Position { get; private set; }
    public IInteractableView InteractableView { get; private set; }

    private List<TileVisual> tilesNearby = new List<TileVisual>();
    private TileVisual lastClosestIlluminatedTile;
    private bool isGrabbed = false;

    [SerializeField, Interface(typeof(IInteractableView))]
    private UnityEngine.Object _interactableView;


    private void Awake()
    {
        InteractableView = _interactableView as IInteractableView;
    }

    private void Start()
    {
        InteractableView.WhenStateChanged += OnStateChanged;
    }

    private void Update()
    {
        if (isGrabbed)
        {
            int indexClosestIlluminatedTile = GetClosestIlluminatedTile();
            if (indexClosestIlluminatedTile == -1) return;
            if (lastClosestIlluminatedTile != tilesNearby[indexClosestIlluminatedTile])
            {
                lastClosestIlluminatedTile?.ChangeLightVisual(false);
                lastClosestIlluminatedTile = tilesNearby[indexClosestIlluminatedTile];
                lastClosestIlluminatedTile.ChangeLightVisual(true);
            }
        }
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
                //OnPieceDrop();
                break;
        }
    }

    [ContextMenu("Select")]
    private void OnPieceSelected()
    {
        Debug.Log(Position);
        GameManager.Instance.SelectPiece(Position);
        isGrabbed=true;
    }

    [ContextMenu("Drop")]
    private void OnPieceDrop()
    {
        if (!isGrabbed) return;
        isGrabbed = false;
        transform.rotation = Quaternion.identity;
        //get position player want to move to
        if(tilesNearby.Count == 0)
        {
            //Failed: no tile in range
            MovePieceToTile(Position);

            return;
        }

        int indexClosestIlluminatedTile = GetClosestIlluminatedTile();

        if (indexClosestIlluminatedTile == -1)
        {
            //Failed: no illuminated tile in range
            MovePieceToTile(Position);
            return;
        }

        Vector2 destination = tilesNearby[indexClosestIlluminatedTile].position;

        //ask manager if move is possible
        MoveState result = GameManager.Instance.MoveTo((int)destination.x,(int)destination.y);
        // Debug line: allow to place piece everytime
        //MoveState result = MoveState.Success;

        switch (result)
        {
            case MoveState.Success:
            case MoveState.Eaten:
                MovePieceToTile(destination);
                lastClosestIlluminatedTile = null;
                break;

            case MoveState.Failed:
                MovePieceToTile(Position);
                lastClosestIlluminatedTile = null;
                break;
        }
    }

    private int GetClosestIlluminatedTile()
    {
        int indexClosestIlluminatedTile = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < tilesNearby.Count; i++)
        {
            if (tilesNearby[i].IsIlluminated)
            {
                float distance = Vector3.Distance(tilesNearby[i].transform.position, transform.position);
                if (distance < minDistance)
                {
                    indexClosestIlluminatedTile = i;
                    minDistance = distance;
                }
            }
        }

        return indexClosestIlluminatedTile;
    }

    public void MovePieceToTile(Vector2 destination)
    {
        TileVisual oldTile = VisualManager.Instance.GetTileVisualAtLocation(Position);
        TileVisual newTile = VisualManager.Instance.GetTileVisualAtLocation(destination);

        transform.parent = newTile.transform;
        Position = destination;
        transform.localPosition = Vector3.zero;

        if (newTile.CurrentPieceOnTile != null)
        {
            Destroy(newTile.CurrentPieceOnTile.gameObject);
        }

        newTile.CurrentPieceOnTile = this;
        if(oldTile != null)
            oldTile.CurrentPieceOnTile = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out TileVisual t))
        {
            tilesNearby.Add(t);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out TileVisual t))
        {
            tilesNearby.Remove(t);
        }
    }
}
