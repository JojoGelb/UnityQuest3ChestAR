using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HandPointer : MonoBehaviour
{
    public OVRHand hand;
    public PieceVisual CurrentTarget { get; private set; }

    [SerializeField] private LayerMask targetLayer;

    void Update() => CheckHandPointer();

    void CheckHandPointer()
    {
        if(Physics.Raycast(hand.PointerPose.position, hand.PointerPose.forward, out RaycastHit hit, Mathf.Infinity, targetLayer))
        {
            if(TryGetComponent(out PieceVisual piece))
            {
                CurrentTarget = piece;
            } else
            {
                CurrentTarget = null;
            }
        }
    }
}
