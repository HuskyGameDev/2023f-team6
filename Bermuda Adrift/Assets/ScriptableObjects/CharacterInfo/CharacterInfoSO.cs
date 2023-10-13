using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "ScriptableObjects/CharacterInfo", order = 1)]
public class CharacterInfo : ScriptableObject
{
    [Header("Name")]
    [SerializeField] string name;
    [Header("Description")]
    [TextArea]
    [SerializeField] string description;
    [Header("Abilities")]
    [SerializeField] ScriptableObject[] abilities;
    [Header("Loadout")]
    [SerializeField] GameObject[] loadout;
}
