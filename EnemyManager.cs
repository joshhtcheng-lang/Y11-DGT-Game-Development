using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // making a global reference so other scripts can easily talk to this enemy manager
    public static EnemyManager main;

    public Transform spawnpoint;
    public Transform[] checkpoints;

    // different types of zombie prefabs that can spawn
    [SerializeField] private GameObject zombie;
    [SerializeField] private GameObject fastZombie;
    [SerializeField] private GameObject tankZombie;

    [SerializeField] private int wave = 1;
    [SerializeField] private int enemyCount = 6;
    [SerializeField] private float enemyCountRate = 0.2f;
    [SerializeField] private float spawnDelayMax = 1f;
    [SerializeField] private float spawnDelayMin = 0.75f;

    // spawn weight percentages or ratios for each type of zombie
    [SerializeField] private float zombieRate = 0.5f;
    [SerializeField] private float fastZombieRate = 0.4f;
    [SerializeField] private float tankZombieRate = 0.1f;

    [SerializeField] private GameObject wavePanel;

    [HideInInspector] public int enemiesAlive = 0;

    private bool waveover = false;
    private bool wavedone = false;
    private List<GameObject> waveset = new List<GameObject>();

    private int zombieCount;
    private int fastZombieCount;
    private int tankZombieCount;

    private void Awake()
    {
        // ensuring only one enemy manager exists, otherwise destroy the extra one (to prevent bugs and stuff)
        if (main != null && main != this)
        {
            Destroy(gameObject);
            return;
        }

        main = this;

        // automatically finding path checkpoints in the scene if they haven't been assigned manually
        if (checkpoints == null || checkpoints.Length == 0 || checkpoints[0] == null)
        {
            GameObject pathObject = GameObject.Find("Path");
            if (pathObject != null)
            {
                checkpoints = new Transform[pathObject.transform.childCount];
                for (int i = 0; i < pathObject.transform.childCount; i++)
                {
                    checkpoints[i] = pathObject.transform.GetChild(i);
                }
            }
        }
    }

    private void Start()
    {
        // kicking off the very first wave right when the game starts
        SetWave();
    }

    private void Update()
    {
        // checking if all enemies are defeated and the wave is finished, then giving money and showing the win panel
        if (!waveover && wavedone && enemiesAlive <= 0)
        {
            if (Player.main != null)
            {
                Player.main.money += 50 + (10 * wave);
            }

            waveover = true;
            if (wavePanel != null) wavePanel.SetActive(true);
        }
    }

    private void SetWave()
    {
        wavedone = false;
        enemiesAlive = 0;

        // calculating how many of each zombie type should spawn based on rates and total count
        zombieCount = Mathf.RoundToInt(enemyCount * zombieRate);
        fastZombieCount = Mathf.RoundToInt(enemyCount * fastZombieRate);
        tankZombieCount = Mathf.RoundToInt(enemyCount * tankZombieRate);

        waveset = new List<GameObject>();

        // building the list of enemies for this wave
        AddEnemiesToWave(zombie, zombieCount);
        AddEnemiesToWave(fastZombie, fastZombieCount);
        AddEnemiesToWave(tankZombie, tankZombieCount);

        // starting the coroutine to spawn them one by one
        StartCoroutine(spawn());
    }

    // helper function to add multiple copies of a zombie prefab into our wave list
    private void AddEnemiesToWave(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            waveset.Add(prefab);
        }
    }

    // looping through the wave list to spawn enemies with a random delay between each one
    private IEnumerator spawn()
    {
        for (int i = 0; i < waveset.Count; i++)
        {
            Instantiate(waveset[i], spawnpoint.position, Quaternion.identity);
            enemiesAlive++;
            yield return new WaitForSeconds(Random.Range(spawnDelayMin, spawnDelayMax));
        }

        wavedone = true;
    }

    // triggered by the ui next wave button to increase difficulty and start the next round
    public void NextWave()
    {
        wave++;
        waveover = false;
        enemyCount += Mathf.RoundToInt(enemyCount * enemyCountRate);

        if (wavePanel != null) wavePanel.SetActive(false);
        SetWave();
    }
}