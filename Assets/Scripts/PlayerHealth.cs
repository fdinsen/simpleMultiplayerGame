using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigid;

    private TMP_Text respawnText;
    private RoundHandler roundHandler = null;
    private bool dead = false;

    private void Start()
    {
        roundHandler = FindObjectOfType<RoundHandler>();

        respawnText = GameObject.FindGameObjectWithTag("RespawnScreen").GetComponent<TMP_Text>();
        if (respawnText != null)
        {
            respawnText.enabled = false;
        }else
        {
            Debug.LogError("Please add prefab DefaultCanvas to the scene!!");
        }
    }

    private void Update()
    {
        if(dead && Input.GetKeyDown(KeyCode.Return))
        {
            Respawn();
        }
    }

    public void Damage(int damage)
    {
        health -= damage;
        checkHealth();
    }

    public void Heal(int heal)
    {
        health += heal;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    private void checkHealth()
    {
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        animator.SetBool("Dead", true);
        roundHandler.ZombieDance();
        yield return new WaitForSeconds(2f);
        dead = true;
        DisplayRespawnText();
    }

    public void Respawn()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void DisplayRespawnText()
    {
        respawnText.enabled = true;
        respawnText.text = "You Died on round " + roundHandler.GetCurrentRound()  + "!\nPress Enter to respawn!";
    }

}
