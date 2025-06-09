using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController : MonoBehaviour
{
    protected Unit unit;
    protected UnitAnimator unitAnimator;
    protected InputManager inputManager;

    public virtual void Initialize(Unit unit)
    {
        this.unit = unit;
        unitAnimator = unit.GetComponent<UnitAnimator>();// should make singleton ref?
        inputManager = InputManager.Instance;
    }

    public abstract void Enable();
    public abstract void Disable();
    public abstract void HandleInput();
    public abstract void UpdateController();

}
