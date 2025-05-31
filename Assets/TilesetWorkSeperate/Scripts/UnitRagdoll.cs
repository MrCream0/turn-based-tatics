using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;
    [SerializeField] private float ragdollExplosionForce = 200f;
    [SerializeField] private float ragdollExplosionRange = 10f;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        Vector3 randomDirection = new Vector3(Random.Range(01f, +1f), 0, Random.Range(-1f, +1f));
        ApplyExplosionToRagdoll(ragdollRootBone, ragdollExplosionForce, transform.position + randomDirection, ragdollExplosionRange);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.SetPositionAndRotation(child.position, child.rotation);

                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
