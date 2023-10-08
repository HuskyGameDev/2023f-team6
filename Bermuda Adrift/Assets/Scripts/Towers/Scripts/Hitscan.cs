using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    private int damage;
    private Camera camera;
    private void Start()
    {
        camera = Camera.main;
        damage = bullet.getDamage();
        gameObject.transform.Rotate(-gameObject.transform.rotation.eulerAngles / 2);
    }
    private void Update()
    {
        gameObject.transform.Translate(transform.up * bullet.getProjectileSpeed() * Time.deltaTime);

        if (Mathf.Abs(gameObject.transform.position.x) >= Mathf.Abs((camera.transform.position.x - camera.orthographicSize) * 2f) || Mathf.Abs(gameObject.transform.position.y) >= Mathf.Abs(camera.transform.position.y - camera.orthographicSize * 1.15f))
        {
            Destroy(gameObject);
        }
    }

    public void Mult(int mult) 
    { 
        damage *= mult;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SendMessage("TakeDamage", damage);
        }
    }
}
