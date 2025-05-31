using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BombProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyBombExploded;

    private Vector3 targetPosition;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float damageRadius;
    [SerializeField] private Transform bombExplostionPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Action onBombBehaviourComplete;

    private float totalDistance;

    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;

        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = .2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius); //Test for layer mask incase of specific colliders

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(30);//possibly use different damage types 
                }
                if (collider.TryGetComponent<DestructableCrate>(out DestructableCrate destructableCrate))
                {
                    destructableCrate.Damage();
                }
            }

            OnAnyBombExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;

            Instantiate(bombExplostionPrefab, targetPosition + Vector3.up * 1, Quaternion.identity);

            Destroy(gameObject);

            onBombBehaviourComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onBombBehaviourComplete)
    {
        this.onBombBehaviourComplete = onBombBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
