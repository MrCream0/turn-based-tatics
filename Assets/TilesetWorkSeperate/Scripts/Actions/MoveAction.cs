using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    //[SerializeField] private Animator unitAnimator;
    [SerializeField] private float unitSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private int maxMoveDistance = 4;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);

        float stoppingDistance = 0.1f;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * unitSpeed * Time.deltaTime;


            //unitAnimator.SetFloat("Speed", unitSpeed);
        }
        else
        {
            //unitAnimator.SetFloat("Speed", 0);
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
                Debug.Log(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    /*public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }*/

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit playerUnit = GetUnit();
        int actionValue = 0;

        if (playerUnit != null)
        {
            GridPosition playerPosition = playerUnit.GetGridPosition();
            // Calculate Manhattan distance to player
            int distanceToPlayer = Mathf.Abs(gridPosition.x - playerPosition.x) + Mathf.Abs(gridPosition.z - playerPosition.z);
            actionValue = 100 - distanceToPlayer; // Higher value for closer positions

            // Optional: Add shooting opportunity bonus
            int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
            actionValue += targetCountAtGridPosition * 10; // Boost for shooting opportunities
        }
        else
        {
            Debug.LogWarning("Player unit not found, using default action value");
        }

        // Add randomness to break ties
        actionValue += UnityEngine.Random.Range(0, 5);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = actionValue,
        };
    }
}
