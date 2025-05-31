using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float projetileDistancePreMoving;
    [SerializeField] private float projetileDistancePostMoving;

#nullable enable
    [SerializeField] private TrailRenderer? projectileTrail;
    [SerializeField] private Transform? projectileExplosionPrefab;
#nullable disable

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        projetileDistancePreMoving = Vector3.Distance(transform.position, targetPosition);

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        projetileDistancePostMoving = Vector3.Distance(transform.position, targetPosition);

        if (projetileDistancePreMoving < projetileDistancePostMoving)
        {
            transform.position = targetPosition;

            if (projectileTrail != null)
            {
                projectileTrail.transform.parent = null;
            }

            Destroy(gameObject);

            if (projectileExplosionPrefab != null)
            {
                Instantiate(projectileExplosionPrefab, targetPosition, Quaternion.identity);
            }

        }
    }
}
