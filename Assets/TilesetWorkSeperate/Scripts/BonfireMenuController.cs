using UnityEngine;

public class BonfireMenuController : MonoBehaviour
{
    public static BonfireMenuController Instance { get; private set; }
    [SerializeField] private GameObject campfirePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (campfirePanel == null)
        {
            Debug.LogError("CampfirePanel is not assigned in the Inspector!");
            return;
        }
        campfirePanel.SetActive(false);
    }

    public void SetPanelActive()
    {
        SetMouseActive();
        campfirePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SetPanelInactive()
    {
        SetMouseInactive();
        campfirePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public bool IsPanelActive()
    {
        return campfirePanel != null && campfirePanel.activeSelf;
    }

    public void SetMouseActive()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetMouseInactive()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}