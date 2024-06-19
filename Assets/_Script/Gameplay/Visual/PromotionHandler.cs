using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class PromotionHandler : MonoBehaviour
{
    public GameObject promotionPanel;

    [SerializeField, Interface(typeof(IInteractableView))]
    private UnityEngine.Object _queenInteractableView;
    [SerializeField, Interface(typeof(IInteractableView))]
    private UnityEngine.Object _bishopInteractableView;
    [SerializeField, Interface(typeof(IInteractableView))]
    private UnityEngine.Object _rookInteractableView;
    [SerializeField, Interface(typeof(IInteractableView))]
    private UnityEngine.Object _knightInteractableView;

    public IInteractableView QueenInteractableView { get; private set; }
    public IInteractableView KnightInteractableView { get; private set; }
    public IInteractableView BishopInteractableView { get; private set; }
    public IInteractableView RookInteractableView { get; private set; }


    private void Awake()
    {
        QueenInteractableView = _queenInteractableView as IInteractableView;
        KnightInteractableView = _knightInteractableView as IInteractableView;
        BishopInteractableView = _bishopInteractableView as IInteractableView;
        RookInteractableView = _rookInteractableView as IInteractableView;
    }

    private void Start()
    {
        QueenInteractableView.WhenStateChanged += OnQueenStateChanged;
        KnightInteractableView.WhenStateChanged += OnKnightStateChanged;
        BishopInteractableView.WhenStateChanged += OnBishopStateChanged;
        RookInteractableView.WhenStateChanged += OnRookStateChanged;
    }
    
    public void ShowPromotionPanel()
    {
        promotionPanel.SetActive(true);
    }

    private void HidePromotionPanel(PieceType pieceType)
    {
        promotionPanel.SetActive(false);
        VisualManager.Instance.PromotePieceTo(GetComponent<PieceVisual>().Position,pieceType,GameManager.Instance.GetPlayerColor());
    }


    private void OnRookStateChanged(InteractableStateChangeArgs args)
    {
        if(args.NewState == InteractableState.Select) {
            Debug.Log("Rook selected");
            HidePromotionPanel(PieceType.Rook);
        }
    }

    private void OnBishopStateChanged(InteractableStateChangeArgs args)
    {
        if(args.NewState == InteractableState.Select) {
            Debug.Log("Bishop selected");
            HidePromotionPanel(PieceType.Bishop);
        }
    }

    private void OnKnightStateChanged(InteractableStateChangeArgs args)
    {
        if(args.NewState == InteractableState.Select) {
            Debug.Log("Knight selected");
            HidePromotionPanel(PieceType.Knight);
        }
    }

    private void OnQueenStateChanged(InteractableStateChangeArgs args)
    {
        if(args.NewState == InteractableState.Select) {
            Debug.Log("Queen selected");
            HidePromotionPanel(PieceType.Queen);
        }
    }

    [ContextMenu("Test")]
    public void Test() {
        HidePromotionPanel(PieceType.Queen);
    }
}
