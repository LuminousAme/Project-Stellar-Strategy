using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class Unit : MonoBehaviour
{
    [SerializeField] protected UnitData unitData;
    [SerializeField] protected Faction faction;
    [SerializeField] private Image cirlce;

    public void Select()
    {
        cirlce.color = faction.selectedColor;
    }

    public void Deleselect()
    {
        cirlce.color = faction.passiveColor;
    }
}