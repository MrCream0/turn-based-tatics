// PlayerSaveData.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveData
{
    public float[] playerOverworldPosition = new float[3]; // [x, y, z]
    public float playerXP = 100f;
    public string currentScene = "Overworld";
    public List<int> defeatedEnemyIds = new List<int>(); // Track defeated enemies
}