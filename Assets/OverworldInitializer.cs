// OverworldInitializer.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldInitializer : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void Start()
    {
        // Set cursor for overworld
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("OverworldInitializer: Cursor set to hidden/locked");

        PlayerSaveData saveData = GameManager.Instance.GetSaveData();
        if (saveData.currentScene == SceneManager.GetActiveScene().name && player != null)
        {
            Vector3 position = new Vector3(
                saveData.playerOverworldPosition[0],
                saveData.playerOverworldPosition[1],
                saveData.playerOverworldPosition[2]
            );
            player.transform.position = position;
            Debug.Log("Restored position: " + position);
        }

        // Disable defeated enemies
        CombatTrigger[] enemies = FindObjectsOfType<CombatTrigger>();
        foreach (CombatTrigger enemy in enemies)
        {
            if (saveData.defeatedEnemyIds.Contains(enemy.GetComponent<CombatTrigger>().GetEnemyId()))
            {
                enemy.gameObject.SetActive(false); // Or Destroy(enemy.gameObject);
                Debug.Log($"Disabled enemy with ID {enemy.GetComponent<CombatTrigger>().GetEnemyId()}");
            }
        }
    }
}