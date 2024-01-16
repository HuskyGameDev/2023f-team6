using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Tower", menuName = "ScriptableObjects/Tower", order = 2)]
public class Tower : ScriptableObject
{
    [SerializeField] private string saveString;

    [Header("Base Form")]
    [SerializeField] private string Name;
    [SerializeField] private string Description;
    [SerializeField] private float damageMult;
    [SerializeField] private float turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet defaultBullet;
    [SerializeField] private int cost;
    [SerializeField] private int lightningResistance;
    [SerializeField] private Sprite image;



    //Upgrade 1
    [Header("Upgrade 1")]
    [SerializeField] private string U1Name;
    [SerializeField] private string U1Description;
    [SerializeField] private float U1damageMult;
    [SerializeField] private float U1turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float U1range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet U1Bullet;
    [SerializeField] private int U1cost;
    [SerializeField] private int U1LightningResistance;
    [SerializeField] private Sprite U1image;


    //Upgrade A1
    [Header("Upgrade A1")]
    [SerializeField] private string UA1Name;
    [SerializeField] private string UA1Description;
    [SerializeField] private float UA1damageMult;
    [SerializeField] private float UA1turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UA1range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UA1Bullet;
    [SerializeField] private int UA1cost;
    [SerializeField] private int UA1LightningResistance;
    [SerializeField] private Sprite UA1image;

    //Upgrade A2
    [Header("Upgrade A2")]
    [SerializeField] private string UA2Name;
    [SerializeField] private string UA2Description;
    [SerializeField] private float UA2damageMult;
    [SerializeField] private float UA2turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UA2range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UA2Bullet;
    [SerializeField] private int UA2cost;
    [SerializeField] private int UA2LightningResistance;
    [SerializeField] private Sprite UA2image;


    //Upgrade B1
    [Header("Upgrade B1")]
    [SerializeField] private string UB1Name;
    [SerializeField] private string UB1Description;
    [SerializeField] private float UB1damageMult;
    [SerializeField] private float UB1turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UB1range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UB1Bullet;
    [SerializeField] private int UB1cost;
    [SerializeField] private int UB1LightningResistance;
    [SerializeField] private Sprite UB1image;



    //Upgrade B2
    [Header("Upgrade B2")]
    [SerializeField] private string UB2Name;
    [SerializeField] private string UB2Description;
    [SerializeField] private float UB2damageMult;
    [SerializeField] private float UB2turnSpeed;   //The closer to 0, the slower the turn speed. Might add a multiplier somewhere so we don't end up working with stuff like .00001 for this variable
    [SerializeField] private float UB2range;       //Use tiles as a measure of range? -1 for infinite range
    [SerializeField] private Bullet UB2Bullet;
    [SerializeField] private int UB2cost;
    [SerializeField] private int UB2LightningResistance;
    [SerializeField] private Sprite UB2image;

    [Header("Misc")]
    [SerializeField] private RuntimeAnimatorController anim;
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private float rarity;      //For use in the level system. Rarity of it showing up in the level up
    [SerializeField] private bool cantTurn;
    [SerializeField] private TowerAI.Priority[] extraPriorities;


    public string getSaveString() { return saveString; }

    public float getDamageMult() { return damageMult; }
    //public float getFireRate() { return fireRate; }
    public float getTurnSpeed() { return turnSpeed; }
    public float getRange() { return range; }
    public RuntimeAnimatorController getAnim() { return anim; }
    public Sprite getBaseSprite() { return baseSprite; }
    public TowerAI.Priority[] getExtraPriorities() { return extraPriorities; }


    public Bullet getDefaultBullet() { return defaultBullet; }
    public int getCost() { return cost; }
    public int getLightningResistance() { return lightningResistance; }
    public float getRarity() { return rarity; }
    public string getName() { return Name; }
    public string getDescription() { return Description; }
    public Sprite getImage() { return image; }
    public bool getCantTurn() { return cantTurn; }

    //Upgrade 1
    public float U1getDamageMult() { return U1damageMult; }
    //public float U1getFireRate() { return U1fireRate; }
    public float U1getTurnSpeed() { return U1turnSpeed; }
    public float U1getRange() { return U1range; }
    public Bullet U1getBullet() { return U1Bullet; }
    public int U1getCost() { return U1cost; }
    public int U1getLightningResistance() { return U1LightningResistance; }
    public string U1getName() { return U1Name; }
    public string U1getDescription() { return U1Description; }
    public Sprite U1getImage() { return U1image; }

    //Upgrade A1
    public float UA1getDamageMult() { return UA1damageMult; }
    //public float UA1getFireRate() { return UA1fireRate; }
    public float UA1getTurnSpeed() { return UA1turnSpeed; }
    public float UA1getRange() { return UA1range; }
    public Bullet UA1getBullet() { return UA1Bullet; }
    public int UA1getCost() { return UA1cost; }
    public int UA1getLightningResistance() { return UA1LightningResistance; }
    public string UA1getName() { return UA1Name; }
    public string UA1getDescription() { return UA1Description; }
    public Sprite UA1getImage() { return UA1image; }

    //Upgrade A2
    public float UA2getDamageMult() { return UA2damageMult; }
    //public float UA2getFireRate() { return UA2fireRate; }
    public float UA2getTurnSpeed() { return UA2turnSpeed; }
    public float UA2getRange() { return UA2range; }
    public Bullet UA2getBullet() { return UA2Bullet; }
    public int UA2getCost() { return UA2cost; }
    public int UA2getLightningResistance() { return UA2LightningResistance; }
    public string UA2getName() { return UA2Name; }
    public string UA2getDescription() { return UA2Description; }
    public Sprite UA2getImage() { return UA2image; }

    //Upgrade B1
    public float UB1getDamageMult() { return UB1damageMult; }
    //public float UB1getFireRate() { return UB1fireRate; }
    public float UB1getTurnSpeed() { return UB1turnSpeed; }
    public float UB1getRange() { return UB1range; }
    public Bullet UB1getBullet() { return UB1Bullet; }
    public int UB1getCost() { return UB1cost; }
    public int UB1getLightningResistance() { return UB1LightningResistance; }
    public string UB1getName() { return UB1Name; }
    public string UB1getDescription() { return UB1Description; }
    public Sprite UB1getImage() { return UB1image; }

    //Upgrade B2
    public float UB2getDamageMult() { return UB2damageMult; }
    //public float UB2getFireRate() { return UB2fireRate; }
    public float UB2getTurnSpeed() { return UB2turnSpeed; }
    public float UB2getRange() { return UB2range; }
    public Bullet UB2getBullet() { return UB2Bullet; }
    public int UB2getCost() { return UB2cost; }
    public int UB2getLightningResistance() { return UB2LightningResistance; }
    public string UB2getName() { return UB2Name; }
    public string UB2getDescription() { return UB2Description; }
    public Sprite UB2getImage() { return UB2image; }
}
