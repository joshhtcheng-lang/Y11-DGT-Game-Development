using System;
using UnityEngine;

public class TowerUpgrades : MonoBehaviour
{
    // setting up a blueprint class for my tower levels, storing stats like range, damage, speed, and cost
    [System.Serializable]
    class Level
    {
        public float range = 8f;
        public int damage = 25;
        public float fireRate = 1f;
        public int cost = 100;
    }

    // creating my array of 3 upgrade tiers for the tower
    [SerializeField] private Level[] levels = new Level[3];
    [NonSerialized] public int currentlevel = 0;
    [NonSerialized] public string currentCost;

    private Tower tower;
    [SerializeField] private TowerRange towerRange;

    void Awake()
    {
        // grabbing my tower component and setting my starting upgrade cost text
        tower = GetComponent<Tower>();
        currentCost = levels[0].cost.ToString();
    }

    public void Upgrade()
    {
        // checking if i have enough upgrades left and if the player's money can afford this tier
        if (currentlevel < levels.Length && levels[currentlevel].cost < Player.main.money)
        {
            // applying the new stats from the current level tier to my tower
            tower.range = levels[currentlevel].range;
            tower.damage = levels[currentlevel].damage;
            tower.fireRate = levels[currentlevel].fireRate;
            towerRange.UpdateRange();

            // deducting the upgrade cost from the player's total money
            Player.main.money -= levels[currentlevel].cost;

            // moving up to the next level index
            currentlevel++;

            // if i hit or exceed the max levels, show "max", otherwise grab the next cost
            if (currentlevel >= levels.Length)
            {
                currentCost = "MAX";
            }
            else
            {
                currentCost = levels[currentlevel].cost.ToString();
            }
        }
    }
}