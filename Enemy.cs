using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Setting up base stats like health, movement speed, and money reward when defeated
    public int health = 50;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int value = 10;
    private Rigidbody2D rb;
    private Transform checkpoint;
    [NonSerialized] public int index = 0;
    [NonSerialized] public float distance = 0;

    private bool isDead = false;

    private void Awake()
    {
        // grabbing my rigidbody component for physics movement
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // locking onto the very first checkpoint in the path when spawning in
        if (EnemyManager.main != null && EnemyManager.main.checkpoints != null && EnemyManager.main.checkpoints.Length > 0)
        {
            checkpoint = EnemyManager.main.checkpoints[0];
        }
    }

    private void Update()
    {
        // Stopping everything if this enemy is already dead
        if (isDead) return;

        // finding the next checkpoint if we lost track of it
        if (checkpoint == null)
        {
            if (EnemyManager.main != null && EnemyManager.main.checkpoints != null && EnemyManager.main.checkpoints.Length > 0)
            {
                checkpoint = EnemyManager.main.checkpoints[index];
                distance = Vector2.Distance(transform.position, EnemyManager.main.checkpoints[index].position);
            }
            return;
        }

        // checking if we got close enough to the current checkpoint to move to the next one
        if (Vector2.Distance(transform.position, checkpoint.position) <= 0.3f)
        {
            index++;

            // if we passed the last checkpoint, deal damage to the player and destroy this enemy
            if (index >= EnemyManager.main.checkpoints.Length)
            {
                if (Player.main != null)
                {
                    Player.main.damage(health);
                }

                isDead = true;
                EnemyManager.main.enemiesAlive--;
                Destroy(gameObject);
                return;
            }

            checkpoint = EnemyManager.main.checkpoints[index];
        }
    }

    private void FixedUpdate()
    {
        // moving smoothly towards the target checkpoint using physics velocity
        if (checkpoint == null || isDead) return;

        Vector2 direction = (checkpoint.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    // taking damage when hit by a tower, and dying if health drops to zero or below
    public void damage(int damageAmount)
    {
        if (isDead) return;

        health -= damageAmount;

        // if health hits zero, give reward money to the player, lower alive count, and destroy object
        if (health <= 0)
        {
            Player.main.money += value;
            isDead = true;
            if (EnemyManager.main != null)
            {
                EnemyManager.main.enemiesAlive--;
            }
            Destroy(gameObject);
        }
    }
}