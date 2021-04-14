using System;
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

    public delegate void HealthChangeAction(int health, int maxHealth);
    public static event HealthChangeAction OnHealthChange;

    public static event Action OnDeath;

    private bool dead = false;

    private void Update()
    {
        if(dead && Input.GetKeyDown(KeyCode.Return))
        {
            Respawn();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Damage(15);
        }
    }

    public void Damage(int damage)
    {
        health -= damage;
        CheckHealth();
        //I multiplayer, tjek hvis dette er main spiller. Hvis ikke, så kald ikke næste linje
        OnHealthChange?.Invoke(health, maxHealth);
    }

    public void Heal(int heal)
    {
        health += heal;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        //I multiplayer, tjek hvis dette er main spiller. Hvis ikke, så kald ikke næste linje
        OnHealthChange?.Invoke(health, maxHealth);
    }

    public int GetHealth()
    {
        return health;
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        animator.SetBool("Dead", true);
        //roundHandler.ZombieDance();
        yield return new WaitForSeconds(2f);
        dead = true;
        OnDeath?.Invoke();
    }

    public void Respawn()
    {
        //Omskriv følgende i multiplayer
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
