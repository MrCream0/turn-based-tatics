using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        // No more enemies have actions they can take, end enemy turn
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    /*private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                // Enemy cannot afford this action
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }

        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }*/

    //Refactoring
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        List<(BaseAction action, EnemyAIAction aiAction)> actionList = new List<(BaseAction, EnemyAIAction)>();

        // Evaluate all possible actions
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                continue;
            }

            if (baseAction is MoveAction moveAction)
            {
                actionList.AddRange(EvaluateMoveAction(enemyUnit, moveAction));
            }
            else if (baseAction is ShootAction shootAction)
            {
                EnemyAIAction shootAIAction = EvaluateShootAction(enemyUnit, shootAction);
                if (shootAIAction != null)
                {
                    actionList.Add((shootAction, shootAIAction));
                }
            }
            // Add other actions (e.g., GrenadeAction) here
        }

        // Select the best action
        if (actionList.Count == 0)
        {
            return false;
        }

        actionList.Sort((a, b) => b.aiAction.actionValue.CompareTo(a.aiAction.actionValue));
        int topActionsCount = Mathf.Min(3, actionList.Count);
        var selected = actionList[UnityEngine.Random.Range(0, topActionsCount)];

        if (enemyUnit.TrySpendActionPointsToTakeAction(selected.action))
        {
            selected.action.TakeAction(selected.aiAction.gridPosition, onEnemyAIActionComplete);
            Debug.Log($"AI chose {selected.action.GetActionName()} at {selected.aiAction.gridPosition} with value {selected.aiAction.actionValue}");
            return true;
        }

        return false;
    }

    private List<(BaseAction, EnemyAIAction)> EvaluateMoveAction(Unit unit, MoveAction moveAction)
    {
        List<(BaseAction, EnemyAIAction)> moveActions = new List<(BaseAction, EnemyAIAction)>();
        List<GridPosition> validMovePositions = moveAction.GetValidActionGridPositionList();

        foreach (GridPosition movePos in validMovePositions)
        {
            int moveValue = 0;

            // Distance to nearest player
            Unit nearestPlayer = GetNearestPlayerUnit(movePos);
            if (nearestPlayer != null)
            {
                float distance = Vector3.Distance(
                    LevelGrid.Instance.GetWorldPosition(movePos),
                    LevelGrid.Instance.GetWorldPosition(nearestPlayer.GetGridPosition())
                );
                moveValue += Mathf.RoundToInt((10f - distance) * 5f); // Closer is better
            }

            // Bonus for cover
            if (IsTileInCover(movePos))
            {
                moveValue += 15;
            }

            // Penalty for clustering
            if (IsTileNearOtherEnemies(unit, movePos))
            {
                moveValue -= 10;
            }

            // Random factor
            moveValue += UnityEngine.Random.Range(0, 5);

            moveActions.Add((moveAction, new EnemyAIAction
            {
                gridPosition = movePos,
                actionValue = moveValue
            }));
        }

        return moveActions;
    }

    private EnemyAIAction EvaluateShootAction(Unit unit, ShootAction shootAction)
    {
        GridPosition currentPos = unit.GetGridPosition();
        List<GridPosition> validShootPositions = shootAction.GetValidActionGridPositionList(currentPos);
        if (validShootPositions.Count == 0)
        {
            return null;
        }

        // Find best target
        int bestShootValue = 0;
        GridPosition bestTargetPos = currentPos;
        foreach (GridPosition shootPos in validShootPositions)
        {
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(shootPos);
            if (targetUnit == null) continue;

            int shootValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealth()) * 100f);
            if (shootValue > bestShootValue)
            {
                bestShootValue = shootValue;
                bestTargetPos = shootPos;
            }
        }

        if (bestShootValue > 0)
        {
            return new EnemyAIAction
            {
                gridPosition = bestTargetPos,
                actionValue = bestShootValue
            };
        }

        return null;
    }

    private Unit GetNearestPlayerUnit(GridPosition fromPosition)
    {
        Unit closestUnit = null;
        float minDistance = float.MaxValue;
        foreach (Unit unit in UnitManager.Instance.GetFriendlyUnitList())
        {
            float distance = Vector3.Distance(
                LevelGrid.Instance.GetWorldPosition(fromPosition),
                LevelGrid.Instance.GetWorldPosition(unit.GetGridPosition())
            );
            if (distance < minDistance)
            {
                minDistance = distance;
                closestUnit = unit;
            }
        }
        return closestUnit;
    }

    private bool IsTileInCover(GridPosition gridPosition)
    {
        Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(gridPosition);
        return Physics.CheckBox(worldPos, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask("Obstacles"));
    }

    private bool IsTileNearOtherEnemies(Unit currentUnit, GridPosition gridPosition)
    {
        foreach (Unit enemy in UnitManager.Instance.GetEnemyUnitList())
        {
            if (enemy != currentUnit && Vector3.Distance(
                LevelGrid.Instance.GetWorldPosition(gridPosition),
                LevelGrid.Instance.GetWorldPosition(enemy.GetGridPosition())) < 2f)
            {
                return true;
            }
        }
        return false;
    }
}
