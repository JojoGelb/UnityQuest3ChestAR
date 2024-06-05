using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
public abstract class VisualPiece : MonoBehaviour
{
    private MaterialSetter materialSetter;
    public VisualBoard board { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public TeamColor team { get; set; }
    public bool hasMoved { get; private set; }

    public List<Vector2Int> availableMoves;

    private void Awake()
    {
        availableMoves = new List<Vector2Int>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public void SetMaterial(Material material)
    {
        materialSetter = GetComponent<MaterialSetter>();
        materialSetter.SetSingleMaterial(material);
    }

    public bool CanMoveTo(Vector2Int coords) 
    {
        return availableMoves.Contains(coords);
    }

    public void SetData(Vector2Int coords, TeamColor team, VisualBoard board) 
    {
        this.team = team;
        occupiedSquare = coords;
        this.board = board;
        transform.position = board.CalculatePositionFromCoords(coords);
           
    }



}
