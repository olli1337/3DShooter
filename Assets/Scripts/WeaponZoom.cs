using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponZoom : MonoBehaviour
{

    [SerializeField] Camera playerCamera;
    [SerializeField] float zoomedOutFOV;
    [SerializeField] float zoomedInFOV;
    [SerializeField] float zoomedInSensitivity;
    [SerializeField] float zoomedOutSensitivity;
    [SerializeField] bool weaponHasZoom;
    private bool zoomedInToggle = false;
    RigidbodyFirstPersonController fpsController;

    public void Start()
    {
        fpsController = GameObject.Find("Player").GetComponent<RigidbodyFirstPersonController>();
    }

    private void OnDisable()
    {
        ZoomedOut();
    }

    private void Update()
    {
        if (weaponHasZoom)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (zoomedInToggle == false)
                {
                    ZoomedIn();
                }
                else
                {
                    ZoomedOut();
                }
            }
        }
    }

    private void ResetZoomDueToWeaponChange()
    {
        zoomedInToggle = false;
        playerCamera.fieldOfView = zoomedOutFOV;
    }

    private void ZoomedOut()
    {
        zoomedInToggle = false;
        playerCamera.fieldOfView = zoomedOutFOV;
        fpsController.mouseLook.XSensitivity = zoomedOutSensitivity;
        fpsController.mouseLook.YSensitivity = zoomedOutSensitivity;
    }

    private void ZoomedIn()
    {
        zoomedInToggle = true;
        playerCamera.fieldOfView = zoomedInFOV;
        fpsController.mouseLook.XSensitivity = zoomedInSensitivity;
        fpsController.mouseLook.YSensitivity = zoomedInSensitivity;
    }
}
