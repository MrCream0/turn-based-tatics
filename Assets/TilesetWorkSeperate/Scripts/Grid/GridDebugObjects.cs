using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDebugObjects : MonoBehaviour
{

    [SerializeField] private TextMeshPro textMeshPro;


    private object gridObject;

    public virtual void SetGridObject(object gridObject)
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro is not assigned!", gameObject);
            return;
        }
        /*if (gridObject == null)
        {
            Debug.LogError("GridObject is null!", gameObject);
            return;
        }*/
        textMeshPro.text = gridObject.ToString();
    }

}
