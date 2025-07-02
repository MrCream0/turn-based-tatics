using UnityEngine;
using TMPro;

public class PlayerCampDetection : MonoBehaviour
{
    [SerializeField] private SphereCollider detectionCollider;
    [SerializeField] private TextMeshProUGUI interactTextObject;
    public bool isActiveUI { get; private set; }

    private bool isSubscribedToInput;

    private void Awake()
    {
        detectionCollider = GetComponent<SphereCollider>();
        if (interactTextObject == null)
        {
            interactTextObject = GetComponentInChildren<TextMeshProUGUI>();
            if (interactTextObject == null)
            {
                Debug.LogError("No TextMeshProUGUI found in children!");
            }
        }
    }

    private void Start()
    {
        isActiveUI = false;
        HideText();
        TrySubscribeToInput();
    }

    private void OnEnable()
    {
        TrySubscribeToInput();
    }

    private void OnDisable()
    {
        if (isSubscribedToInput && InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPerformed -= HandleInteract;
            isSubscribedToInput = false;
        }
    }

    private void TrySubscribeToInput()
    {
        if (!isSubscribedToInput && InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPerformed += HandleInteract;
            isSubscribedToInput = true;
            Debug.Log("PlayerCampDetection: Subscribed to InputManager.OnInteractPerformed");
        }
        else if (!isSubscribedToInput)
        {
            Debug.LogWarning("PlayerCampDetection: InputManager instance not found, retrying...");
            StartCoroutine(RetrySubscribe());
        }
    }

    private System.Collections.IEnumerator RetrySubscribe()
    {
        while (!isSubscribedToInput && InputManager.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
            TrySubscribeToInput();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Detected: Show interact button");
            isActiveUI = true;
            ShowText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Exited: Hide interact button");
            isActiveUI = false;
            HideText();
            BonfireMenuController.Instance?.SetPanelInactive();
        }
    }

    private void HandleInteract()
    {
        if (isActiveUI && BonfireMenuController.Instance != null)
        {
            bool isPanelActive = BonfireMenuController.Instance.IsPanelActive();
            if (isPanelActive)
            {
                BonfireMenuController.Instance.SetPanelInactive();
            }
            else
            {
                BonfireMenuController.Instance.SetPanelActive();
            }
        }
    }

    private void ShowText()
    {
        if (interactTextObject != null)
        {
            interactTextObject.enabled = true;
        }
    }

    private void HideText()
    {
        if (interactTextObject != null)
        {
            interactTextObject.enabled = false;
        }
    }
}