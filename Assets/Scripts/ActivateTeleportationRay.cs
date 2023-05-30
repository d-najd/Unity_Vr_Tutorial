using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateTeleportationRay : MonoBehaviour
{
    [SerializeField] private GameObject leftTeleportation;
    [SerializeField] private GameObject rightTeleportation;
    
    // [SerializeField] private XRInteractorLineVisual leftTeleportation;
    // [SerializeField] private XRInteractorLineVisual rightTeleportation;

    [SerializeField] private InputActionProperty leftActivate;
    [SerializeField] private InputActionProperty rightActivate;

    [SerializeField] private InputActionProperty leftCancel;
    [SerializeField] private InputActionProperty rightCancel;
    
    [SerializeField] private XRRayInteractor leftRay;
    [SerializeField] private XRRayInteractor rightRay;
    
    private void Update()
    {
        var isLeftRayHovering = leftRay.TryGetHitInfo(
            out var leftPos, 
            out var leftNormal, 
            out var leftNumber,
            out var leftValid);
        
        var isRightRayHovering = rightRay.TryGetHitInfo(
            out var rightPos, 
            out var rightNormal, 
            out var rightNumber,
            out var rightValid);
        
        leftTeleportation.SetActive(
            !isLeftRayHovering &&
            leftCancel.action.ReadValue<float>() == 0f &&
            leftActivate.action.ReadValue<float>() > 0.1);
        rightTeleportation.SetActive(
            !isRightRayHovering &&
            rightCancel.action.ReadValue<float>() == 0f &&
            rightActivate.action.ReadValue<float>() > 0.1f);
        // leftTeleportation.enabled = leftActivate.action.ReadValue<float>() > 0.1f;
        // rightTeleportation.enabled = rightActivate.action.ReadValue<float>() > 0.1f;
    }
}
