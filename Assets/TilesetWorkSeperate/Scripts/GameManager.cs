// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private PlayerSaveData saveData = new PlayerSaveData();
    private string saveFilePath;

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

    public void StartCombat(string combatSceneName)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            saveData.playerOverworldPosition[0] = player.transform.position.x;
            saveData.playerOverworldPosition[1] = player.transform.position.y;
            saveData.playerOverworldPosition[2] = player.transform.position.z;
        }
        saveData.currentScene = combatSceneName;
        SceneManager.LoadScene(combatSceneName);
    }

    public void EndCombat()
    {
        saveData.currentScene = "Overworld";
        SceneManager.LoadScene("Overworld");
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
        saveData.playerXP = 100; // Update with actual XP from PlayerController

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved to " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            StartCoroutine(LoadSceneAsync(saveData.currentScene));
        }
        else
        {
            Debug.LogWarning("No save file found at " + saveFilePath);
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("Game loaded from " + saveFilePath);
    }

    public void NewGame()
    {
        saveData = new PlayerSaveData
        {
            playerOverworldPosition = new float[] { 10f, 0f, 10f }, // Default spawn point
            playerXP = 0, // Starting XP
            currentScene = "Overworld"
            // Initialize other fields as needed
        };
        StartCoroutine(LoadSceneAsync("Overworld"));
        SaveGame(); // Save the new game state
        Debug.Log("New game started");
    }

    public PlayerSaveData GetSaveData()
    {
        return saveData;
    }
}