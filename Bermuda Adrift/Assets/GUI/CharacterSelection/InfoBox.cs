using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBox : MonoBehaviour
{
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject abilities;
    [SerializeField] private GameObject loadout;
    public void Description()
    {
        description.SetActive(true);
        abilities.SetActive(false);
        loadout.SetActive(false);
    }
    public void Abilities()
    {
        description.SetActive(false);
        abilities.SetActive(true);
        loadout.SetActive(false);
    }
    public void Loadout()
    {
        description.SetActive(false);
        abilities.SetActive(false);
        loadout.SetActive(true);
    }
}
