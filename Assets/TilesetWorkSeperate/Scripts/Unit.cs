using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    [SerializeField] private UnitClass unitClass;
    [SerializeField] private bool isEnemy;
    [SerializeField] private EnemyPersonality enemyPersonality;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;

    private int level;
    private float maxHealth;
    private int strength;
    private int agility;
    private int intelligence;
    private int defense;

    private int experience = 0;
    private const int XP_PER_LEVEL = 100;


    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
        if (isEnemy && enemyPersonality == null)
        {
            enemyPersonality = new EnemyPersonality();
        }

        InitialzeStats();

    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }


    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldgridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldgridPosition, newGridPosition);
        }
    }

    private void InitialzeStats()
    {
        if (unitClass == null)
        {
            Debug.LogError($"Unit {gameObject.name} has not assigned UnitClass!");
            return;
        }

        level = unitClass.level;

        maxHealth = unitClass.baseMaxHealth;
        strength = unitClass.baseStrength;
        agility = unitClass.baseAgility;
        intelligence = unitClass.baseIntelligence;
        defense = unitClass.baseDefense;

        healthSystem.SetMaxHealth(maxHealth);
    }

    public void GainExperience(int xpAmmount)
    {
        experience += xpAmmount;
        while (experience >= XP_PER_LEVEL)
        {
            experience -= XP_PER_LEVEL;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;

        if (UnityEngine.Random.value < unitClass.healthGrowth)
        {
            maxHealth += 1;
        }
        if (UnityEngine.Random.value < unitClass.strengthGrowth)
        {
            strength += 1;
        }
        if (UnityEngine.Random.value < unitClass.agilityGrowth)
        {
            agility += 1;
        }
        if (UnityEngine.Random.value < unitClass.intelligenceGrowth)
        {
            intelligence += 1;
        }
        if (UnityEngine.Random.value < unitClass.defenseGrowth)
        {
            defense += 1;
        }

        healthSystem.SetMaxHealth(maxHealth);
        healthSystem.Heal(healthSystem.GetHealthNormalized() * 0.2f);
    }

    public int GetExperience() => experience;

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    /*public BaseAction[] SetBaseActions(BaseAction[] unitActions)
    {
        foreach (BaseAction i in unitActions)
        {
            
        }
    }*/

    public EnemyPersonality GetEnemyPersonality()
    {
        return enemyPersonality;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
            !IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(float damageAmmount)
    {
        healthSystem.TakeDamage(damageAmmount);
    }

    public float GetHealth()
    {
        return healthSystem.GetHealthNormalized();
    } //this initialization may cause issues with new class implimentation

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetStrength()
    {
        return strength;
    }

    public int GetAgility()
    {
        return agility;
    }

    public int GetIntelligence()
    {
        return intelligence;
    }

    public int GetDefense()
    {
        return defense;
    }

    public int GetLevel()
    {
        return level;
    }

    public UnitClass GetUnitClass()
    {
        return unitClass;
    }
}