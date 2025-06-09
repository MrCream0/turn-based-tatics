using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.SaveGame();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            GameManager.Instance.LoadGame();
        }
    }
}
