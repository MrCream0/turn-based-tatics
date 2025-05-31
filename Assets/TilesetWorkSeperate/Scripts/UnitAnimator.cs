using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform projectilePrefab;
    [SerializeField] Transform projectilePointTransform;

    //Temp float for anim speed
    [SerializeField] float unitSpeed = 5f;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetFloat("Speed", unitSpeed);//temp assign to speed
    }
    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetFloat("Speed", 0);
    }

    private void ShootAction_OnShoot(object ender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Cast");

        Transform projectileTransform = Instantiate(projectilePrefab, projectilePointTransform.position, Quaternion.identity);
        SpellProjectile spellProjectile  = projectileTransform.GetComponent<SpellProjectile>();

        Vector3 targetUnitShootPosition = e.targetUnit.GetWorldPosition();

        targetUnitShootPosition.y = projectilePointTransform.position.y;
        spellProjectile.Setup(targetUnitShootPosition);
    }
}
