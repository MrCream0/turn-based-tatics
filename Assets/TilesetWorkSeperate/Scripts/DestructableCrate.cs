using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;
    //[SerializeField] private float force;
    //[SerializeField] private float range;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    public void Damage()
    {

        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(01f, +1f), 0, UnityEngine.Random.Range(-1f, +1f));

        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(crateDestroyedPrefab, 150f, transform.position + randomDirection, 100f);

        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
