using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // setting up base stats like range, damage, fire rate, and cost for my tower
    [Header("Tower Stats")]
    public float range = 8f;
    public int damage = 25;
    public float fireRate = 1f;
    public int cost = 50;

    // toggles to choose how the tower picks its target
    [Header("Targeting Mode")]
    public bool first = true;
    public bool last = false;
    public bool strong = false;

    // visual effect that plays when the tower attacks
    [Header("Effects")]
    [SerializeField] GameObject fireEffect;

    [NonSerialized]
    public GameObject target;
    private float cooldown = 0f;

    void Start()
    {

    }

    void Update()
    {
        // checking if i have a valid target to shoot at
        if (target != null)
        {
            // if my attack cooldown is ready, deal damage, reset timer, and trigger the effect
            if (cooldown >= fireRate)
            {
                target.GetComponent<Enemy>().damage(damage);
                cooldown = 0f;
                StartCoroutine(FireEffect());
            }

            // otherwise, keep counting up time until the next shot
            else
            {
                cooldown += 1 * Time.deltaTime;
            }
            // 1. getting the direction pointing from the tower to the target
            Vector3 direction = target.transform.position - transform.position;

            // 2. rotating the tower so it aims right at the target
            transform.right = -direction;
        }
    }

    // turning on the fire effect for half a second before turning it back off
    IEnumerator FireEffect()
    {


        fireEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fireEffect.SetActive(false);
    }
}