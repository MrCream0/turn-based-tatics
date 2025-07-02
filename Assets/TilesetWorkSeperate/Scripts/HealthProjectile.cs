using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthProjectile : MonoBehaviour
{
    [SerializeField] private float castSpeed;
    [SerializeField] private float healRadius;
    [SerializeField] private Transform healEffectPrefab;
    //trail renderer and animation Curve in future

    private Action onHealBehaviourComplete;

    private float totalDistance;

    private Vector3 positionXZ;

    private void Update()
    {
        
    }

    public void Setup(GridPosition targetGridPosition, Action onHealBehaviourComplete)
    {

    }
}
