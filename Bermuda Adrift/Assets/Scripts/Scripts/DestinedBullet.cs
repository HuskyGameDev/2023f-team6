using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "destinedBullet",menuName = "ScriptableObjects/destinedBullet")]
public class DestinedBullet : ScriptableObject
{
    [SerializeField] private GameObject destinationPrefab;
    [SerializeField] private RuntimeAnimatorController bulletAnimator;
    [SerializeField] private RuntimeAnimatorController destinationAnimator;
    [SerializeField] private float AOE;
    [SerializeField] private float angle;
    [SerializeField] private float timeToLand;
    [SerializeField] private float delayTime;

    [SerializeField] private int damage;
    [SerializeField] private int scrapBonus;

    public GameObject getDestinationPrefab() { return destinationPrefab; }
    public RuntimeAnimatorController getBulletAnimator() { return bulletAnimator; }
    public RuntimeAnimatorController getDestinationAnimator() { return destinationAnimator; }
    public float getAOE() { return AOE; }
    public float getAngle() { return angle; }
    public float getTimeToLand() { return timeToLand; }
    public float getDelayTime() { return delayTime; }
    public int getDamage() { return damage; }
    public int getScrapBonus() { return scrapBonus; }
}
