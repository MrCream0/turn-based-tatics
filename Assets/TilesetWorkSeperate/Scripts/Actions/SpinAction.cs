using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{

    private float totalSpinAmmount;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAddAmmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmmount, 0);

        totalSpinAmmount += spinAddAmmount;
        if (totalSpinAmmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmmount = 0f;
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        //List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>
        {
            unitGridPosition
        };
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
}
