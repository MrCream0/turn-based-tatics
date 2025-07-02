using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : BaseAction
{
    [SerializeField] private Transform healthCastPrefab;

    private int maxHealDistance = 5;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Heal Other";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPosition = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxHealDistance; x <= maxHealDistance; x++)
        {
            for (int z = -maxHealDistance; z <= maxHealDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testHealDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testHealDistance > maxHealDistance) continue;

                validGridPosition.Add(testGridPosition);
                Debug.Log(testGridPosition);
            }
        }

        return validGridPosition;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform healthCastTransform = Instantiate(healthCastPrefab, unit.GetWorldPosition(), Quaternion.identity);
        HealthProjectile healthProjectile = healthCastTransform.GetComponent<HealthProjectile>();
        healthProjectile.Setup(gridPosition, OnHealBehaviourComplete);
        Debug.Log("BombAction");
        ActionStart(onActionComplete);
    }

    private void OnHealBehaviourComplete()
    {
        ActionComplete();
    }
}
