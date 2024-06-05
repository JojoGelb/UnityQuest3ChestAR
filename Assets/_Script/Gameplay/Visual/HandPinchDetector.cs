
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class HandPinchDetector : MonoBehaviour
{
    [SerializeField] private HandPointer handPointer;
    [SerializeField] private HandGrabInteractor interactor;


    bool _hasPinched;
    bool _isIndexFingerPinching;
    float _pinchStrenth;
    OVRHand.TrackingConfidence _confidence;

    private void Update() => CheckPinch();

    void CheckPinch()
    {
        _pinchStrenth = handPointer.hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        _isIndexFingerPinching = handPointer.hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        _confidence = handPointer.hand.GetFingerConfidence(OVRHand.HandFinger.Index);

            if(handPointer.CurrentTarget && !_hasPinched && _isIndexFingerPinching && _confidence == OVRHand.TrackingConfidence.High)
            {
                _hasPinched = true;
                interactor.ForceSelect(handPointer.CurrentTarget.InteractableView as HandGrabInteractable);
            }
            else if(_hasPinched && !_isIndexFingerPinching)
            {
                _hasPinched = false;
                interactor.ForceRelease();
            }
    }
}