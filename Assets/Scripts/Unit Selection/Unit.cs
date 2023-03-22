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

    protected int maxHealth;
    protected int currentHealth;

    protected virtual void Start()
    {
        Deleselect();
        maxHealth = unitData.baseHealth;
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        float t = Mathf.Clamp01((float)currentHealth / (float)maxHealth);
        healthSlider.value = t;
    }

    public void Select()
    {
        cirlce.color = faction.selectedColor;
    }

    public void Deleselect()
    {
        cirlce.color = faction.passiveColor;
    }

    public Faction GetFaction() => faction;
}