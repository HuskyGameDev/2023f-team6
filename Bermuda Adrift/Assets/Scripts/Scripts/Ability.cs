using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public enum attackType { melee, projectile, buff};
    [SerializeField] private attackType type;

    [SerializeField] private GameObject meleeHitboxes;
    [SerializeField] private Bullet bullet;
    [SerializeField] private Buffs buff;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int slot;
    [SerializeField] private float cooldown;

    [SerializeField] private Sprite thumbnail;
    [SerializeField] private Sprite ModelSprite;
    [SerializeField] private string Name;
    [SerializeField] private string description;

    public attackType getAttackType() { return type; }
    public GameObject getHitboxes() { return meleeHitboxes; }
    public Bullet getBullet() { return bullet; }
    public Buffs getBuff() { return buff; }
    public GameObject getBulletPrefab() { return bulletPrefab; }
    public int getSlot() { return slot; }
    public float getCooldown() { return cooldown; }
    public Sprite getThumbnail() { return thumbnail; }
    public Sprite getModelSprite() { return ModelSprite; }
    public string getName() { return Name; }
    public string getDescription() { return description; }
}
