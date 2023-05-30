using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintGun : MonoBehaviour
{
    [SerializeField] private Transform paintExitPoint;
    [SerializeField] private float raycastDistance = 30f;

    private void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Paint);
    }

    private void Paint(ActivateEventArgs arg)
    {
        Physics.aaaaaaaaaaaaaa
        Physics.Raycast(paintExitPoint.transform, Vector3.forward, raycastDistance);
        paintExitPoint.forward   
    }

}
