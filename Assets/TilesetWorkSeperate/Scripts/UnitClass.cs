using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitClass", menuName = "Unit/Class")]
public class UnitClass : ScriptableObject
{
    public string className;
    public Sprite classIcon;

    public int level;

    public float baseMaxHealth;
    public int baseStrength;
    public int baseAgility;
    public int baseIntelligence;
    public int baseDefense;

    public float healthGrowth;
    public float strengthGrowth;
    public float agilityGrowth;
    public float intelligenceGrowth;
    public float defenseGrowth;

    public UnitType unitType;

    public float GetBaseMaxHealthStat(float health)
    {
        return baseMaxHealth;
    }

    public UnitType GetUnitType()
    {
        return unitType;
    }
}
