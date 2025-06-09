using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public float[] playerOverworldPosition = new float[3]; //vector3
    //public float playerHealth;
    public string currentScene;
    public int playerXP;
    public int[] playerInventory;
}
