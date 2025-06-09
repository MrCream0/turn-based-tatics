using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CameraMode { Free, Grid }
    public static CameraManager Instance { get; private set; }

    [SerializeField] private GameObject actionCameraObject;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

                Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                float shoulderOffsetAmmount = 0.5f;

                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmmount;

                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDirection * -1);

                actionCameraObject.transform.position = actionCameraPosition;
                actionCameraObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);


                ShowActionCamera();
                break;
        }
    }
    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        actionCameraObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraObject.SetActive(false);
    }

    public void SetCameraMode(CameraMode mode, Unit targetUnit = null)
    {
        if (targetUnit == null) targetUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (targetUnit == null) return;

        switch (mode)
        {
            case CameraMode.Free:
                transform.position = targetUnit.transform.position + new Vector3(0, 5, -5);
                transform.LookAt(targetUnit.transform);
                break;
            case CameraMode.Grid:
                transform.position = targetUnit.transform.position + new Vector3(0, 10, 0);
                transform.eulerAngles = new Vector3(45, 0, 0);
                break;
        }
    }
}
