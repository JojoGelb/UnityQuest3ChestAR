using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public PieceType PieceTypeSelected = PieceType.None;

    public Image uiImage;

    public Sprite imgPawn = null;
    public Sprite imgRook = null;
    public Sprite imgBishop = null;
    public Sprite imgKnight = null;
    public Sprite imgQueen = null;
    public Sprite imgKing = null;

    public void SetImage(PieceType selectedPiece) {
        switch (selectedPiece) { 
            case PieceType.None: uiImage.sprite = null; break;
            
            case PieceType.Pawn: uiImage.sprite = imgPawn; break;
            case PieceType.Rook: uiImage.sprite = imgRook; break;
            case PieceType.Bishop: uiImage.sprite = imgBishop; break;
            case PieceType.Knight:  uiImage.sprite = imgKnight; break;
            case PieceType.Queen: uiImage.sprite = imgQueen; break;
            case PieceType.King: uiImage.sprite = imgKing; break;   
            
            default: uiImage.sprite = null; break;
                
        }
    }

    public void Start()
    {
        SetImage(PieceTypeSelected);
    }
}
