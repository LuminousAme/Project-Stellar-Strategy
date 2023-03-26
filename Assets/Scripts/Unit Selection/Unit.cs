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
    [SerializeField] protected List<Cannon> cannons = new List<Cannon>();
    [SerializeField] protected GameObject model;
    [SerializeField] protected List<MultiParticle> Expolsions;

    protected int maxHealth;
    protected int currentHealth;
    protected List<Unit> enemyUnitsInCombatRange = new List<Unit>();
    protected Unit currentCombatTarget;

    public System.Action<Unit> OnUnitDestroyed;

    bool destroyed = false;
    float timeElapsedSinceDestroyed = 0.0f;
    [SerializeField] protected float destroyedTime = 2f;
    int expolsionIndex = 1;

    protected virtual void Start()
    {
        Deleselect();
        maxHealth = unitData.baseHealth;
        currentHealth = maxHealth;
        currentCombatTarget = null;
        combatRangeTrigger.radius = unitData.combatRadius;
        combatRangeTrigger.isTrigger = true;
        destroyed = false;
        for (int i = 0; i < thrusters.Count; i++)
        {
            ParticleSystem.MainModule main = thrusters[i].main; 
            main.startColor = faction.selectedColor;
        }
        for (int i = 0; i < cannons.Count; i++) cannons[i].SetColor(faction.selectedColor);
    }

    protected virtual void Update()
    {
        float t = Mathf.Clamp01((float)currentHealth / (float)maxHealth);
        healthSlider.value = t;

        if(destroyed)
        {
            t = Mathf.Clamp01(timeElapsedSinceDestroyed / destroyedTime);
            float scale = Mathf.Lerp(1.0f, 0.0f, t);
            cirlce.transform.localScale = new Vector3(scale, scale, scale);
            Image hp1 = healthSlider.transform.GetChild(0).GetComponent<Image>();
            Image hp2 = healthSlider.transform.GetChild(1).GetComponent<Image>();
            hp1.color = new Color(hp1.color.r, hp1.color.g, hp1.color.b, scale);
            hp2.color = new Color(hp2.color.r, hp2.color.g, hp2.color.b, scale);

            if (timeElapsedSinceDestroyed >= destroyedTime) Destroy(gameObject);

            float explodeTime = (0.6f * destroyedTime) / (float)Expolsions.Count;
            if (timeElapsedSinceDestroyed >= explodeTime * expolsionIndex && expolsionIndex < Expolsions.Count)
            {
                Expolsions[expolsionIndex].Play();
                expolsionIndex++;
            }

            timeElapsedSinceDestroyed += Time.deltaTime;
        }
        else
        {
            CombatUpdate();
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

    protected virtual void OnTriggerEnter(Collider other)
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

    protected virtual void OnTriggerExit(Collider other)
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
            if(enemyUnitsInCombatRange[i] == null)
            {
                enemyUnitsInCombatRange.RemoveAt(i);
                continue;
            }
            float distance = Vector3.Distance(transform.position, enemyUnitsInCombatRange[i].transform.position);
            if(distance < closestDist)
            {
                closestDist = distance;
                currentCombatTarget = enemyUnitsInCombatRange[i];
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            OnUnitDestroyed?.Invoke(this);
            currentCombatTarget = null;
            for (int i = 0; i < cannons.Count; i++) cannons[i].target = null;
            Expolsions[0].Play();
            Destroy(model);
            destroyed = true;
        }
    }

    protected virtual void CombatUpdate()
    {
        FindCombatTarget();

        for (int i = 0; i < cannons.Count; i++)
        {
            cannons[i].target = currentCombatTarget;
            cannons[i].SetOffset(i);
        }
    }

    public Faction GetFaction() => faction;
    public void SetFaction(Faction faction) => this.faction = faction;

    public bool GetInCombat() => currentCombatTarget != null;
}