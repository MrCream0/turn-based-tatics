// InputManager.cs
#define USE_NEW_INPUT_SYSTEM
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public Action OnInteractPerformed { get; internal set; }

    private PlayerInputActions playerInputActions;

    public delegate void MoveAction(Vector2 moveVector);
    public event MoveAction OnMovePerformed;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple InputManager instances detected! Destroying: " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        playerInputActions = new PlayerInputActions();
        Debug.Log("InputManager: Initialized PlayerInputActions");
    }

    private void OnEnable()
    {
        // Don’t enable input here; enable only for combat
    }

    private void OnDisable()
    {
        DisableCombatController();
        Debug.Log("InputManager: Disabled on OnDisable");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            DisableCombatController();
            playerInputActions?.Dispose();
            Debug.Log("InputManager: Destroyed, cleaned up PlayerInputActions");
            Instance = null;
        }
    }

    public void EnableCombatController()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Enable();
            Debug.Log("InputManager: Enabled combat input actions");
        }
        else
        {
            Debug.LogWarning("InputManager: PlayerInputActions is null, cannot enable input");
        }
    }

    public void DisableCombatController()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Disable();
            Debug.Log("InputManager: Disabled combat input actions");
        }
        else
        {
            Debug.LogWarning("InputManager: PlayerInputActions is null, cannot disable input");
        }
    }

    private void Update()
    {
        // Rename ThirdPersonMovement to CombatMovement in Input Actions asset if it’s combat-specific
        Vector2 moveInput = playerInputActions.ThirdPersonPlayer.ThirdPersonMovement.ReadValue<Vector2>();
        if (moveInput != Vector2.zero) OnMovePerformed?.Invoke(moveInput);
    }

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputMoveDir = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W)) inputMoveDir.y = +1f;
        if (Input.GetKey(KeyCode.S)) inputMoveDir.y = -1f;
        if (Input.GetKey(KeyCode.A)) inputMoveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputMoveDir.x = +1f;
        return inputMoveDir;
#endif
    }

    public float GetCameraRotateAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        float rotateAmount = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateAmount = +1f;
        if (Input.GetKey(KeyCode.E)) rotateAmount = -1f;
        return rotateAmount;
#endif
    }

    public float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        float zoomAmount = 0f;
        if (Input.mouseScrollDelta.y > 0) zoomAmount = -1f;
        if (Input.mouseScrollDelta.y < 0) zoomAmount = +1f;
        return zoomAmount;
#endif
    }
}