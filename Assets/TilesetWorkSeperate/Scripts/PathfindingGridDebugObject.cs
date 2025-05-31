using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PathfindingGridDebugObject : GridDebugObjects
{

    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private TextMeshPro fCostText;
    [SerializeField] private SpriteRenderer isWalkableSpriteRenderer;

    private PathNode pathNode;

    public override void SetGridObject(object gridObject)
    {
        //Debug.Log($"SetGridObject called with: {gridObject?.GetType()?.Name ?? "null"}");
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
       // if (pathNode == null)
        //{
          //  Debug.LogError("PathNode is null after casting!", gameObject);
       // }
    }

    protected override void Update()
    {
        base.Update();
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
        isWalkableSpriteRenderer.color = pathNode.IsWalkable() ? Color.green : Color.red;
    }
}

