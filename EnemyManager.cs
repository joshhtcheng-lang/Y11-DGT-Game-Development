using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //EnemyManager and checkpoint setup to be accessible in the inspector and by other scripts
    public static EnemyManager main;
    public Transform spawnpoint;
    public Transform[] checkpoints;

    //Enemy prefabs in Unity
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject fastEnemy;
    [SerializeField] private GameObject tankEnemy;
    [SerializeField] private GameObject bossEnemy;

    // Wave system variables
    [SerializeField] private int wave = 1;
    [SerializeField] private int enemyCount = 6;
    [SerializeField] private float enemyCountRate = 1.2f;
    [SerializeField] private float spawnDelayMax = 1f;
    [SerializeField] private float spawnDelayMin = 0.75f;
    

    [SerializeField] private float zombieRate = 0.5f;
    [SerializeField] private float fastZombieRate = 0.4f;
    [SerializeField] private float tankZombieRate = 0.1f;
    [SerializeField] private float bossZombieRate = 0.05f;


    private bool wavedone = false;
    private List<GameObject> waveset = new List<GameObject>();
    private int enemyLeft;

    private int zombieCount;
    private int fastZombieCount;
    private int tankZombieCount;
    private int bossZombieCount;

    // Checks if there are multiple EnemyManager and destroys any duplicates
    void Awake()
    {
        main = this;
    }

    // Starts the first wave of enemies
    void Start()
    {
        SetWave();
    }
    
    // Keeps track of the number of enemies alive using the variable "activeEnemies" and will start next wave when all enemies are dead
    private int activeEnemies = 0;

    public void EnemyDestroyed()
    {activeEnemies--;

        if (activeEnemies <= 0 && wavedone)
        {
            wave++;
            enemyCount = Mathf.RoundToInt(enemyCount * enemyCountRate);
            SetWave();
        }
    }



    // Increases the number of enemies per wave based on the wave number 
    private void SetWave()
    {
        zombieCount = Mathf.RoundToInt(enemyCount * zombieRate);
        fastZombieCount = Mathf.RoundToInt(enemyCount * fastZombieRate);
        tankZombieCount = Mathf.RoundToInt(enemyCount * tankZombieRate);
        bossZombieCount = Mathf.RoundToInt(enemyCount * bossZombieRate);

        enemyLeft = zombieCount + fastZombieCount + tankZombieCount + bossZombieCount;
        enemyCount = enemyLeft;
        waveset = new List<GameObject>();

        for (int i = 0; i < zombieCount; i++)
        {
            waveset.Add(enemy);
        }

        for (int i = 0; i < fastZombieCount; i++)
        {
            waveset.Add(fastEnemy);
        }

        for (int i = 0; i < tankZombieCount; i++)
        {
            waveset.Add(tankEnemy);
        }

        for (int i = 0; i < bossZombieCount; i++)
        {
            waveset.Add(bossEnemy);
        }

        wavedone = false;
        StartCoroutine(Spawn());
    }

    // Adds a random delay between spawning each enemy
    IEnumerator Spawn()
    {
        for (int i = 0; i < waveset.Count; i++)
        {
            Instantiate(waveset[i], spawnpoint.position, Quaternion.identity);
            activeEnemies++;
            yield return new WaitForSeconds(Random.Range(spawnDelayMin, spawnDelayMax));
        }

        wavedone = true;


        // Failsafe so that if there are no active enemies on the map, the next wave will start anyway
        if (activeEnemies == 0)
        {
            wave ++;
            enemyCount = Mathf.RoundToInt(enemyCount * enemyCountRate);
            SetWave();
        }


    }
   

}
