using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy move speed, made to be changed in the inspector
    [SerializeField] private float movespeed = 2f;

    // Rigidbody2D component
    private Rigidbody2D rb;

    // variable to store checkpoints 
    private Transform checkpoint;

    // variable named index to keep track of the current checkpoint
    private int index = 0;

    // finds rigidbody2d component and stores it in the variable "rb"
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Checks if there is a EnemyManager gameobject and if there are any checkpoints set up. If not it will log a warning message.
    void Start()
    {
        if (EnemyManager.main != null && EnemyManager.main.checkpoints.Length <= 0)
        {
            Debug.LogWarning("EnemyManager or Checkpoints are not set up correctly.");
        }

        checkpoint = EnemyManager.main.checkpoints[index];
    }
    
   
    void Update()
    {
         // If something is wrong with enemymanager gameobject or checkpoints, return and not run the rest of the code.
        if (EnemyManager.main == null || EnemyManager.main.checkpoints == null || index >= EnemyManager.main.checkpoints.Length) return;

        // If the enemy is close enough to the current checkpoint, increase the index by one to move to the next one.
        if (Vector2.Distance(checkpoint.position, transform.position) < 0.1f)
        {
            index++;

            // if the index (current checkpoint) is greater than or equal to the total number of checkpoints, destroy the enemy game object.
            if (index >= EnemyManager.main.checkpoints.Length)
            {
                if (EnemyManager.main != null)
                {
                    EnemyManager.main.EnemyDestroyed();
                }

                Destroy(gameObject);
                return;
            }

            // if the there are still checkpoints on the list left, set the next checkpoint to the next one in the list.
            checkpoint = EnemyManager.main.checkpoints[index];
        }
       
    }

    // Moves the enemy towards the current checkpoint with the ridgidbody2d component and the movespeed variable.
    void FixedUpdate()
    {
        if (checkpoint == null) return;
        Vector2 direction = (checkpoint.position - transform.position).normalized;
        rb.linearVelocity = direction * movespeed;
    }
    
}
