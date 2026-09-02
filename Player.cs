using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Lets other scripts easily access this Player script
    public static Player main;

    [SerializeField] private int health = 500;
    public int money; 

    [SerializeField] private TextMeshProUGUI HPGui;
    [SerializeField] private TextMeshProUGUI MoneyGUI;

    [SerializeField] private GameObject gameOverGUI;

    void Awake()
    {
        // Sets up the script connection and starting money
        main = this;
        money = 500; 
    }

    void Update()
    {
        // Updates the health and money text on the screen every frame
        HPGui.text = "HP: " + health.ToString();
        MoneyGUI.text = "Money: " + money.ToString();
    }

    // Lowers health (when enemies reach the end) and shows the game over screen if health runs out
    public void damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            gameOverGUI.SetActive(true);
        }
    }

    // Reloads the current level to play again
    public void restart ()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}