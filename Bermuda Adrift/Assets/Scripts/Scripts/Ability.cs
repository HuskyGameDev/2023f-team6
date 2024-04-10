using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public enum attackType { melee, projectile, destinedProjectile, targetedAttack, tempTower, buff};
    [SerializeField] private attackType type;

    [SerializeField] private GameObject meleeHitboxes;

    [SerializeField] private Bullet bullet;

    [SerializeField] private DestinedBullet destinedBullet;

    [SerializeField] private GameObject targetedAttacker;

    [SerializeField] private Tower tempTower;
    [SerializeField] private float towerDuration;

    [SerializeField] private Buffs buff;


    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int slot;
    [SerializeField] private float cooldown;

    [SerializeField] private Sprite thumbnail;
    [SerializeField] private Sprite ModelSprite;
    [SerializeField] private string Name;
    [SerializeField] private string description;
    [SerializeField] private bool looped;
    [SerializeField] private bool lockedDirection;
    [SerializeField] private string audioString;
    [SerializeField] private string[] arrayAudioString;

    public attackType getAttackType() { return type; }
    public GameObject getHitboxes() { return meleeHitboxes; }
    public Bullet getBullet() { return bullet; }
    public DestinedBullet getDestinedBullet() { return destinedBullet; }
    public GameObject getTargetedAttacker() { return targetedAttacker; }
    public Tower getTempTower() { return tempTower; }
    public float getTowerDuration() { return towerDuration; }
    public Buffs getBuff() { return buff; }
    public GameObject getBulletPrefab() { return bulletPrefab; }
    public int getSlot() { return slot; }
    public float getCooldown() { return cooldown; }
    public Sprite getThumbnail() { return thumbnail; }
    public Sprite getModelSprite() { return ModelSprite; }
    public string getName() { return Name; }
    public string getDescription() { return description; }
    public bool canBeLooped() { return looped; }
    public bool directionLocked() { return lockedDirection; }
    public string getAudioString() { return audioString; }
    public string getArrayAudioString() 
    {
        if (arrayAudioString.Length > 0)
            return arrayAudioString[Random.Range(0, arrayAudioString.Length)];
        else
            return "";
    }
}
