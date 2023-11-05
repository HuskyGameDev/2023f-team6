using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "ScriptableObjects/CharacterInfo", order = 1)]
public class CharacterInfoSO : ScriptableObject
{
    [Header("Name")]
    [SerializeField] public string charName;
    [Header("Description")]
    [TextArea]
    [SerializeField]public string description;
    [Header("Abilities")]
    [SerializeField] public AbilitySO[] abilities;
    [Header("Loadout")]
    [SerializeField] public GameObject[] loadout;
}
