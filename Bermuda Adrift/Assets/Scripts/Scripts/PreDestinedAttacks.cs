using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreDestinedAttacks : MonoBehaviour
{
    private DestinedBullet incoming;

    private GameObject destination = null;
    private Vector3 endPoint = Vector3.zero;
    private bool falling = false;
    private float timeLeft;
    private float xMove;
    private float yMove;
    private float damageMult;
    private float underwaterMult;
    private float airborneMult;
    private float critChance;
    private float levelScale = 1;

    private void setBullet(DestinedBullet bullet)
    {
        incoming = bullet;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 1f;
        destination = Instantiate(incoming.getDestinationPrefab(), mousePosition, Quaternion.identity);
        destination.GetComponent<Animator>().runtimeAnimatorController = incoming.getDestinationAnimator();
        destination.transform.localScale = new Vector3(incoming.getAOE(), incoming.getAOE());
        endPoint = destination.transform.position;

        gameObject.GetComponent<Animator>().runtimeAnimatorController = incoming.getBulletAnimator();

        yMove = -((Camera.main.transform.position.y - Camera.main.orthographicSize) * 1.15f);   //Upper bound of where the enemies spawn from
        xMove = yMove * Mathf.Tan(incoming.getAngle());

        gameObject.GetComponent<SpriteRenderer>().flipX = true;
        if (destination.transform.position.x < 0)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            xMove *= -1;
        }
        gameObject.transform.position = new Vector3(xMove * 2, yMove * 2, 1);
        timeLeft = incoming.getTimeToLand();

        StartCoroutine(fallDelay());
    }

    private void Update()
    {
        if (falling)
        {
            timeLeft -= Time.deltaTime;
            move();
        }
        if (timeLeft <= 0)
            landed();
    }

    private void move()
    {
        float percentage = timeLeft / incoming.getTimeToLand();
        gameObject.transform.position = endPoint + new Vector3(xMove * percentage, yMove * percentage);
    }

    private void landed()
    {
        gameObject.transform.position = destination.transform.position;
        falling = false;
        gameObject.GetComponent<Animator>().SetTrigger("Landed");
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }
    private IEnumerator fallDelay()
    {
        yield return new WaitForSeconds(incoming.getDelayTime());
        falling = true;
    }
    public void destroyObject()
    {
        GameObject.Find("Managers").GetComponent<GameManager>().addScrap(incoming.getScrapBonus());
        Destroy(destination);
        Destroy(gameObject);
    }
    private int critCalc()
    {
        if (Random.Range(0, 1f) < (critChance * 0.1))
        {
            return 2;
        }

        return 1;
    }
    private void Mult(float mult) { damageMult = mult; }
    private void setCritChance(float chance) { critChance = chance; }
    private void UnderwaterMult(float mult) { underwaterMult = mult; }
    private void AirborneMult(float mult) { airborneMult = mult; }
    private void setLevelScale(float scale) { levelScale = scale; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) 
            return;

        int crit = critCalc();

        if (crit > 1)
        {
            if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)
                collision.gameObject.SendMessage("CritDamage", incoming.getDamage() * damageMult * underwaterMult * crit * levelScale);

            else if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)
                collision.gameObject.SendMessage("CritDamage", incoming.getDamage() * damageMult * airborneMult * crit * levelScale);

            else
                collision.gameObject.SendMessage("CritDamage", incoming.getDamage() * damageMult);
        }
        else
        {
            if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Underwater || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.WaterBoss)
                collision.gameObject.SendMessage("TakeDamage", incoming.getDamage() * damageMult * underwaterMult * levelScale);

            else if (collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.Airborne || collision.gameObject.GetComponent<AI>().getType() == Enemy.Types.AirborneBoss)
                collision.gameObject.SendMessage("TakeDamage", incoming.getDamage() * damageMult * airborneMult * levelScale);

            else
                collision.gameObject.SendMessage("TakeDamage", incoming.getDamage() * damageMult);
        }
    }

}
