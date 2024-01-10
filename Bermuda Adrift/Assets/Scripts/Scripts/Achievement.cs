using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievement")]
public class Achievement : ScriptableObject
{
    [SerializeField] private bool achieved;

    [SerializeField] private Tower associatedTower;
    [SerializeField] private BarrierScriptable associatedBarrier;
    [SerializeField] private Player associatedCharacter;

    public void unlock() { achieved = true; }


    public bool isUnlocked() { return achieved; }
    public Tower getAssociatedTower() { return associatedTower; }
    public BarrierScriptable getAssociatedBarrier() { return associatedBarrier; }
    public Player getAssociatedCharacter() { return associatedCharacter; }
}
