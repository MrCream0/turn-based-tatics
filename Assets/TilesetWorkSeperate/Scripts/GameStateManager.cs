using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Overworld;
    public enum GameState
    {
        Overworld,
        Combat
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchMode(GameState newState)
    {
        CurrentState = newState;
        switch (newState)
        {
            case GameState.Overworld:
                Debug.Log("Current State is overworld");
                break;
            case GameState.Combat:
                Debug.Log("Current State is combat");
                break;
        }
    }

}
