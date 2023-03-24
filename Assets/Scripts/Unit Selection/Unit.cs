using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit : MonoBehaviour
{
    [SerializeField] protected UnitData unitData;
    [SerializeField] protected Faction faction;
    [SerializeField] private Image cirlce;
    [SerializeField] private Slider healthSlider;
    [SerializeField] protected SphereCollider combatRangeTrigger;
    [SerializeField] protected List<ParticleSystem> thrusters = new List<ParticleSystem>();

    protected int maxHealth;
    protected int currentHealth;
    protected List<Unit> enemyUnitsInCombatRange = new List<Unit>();
    protected Unit currentCombatTarget;

    public System.Action<Unit> OnUnitDestroyed;

    protected virtual void Start()
    {
        Deleselect();
        maxHealth = unitData.baseHealth;
        currentHealth = maxHealth;
        currentCombatTarget = null;
        combatRangeTrigger.radius = unitData.combatRadius;
        combatRangeTrigger.isTrigger = true;
        for (int i = 0; i < thrusters.Count; i++)
        {
            ParticleSystem.MainModule main = thrusters[i].main; 
            main.startColor = faction.selectedColor;
        }
    }

    protected virtual void Update()
    {
        float t = Mathf.Clamp01((float)currentHealth / (float)maxHealth);
        healthSlider.value = t;

        if(currentHealth <= 0)
        {
            OnUnitDestroyed?.Invoke(this);
            //replace with animation before deletion soon
            Destroy(gameObject);
        }
    }

    public void Select()
    {
        cirlce.color = faction.selectedColor;
    }

    public void Deleselect()
    {
        cirlce.color = faction.passiveColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform trans = other.transform;
        Unit unit = trans.GetComponent<Unit>();
        if(unit != null)
        {
            if(!unit.faction.SameFaction(faction))
            {
                enemyUnitsInCombatRange.Add(unit);
                unit.OnUnitDestroyed += RemoveUnitFromEnemyList;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform trans = other.transform;
        Unit unit = trans.GetComponent<Unit>();
        if (unit != null) RemoveUnitFromEnemyList(unit);
    }

    private void RemoveUnitFromEnemyList(Unit unit)
    {
        if (unit == currentCombatTarget) currentCombatTarget = null;
        if (enemyUnitsInCombatRange.Contains(unit))
        {
            enemyUnitsInCombatRange.Remove(unit);
            unit.OnUnitDestroyed -= RemoveUnitFromEnemyList;
        }
    }

    private void FindCombatTarget()
    {
        if (enemyUnitsInCombatRange.Count <= 0)
        {
            currentCombatTarget = null;
            return;
        }

        float closestDist = float.MaxValue;
        for(int i = 0; i < enemyUnitsInCombatRange.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, enemyUnitsInCombatRange[i].transform.position);
            if(distance < closestDist)
            {
                closestDist = distance;
                currentCombatTarget = enemyUnitsInCombatRange[i];
            }
        }
    }

    protected virtual void CombatUpdate()
    {
        FindCombatTarget();


    }

    public Faction GetFaction() => faction;
}