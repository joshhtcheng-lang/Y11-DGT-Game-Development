using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacement : MonoBehaviour
{
    // setting up my range circle visuals and colors for when placement is allowed or blocked
    [SerializeField] private SpriteRenderer rangeSprite;
    [SerializeField] private CircleCollider2D rangeCollider;
    [SerializeField] private Color gray;
    [SerializeField] private Color red;

    // tracking if the tower is currently following my mouse around
    [NonSerialized] public bool isPlacing = true;
    private int restrictionCount = 0;

    private Tower tower;

    void Awake()
    {
        // grabbing my tower script and turning off its attack range collider while dragging
        tower = GetComponent<Tower>();
        rangeCollider.enabled = false;
    }

    void Update()
    {
        // making the tower follow my mouse position across the screen while placing
        if (isPlacing)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            transform.position = mousePosition;
        }

        // stopping placement clicks from happening if my mouse is currently hovering over ui buttons
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        bool isRestricted = restrictionCount > 0;

        // placing the tower down if i click, it's not blocked, and i have enough money
        if (Mouse.current.leftButton.wasPressedThisFrame && !isRestricted && tower.cost < Player.main.money)
        {
            rangeCollider.enabled = true;
            rangeSprite.enabled = false;
            isPlacing = false;
            GetComponent<TowerPlacement>().enabled = false;
            Player.main.money -= tower.cost;
        }

        // turning the range circle red if blocked, otherwise keeping it gray
        if (isRestricted)
        {
            rangeSprite.color = red;
        }
        else
        {
            rangeSprite.color = gray;
        }
    }

    // counting up how many illegal areas or other towers i am touching
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Restricted") || (collision.CompareTag("Tower") && isPlacing))
        {
            restrictionCount++;
        }
    }

    // counting down when i move away from illegal areas or other towers
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Restricted") || (collision.CompareTag("Tower") && isPlacing))
        {
            restrictionCount = Mathf.Max(0, restrictionCount - 1);
        }
    }
}