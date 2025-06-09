// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayerSaveData saveData = new PlayerSaveData();
    private string saveFilePath;
    private int currentEnemyId; // Track the current enemy for combat

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        saveFilePath = Application.persistentDataPath + "/savegame.sav";
    }

    public void StartCombat(string combatSceneName, int enemyId)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            saveData.playerOverworldPosition[0] = player.transform.position.x;
            saveData.playerOverworldPosition[1] = player.transform.position.y;
            saveData.playerOverworldPosition[2] = player.transform.position.z;
        }
        saveData.currentScene = combatSceneName;
        currentEnemyId = enemyId; // Store enemy ID for this combat
        StartCoroutine(LoadSceneAsync(combatSceneName, isCombat: true));
    }

    public void EndCombat(bool victory)
    {
        if (victory)
        {
            // Mark enemy as defeated
            if (!saveData.defeatedEnemyIds.Contains(currentEnemyId))
            {
                saveData.defeatedEnemyIds.Add(currentEnemyId);
            }
            SaveGame(); // Save the defeated state
        }
        saveData.currentScene = "Overworld";
        StartCoroutine(LoadSceneAsync("Overworld", isCombat: false));
    }

    public void SaveGame()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            saveData.playerOverworldPosition[0] = player.transform.position.x;
            saveData.playerOverworldPosition[1] = player.transform.position.y;
            saveData.playerOverworldPosition[2] = player.transform.position.z;
        }
        saveData.currentScene = SceneManager.GetActiveScene().name;
        saveData.playerXP = 100; // Update with actual XP

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved to " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                saveData = JsonUtility.FromJson<PlayerSaveData>(json);
                bool isCombat = saveData.currentScene != "Overworld";
                StartCoroutine(LoadSceneAsync(saveData.currentScene, isCombat));
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load save file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("No save file found at " + saveFilePath);
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool isCombat)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (isCombat)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("Combat scene loaded, cursor set to visible/unlocked");
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Overworld scene loaded, cursor set to hidden/locked");
        }
    }

    public void NewGame()
    {
        saveData = new PlayerSaveData
        {
            playerOverworldPosition = new float[] { 10f, 0f, 10f },
            playerXP = 0,
            currentScene = "Overworld",
            defeatedEnemyIds = new List<int>()
        };
        StartCoroutine(LoadSceneAsync("Overworld", isCombat: false));
        SaveGame();
        Debug.Log("New game started");
    }

    public PlayerSaveData GetSaveData()
    {
        return saveData;
    }
}