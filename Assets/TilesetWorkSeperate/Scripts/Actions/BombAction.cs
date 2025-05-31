using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : BaseAction
{

    [SerializeField] private Transform bombProjectilePrefab;

    private int maxThrowDistance = 10;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

       // ActionComplete();
        
    }

    public override string GetActionName()
    {
        return "Bomb";
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
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > maxThrowDistance)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
                Debug.Log(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform bombProjectileTransform = Instantiate(bombProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        BombProjectile bombProjectile = bombProjectileTransform.GetComponent<BombProjectile>();
        bombProjectile.Setup(gridPosition, OnBombBehaviourComplete);
        Debug.Log("BombAction");
        ActionStart(onActionComplete);
    }

    private void OnBombBehaviourComplete()
    {
        ActionComplete();
    }
}
