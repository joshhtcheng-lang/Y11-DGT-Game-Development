using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    [SerializeField] private Tower Tower;
    private List<GameObject> targets = new List<GameObject>();

    void Start()
    {
        // updating my range scale right when the game starts based on the tower stats
        UpdateRange();
    }

    void Update()
    {
        // cleaning up my enemy list by removing any targets that died or disappeared
        targets.RemoveAll(enemy => enemy == null);

        // checking if there are any enemies in range to target
        if (targets.Count > 0)
        {
            // targeting the enemy furthest along the path if first mode is selected
            if (Tower.first)
            {
                float minDistance = Mathf.Infinity;
                int maxIndex = 0;
                GameObject firstTarget = null;

                foreach (GameObject target in targets)
                {
                    int index = target.GetComponent<Enemy>().index;
                    float distance = target.GetComponent<Enemy>().distance;

                    if (index > maxIndex || (index == maxIndex && distance < minDistance))
                    {
                        maxIndex = index;
                        minDistance = distance;
                        firstTarget = target;
                    }

                }

                Tower.target = firstTarget;
            }
            // targeting the enemy closest to the start if last mode is selected
            else if (Tower.last)
            {
                float maxDistance = -Mathf.Infinity;
                int minIndex = int.MaxValue;
                GameObject lastTarget = null;

                foreach (GameObject target in targets)
                {
                    int index = target.GetComponent<Enemy>().index;
                    float distance = target.GetComponent<Enemy>().distance;

                    if (index < minIndex || (index == minIndex && distance > maxDistance))
                    {
                        minIndex = index;
                        maxDistance = distance;
                        lastTarget = target;
                    }
                }

                Tower.target = lastTarget;
            }
            // targeting the enemy with the most health if strong mode is selected
            else if (Tower.strong)
            {
                GameObject strongestTarget = null;
                float maxHealth = 0;

                foreach (GameObject target in targets)
                {
                    float health = target.GetComponent<Enemy>().health;

                    if (health > maxHealth)
                    {
                        maxHealth = health;
                        strongestTarget = target;
                    }
                }
                Tower.target = strongestTarget;
            }
            // default fallback to targeting the very first enemy that entered the range
            else
            {
                Tower.target = targets[0];
            }

        }
        else
        {
            // clearing my target if nobody is in range
            Tower.target = null;
        }
    }

    // adding an enemy to my list when they step inside the tower range circle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            targets.Add(collision.gameObject);
        }
    }

    // removing an enemy from my list when they walk out of the tower range circle
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            targets.Remove(collision.gameObject);
        }
    }

    // resizing the range circle object in the game world to match the tower stats
    public void UpdateRange()
    {
        transform.localScale = new Vector3(Tower.range, Tower.range, Tower.range);
    }
}