using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableDatabase", menuName = "ScriptableObjects/database")]
public class PlaceableDatabase : ScriptableObject
{
    [SerializeField] private string[] saveStrings;
    [SerializeField] private ScriptableObject[] matchObjects;

    public ScriptableObject getMatchingObject(string saveString)
    {
        for (int i = 0; i < saveStrings.Length; i++)
        {
            if (saveStrings[i].CompareTo(saveString) == 0)
                return matchObjects[i];
        }
        Debug.Log(saveString + " is not in database");
        return null;
    }
}
