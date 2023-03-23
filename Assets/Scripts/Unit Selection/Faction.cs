using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "CelestialConquest/Faction", order = 2)]
public class Faction : ScriptableObject
{
    [SerializeField]
    private string m_factionName = "faction";
    public string factionName { get{ return m_factionName; } }

    [SerializeField]
    private Color m_selectColor = Color.green;
    public Color selectedColor { get { return m_selectColor; } }

    [SerializeField]
    private Color m_passiveColor = Color.white;
    public Color passiveColor { get { return m_passiveColor; } }

    public bool SameFaction(Faction otherFaction)
    {
        return m_factionName == otherFaction.m_factionName;
    }
}
