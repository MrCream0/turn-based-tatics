using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit;

    public event EventHandler OnSwordStarted;
    public event EventHandler OnSwordCompleted;

    private int maxSwordRange = 1;

    private enum State
    {
        SwordSwingBeforeHit,
        SwordSwingAfterHit,
    }

    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwordSwingBeforeHit:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.SwordSwingAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    public override string GetActionName()
    {
        return "Sword Attack";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordRange; x <= maxSwordRange; x++)
        {
            for (int z = -maxSwordRange; z <= maxSwordRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
                Debug.Log(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwordSwingBeforeHit:
                state = State.SwordSwingAfterHit;
                float afterHitStateTime = 0.1f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(25);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwordSwingAfterHit:
                OnSwordCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.SwordSwingBeforeHit;
        float beforeHitStateTime = .7f;
        stateTimer = beforeHitStateTime;
        Debug.Log("Sword Action");

        OnSwordStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }


    public int GetMaxSwordRange()
    {
        return maxSwordRange;
    }
}
