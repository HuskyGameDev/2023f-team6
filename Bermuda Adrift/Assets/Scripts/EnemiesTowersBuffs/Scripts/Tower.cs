using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObjects/Tower", order = 2)]
public class Tower : ScriptableObject
{
    [SerializeField] private float damageMult;
    //[SerializeField] private float fireRate;    //0 is no delay, 1 is 1 second between shots, and so on
    [SerializeField] private float turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private RuntimeAnimatorController anim;
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Bullet defaultBullet;

    [SerializeField] private int cost;
    [SerializeField] private float rarity;      //For use in the level system. Rarity of it showing up in the level up options



    //Upgrade 1
    [SerializeField] private float U1damageMult;
    //[SerializeField] private float U1fireRate;    //0 is no delay, 1 is 1 second between shots, and so on
    [SerializeField] private float U1turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float U1range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet U1Bullet;
    [SerializeField] private int U1cost;


    //Upgrade A1
    [SerializeField] private float UA1damageMult;
    //[SerializeField] private float UA1fireRate;    //0 is no delay, 1 is 1 second between shots, and so on
    [SerializeField] private float UA1turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UA1range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UA1Bullet;
    [SerializeField] private int UA1cost;

    //Upgrade A2
    [SerializeField] private float UA2damageMult;
    //[SerializeField] private float UA2fireRate;    //0 is no delay, 1 is 1 second between shots, and so on
    [SerializeField] private float UA2turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UA2range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UA2Bullet;
    [SerializeField] private int UA2cost;


    //Upgrade B1
    [SerializeField] private float UB1damageMult;
    //[SerializeField] private float UB1fireRate;    //0 is no delay, 1 is 1 second between shots, and so on
    [SerializeField] private float UB1turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UB1range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UB1Bullet;
    [SerializeField] private int UB1cost;



    //Upgrade B2
    [SerializeField] private float UB2damageMult;
    //[SerializeField] private float UB2fireRate;    //0 is no delay, 1 is 1 second between shots, and so on
    [SerializeField] private float UB2turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UB2range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UB2Bullet;
    [SerializeField] private int UB2cost;




    public float getDamageMult() { return damageMult; }
    //public float getFireRate() { return fireRate; }
    public float getTurnSpeed() { return turnSpeed; }
    public float getRange() { return range; }
    public RuntimeAnimatorController getAnim() { return anim; }
    public Sprite getBaseSprite() { return baseSprite; }
    public Bullet getDefaultBullet() { return defaultBullet; }

    public int getCost() { return cost; }
    public float getRarity() { return rarity; }

    //Upgrade 1
    public float U1getDamageMult() { return U1damageMult; }
    //public float U1getFireRate() { return U1fireRate; }
    public float U1getTurnSpeed() { return U1turnSpeed; }
    public float U1getRange() { return U1range; }
    public Bullet U1getBullet() { return U1Bullet; }
    public int U1getCost() { return U1cost; }

    //Upgrade A1
    public float UA1getDamageMult() { return UA1damageMult; }
    //public float UA1getFireRate() { return UA1fireRate; }
    public float UA1getTurnSpeed() { return UA1turnSpeed; }
    public float UA1getRange() { return UA1range; }
    public Bullet UA1getBullet() { return UA1Bullet; }
    public int UA1getCost() { return UA1cost; }

    //Upgrade A2
    public float UA2getDamageMult() { return UA2damageMult; }
    //public float UA2getFireRate() { return UA2fireRate; }
    public float UA2getTurnSpeed() { return UA2turnSpeed; }
    public float UA2getRange() { return UA2range; }
    public Bullet UA2getBullet() { return UA2Bullet; }
    public int UA2getCost() { return UA2cost; }

    //Upgrade B1
    public float UB1getDamageMult() { return UB1damageMult; }
    //public float UB1getFireRate() { return UB1fireRate; }
    public float UB1getTurnSpeed() { return UB1turnSpeed; }
    public float UB1getRange() { return UB1range; }
    public Bullet UB1getBullet() { return UB1Bullet; }
    public int UB1getCost() { return UB1cost; }

    //Upgrade B2
    public float UB2getDamageMult() { return UB2damageMult; }
    //public float UB2getFireRate() { return UB2fireRate; }
    public float UB2getTurnSpeed() { return UB2turnSpeed; }
    public float UB2getRange() { return UB2range; }
    public Bullet UB2getBullet() { return UB2Bullet; }
    public int UB2getCost() { return UB2cost; }
}
