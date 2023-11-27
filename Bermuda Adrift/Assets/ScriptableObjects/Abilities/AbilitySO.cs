using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilitySO", menuName = "ScriptableObjects/AbilitySO", order = 1)]
public class AbilitySO : ScriptableObject
{
    [Header("Name")]
    [SerializeField] public string name;
    [Header("Description")]
    [TextArea]
    [SerializeField] public string description;
    [SerializeField] public Sprite image;
}
