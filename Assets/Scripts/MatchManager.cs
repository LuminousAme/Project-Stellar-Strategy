using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public Faction playerFaction;
    public int intesnityLevelCutoff = 5;
    int numInCombat = 0;
    int desiredIntensity = 0;

    bool inCombat = false;
    bool acutalInCombat = false;
    float timeSinceCombatStatusChanged = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.FadeTracksIn(1, int.MaxValue, 5f);
        inCombat = false;
        acutalInCombat = false;
    }

    private void Update()
    {
        UpdateMusic();
    }

    void UpdateMusic()
    {
        numInCombat = 0;

        Unit[] units = FindObjectsOfType<Unit>();
        for (int i = 0; i < units.Length; i++)
        {
            Unit unit = units[i];
            if (!playerFaction.SameFaction(unit.GetFaction())) continue;

            if (unit.GetInCombat()) numInCombat++;
        }

        desiredIntensity = numInCombat / intesnityLevelCutoff;
        
        if(numInCombat > 0 && !inCombat)
        {
            inCombat = true;
            timeSinceCombatStatusChanged = 0.0f;
        }
        if(numInCombat > 0 && inCombat && !acutalInCombat && timeSinceCombatStatusChanged > 0.2f)
        {
            acutalInCombat = true;
            if (MusicManager.instance.CurrentIndex() != 2) MusicManager.instance.FadeTracksIn(2, 0, 5f);
        }

        if(numInCombat == 0 && inCombat)
        {
            inCombat = false;
            timeSinceCombatStatusChanged = 0.0f;
        }
        if (numInCombat == 0 && !inCombat && acutalInCombat && timeSinceCombatStatusChanged > 1.0f)
        {
            acutalInCombat = false;
            if (MusicManager.instance.CurrentIndex() == 2) MusicManager.instance.FadeTracksIn(1, 0, 5f);
        }

        if (MusicManager.instance.CurrentIndex() == 2) MusicManager.instance.SetCurrentIntensity(desiredIntensity);

        timeSinceCombatStatusChanged += Time.deltaTime;
    }
}
