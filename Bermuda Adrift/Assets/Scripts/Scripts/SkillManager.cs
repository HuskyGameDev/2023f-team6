using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Skill[] skills;
    
    private int totalSkillPoints;

    public float CenterpieceHealth { get; private set; }
    public float MovementSpeed { get; private set; }
    public float CharacterDamage { get; private set; }
    public float CharacterCooldowns { get; private set; }
    public float IslandChance { get; private set; }
    public float ScrapMultiplier { get; private set; }
    public float CritChance { get; private set; }
    public float BarrierHealth { get; private set; }
    public float CheaperTowers { get; private set; }

    public bool BarrierReflect { get; private set; }
    public bool TwoBlueprints { get; private set; }
    public bool RandomBlueprintStart { get; private set; }
    private void OnEnable()
    {
        CenterpieceHealth = getSkill("Centerpiece Health Up").getUnlocked() ? getSkill("Centerpiece Health Up").getEffectStrength() : 1;
        MovementSpeed = getSkill("Movement Speed Up").getUnlocked() ? getSkill("Movement Speed Up").getEffectStrength() : 1;
        CharacterDamage = getSkill("Player Damage Up").getUnlocked() ? getSkill("Player Damage Up").getEffectStrength() : 1;
        CharacterCooldowns = getSkill("Shorter Cooldowns").getUnlocked() ? getSkill("Shorter Cooldowns").getEffectStrength() : 1;
        IslandChance = getSkill("Island Chance Up").getUnlocked() ? getSkill("Island Chance Up").getEffectStrength() : 1;
        ScrapMultiplier = getSkill("More Scrap").getUnlocked() ? getSkill("More Scrap").getEffectStrength() : 1;
        CritChance = getSkill("Crit Chance Up").getUnlocked() ? getSkill("Crit Chance Up").getEffectStrength() : 1;
        BarrierHealth = getSkill("Barrier Health Up").getUnlocked() ? getSkill("Barrier Health Up").getEffectStrength() : 1;
        CheaperTowers = getSkill("Cheaper Towers").getUnlocked() ? getSkill("Cheaper Towers").getEffectStrength() : 1;


        BarrierReflect = getSkill("Barrier Damage Reflect").getUnlocked();
        TwoBlueprints = getSkill("Chance For 2 Blueprints").getUnlocked();
        RandomBlueprintStart = getSkill("Start With a Random Blueprint").getUnlocked();
    }
    Skill getSkill(string skillName)
    {
        foreach (Skill skill in skills)
        {
            if (skill.getTitleText().CompareTo(skillName) == 0)
                return skill;
        }

        Debug.Log("No match in getSkill for " + skillName);
        return null;
    }
    public void buySkill(Skill skill)
    {
        if (totalSkillPoints < skill.getCost())
            return;

        totalSkillPoints -= skill.getCost();
        skill.setUnlocked(true);
    }

    public void addPoints(int points) { totalSkillPoints += points; }

    public void LoadData(S_O_Saving saver)
    {
        totalSkillPoints = saver.totalPoints;

        foreach (Skill skill in skills)
        {
            skill.setUnlocked(saver.getSkillSave(skill.getTitleText()).unlocked);
        }
    }

    public void SaveData(S_O_Saving saver)
    {
        saver.totalPoints = totalSkillPoints;

        foreach (Skill skill in skills)
        {
            saver.getSkillSave(skill.getTitleText()).unlocked = skill.getUnlocked();
        }
    }
}
