using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private float _lifeTime = 10f; // time until despawn
    private float _currentTime = 0;
    private GameObject Player;

    public float BulletSpeed;
    public Vector2 Force; // force applied to objects hit
    public int Damage;

    public void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > _lifeTime)
            Destroy(gameObject);
    }
    public void Shoot(GameObject player)
    {
        Player = player;
        Vector3 direction = Player.transform.position - transform.position;
        GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y).normalized * BulletSpeed;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 direction = Player.transform.position - transform.position;
            Vector2 force = new Vector2((direction.x > 0 ? 1 : -1) * Math.Abs(Force.x), Force.y);
            Player.GetComponent<PlayerController>().TakeDamage(Damage, force);
            Destroy(gameObject);
        }
    }
}
