// PlayerSaveData.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveData
{
    public float[] playerOverworldPosition = new float[3]; // [x, y, z]
    public int unitXP;
    public int unitLevel;
    public int unitMaxHealth;
    public int unitStrength;
    public int unitAgility;
    public int unitIntelligence;
    public int unitDefense;
    public string unitClassAssetPath;
    public string currentScene = "Overworld";
    public List<int> defeatedEnemyIds = new List<int>(); // Track defeated enemies
}