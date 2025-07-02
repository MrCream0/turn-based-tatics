#define USE_NEW_INPUT_SYSTEM
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public Action OnInteractPerformed;

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
    }

    private void OnEnable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.ThirdPersonPlayer.Enable();
            // Subscribe to Interaction action
            playerInputActions.ThirdPersonPlayer.Interaction.performed += ctx => OnInteractPerformed?.Invoke();
        }
        else
        {
            Debug.LogWarning("InputManager: PlayerInputActions is null, cannot enable input");
        }
    }

    private void OnDisable()
    {
        if (playerInputActions != null)
        {
            // Unsubscribe from Interaction action
            playerInputActions.ThirdPersonPlayer.Interaction.performed -= ctx => OnInteractPerformed?.Invoke();
            playerInputActions.ThirdPersonPlayer.Disable();
        }
        else
        {
            Debug.LogWarning("InputManager: PlayerInputActions is null, cannot disable input");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            if (playerInputActions != null)
            {
                playerInputActions.ThirdPersonPlayer.Interaction.performed -= ctx => OnInteractPerformed?.Invoke();
                playerInputActions?.Dispose();
            }
            Debug.Log("InputManager: Destroyed, cleaned up PlayerInputActions");
            Instance = null;
        }
    }

    public void EnableCombatController()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Enable();
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
        }
        else
        {
            Debug.LogWarning("InputManager: PlayerInputActions is null, cannot disable input");
        }
    }

    private void Update()
    {
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

    public Vector2 GetThirdPersonInput()
    {
#if USE_NEW_INPUT_SYSTEM
        Vector2 input = playerInputActions.ThirdPersonPlayer.ThirdPersonMovement.ReadValue<Vector2>();
        return input;
#else
        Vector2 input;
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        Debug.Log($"Old Input System: {input}");
        return input;
#endif
    }

    public bool GetOverworldInteract()
    {
        return playerInputActions.ThirdPersonPlayer.Interaction.WasPressedThisFrame();
    }
}