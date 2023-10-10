using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Bullet", order = 2)]
public class Bullet : ScriptableObject
{
    [SerializeField] private int projectileSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float AOE; //radius of the aoe from the bullet landing. 0 to only damage enemies that were directly hit
    [SerializeField] private int effect;
    [SerializeField] private float scale;
    [SerializeField] private int timer;
    [SerializeField] private Sprite sprite;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private bool FriendlyFire;

    public int getProjectileSpeed() { return projectileSpeed; }
    public int getDamage() { return damage; }
    public float getAOE() { return AOE; }
    public int getEffect() { return effect; }
    public float getScale() { return scale; }
    public int getTimer() { return timer; }
    public Sprite getSprite() { return sprite; }
    public RuntimeAnimatorController getAnimator() { return animator; }
    public bool getFriendlyFire() { return FriendlyFire; }
}
