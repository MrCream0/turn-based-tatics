using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    public static CombatTrigger Instance { get; private set; }

    [SerializeField] private int enemyId;

    [SerializeField] private string combatSceneName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!GameManager.Instance.GetSaveData().defeatedEnemyIds.Contains(enemyId))
            {
                GameManager.Instance.StartCombat(combatSceneName, enemyId);
            }
            else
            {
                Debug.Log($"Enemy {enemyId} already defeated, skipping combat");
            }
        }
    }

    public int GetEnemyId()
    {
        return enemyId;
    }
}
