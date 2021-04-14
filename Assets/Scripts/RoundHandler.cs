using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class RoundHandler : MonoBehaviour {

    // Round related
    private int _currentRound = 1;

    // Zombie related
    [SerializeField] private GameObject _bossZombie;
    [SerializeField] private List<GameObject> _baseZombies = new List<GameObject>();
    [SerializeField] private int _bossRound; // Every x round, default: 5
    [SerializeField] private int _waitTimeBetweenRounds = 5;
    [SerializeField] private int _beginningCountdown = 5;

    private int _zombieBaseHealth;
    public List<GameObject> _zombieSpawners = new List<GameObject>();

    public delegate void RoundAction(int round, int waitTime);
    public static RoundAction roundBegun;
    public static RoundAction roundEnded;

    public delegate void BeginGameAction(int countdown);
    public static BeginGameAction gameBegun;

    // Start is called before the first frame update
    public void Start() {
        _zombieSpawners.AddRange(GameObject.FindGameObjectsWithTag("ZombieSpawner"));
        _zombieBaseHealth = 50;

        if (_bossZombie == null) {
            Debug.LogError("Boss zombie is not assigned!");
        }

        if (_zombieSpawners.Count == 0) {
            Debug.LogError("No zombie spawner(s) found!");
        }

        if (_baseZombies.Count == 0) {
            Debug.LogError("No zombies have been assigned!");
        }

        if (_bossRound <= 0) {
            _bossRound = 5;
        }

        StartCoroutine(BeginGame());
    }

    /* ##################################################

            Zombie related

    ################################################## */

    private List<GameObject> _zombiesToSpawn = new List<GameObject>();
    private List<GameObject> _zombies = new List<GameObject>();

    private int ZombiesAlive() {
        int alive = 0;

        foreach (GameObject zombie in _zombies) {
            bool zombieIsAlive = zombie.GetComponent<ZombieHandler>().IsAlive();
            if(zombieIsAlive) {
                alive++;
            }
        }
        return alive;
    }

    private void SpawnZombies() {
        int zombieHealth = CalculateHealth();
        int zombies = CalculateAmountOfBaseZombies();
        
        for (int i = 0; i < zombies; i++) {
            int zombieSelected = UnityEngine.Random.Range(0, _baseZombies.Count);
            _zombiesToSpawn.Add(_baseZombies[zombieSelected]);
        }

        foreach (GameObject zombie in _zombiesToSpawn) {
            GameObject spawner = _zombieSpawners[UnityEngine.Random.Range(0, _zombieSpawners.Count)];
            _zombies.Add(spawner.GetComponent<SpawnerScript>().SpawnZombie(zombie, zombieHealth));
        }

        if (IsBossRound()) {
            int bosses = CalculateAmountOfBossZombies();
            int bossHealth = CalculteBossHealth(zombieHealth);

            for (int i = 0; i < bosses; i++) {
                _zombiesToSpawn.Add(_bossZombie);
                GameObject spawner = _zombieSpawners[UnityEngine.Random.Range(0, _zombieSpawners.Count)];
                _zombies.Add(spawner.GetComponent<SpawnerScript>().SpawnZombie(_bossZombie, bossHealth));
            }
        }
    }

    private int CalculateHealth() {
        int healthMultiplier = 2;
        return _zombieBaseHealth + (_currentRound * healthMultiplier);
    }

    private bool IsBossRound() {
        return _currentRound % _bossRound == 0;
    }

    private int CalculateAmountOfBaseZombies() {
        int baseAmount = 15;
        int roundMultiplier = 3;
        return baseAmount + (_currentRound * roundMultiplier);
    }

    private int CalculateAmountOfBossZombies() {
        return _currentRound / _bossRound;
    }

    private int CalculteBossHealth(int baseHealth) {
        int bossHealthMultiplier = 2;
        return baseHealth * bossHealthMultiplier;
    }

    private void ZombieCleanUp() {
        foreach (GameObject zombie in _zombies) {
            Destroy(zombie);
        }

        _zombies.Clear();
        _zombiesToSpawn.Clear();

    }

    public void ZombieDance() {
        bool isAlive;
        ZombieHandler handler;

        foreach (GameObject zombie in _zombies) {
            handler = zombie.GetComponent<ZombieHandler>();
            isAlive = handler.IsAlive();

            if (isAlive) {
                handler.StartDancing();
            }
        }
    }

    /* ##################################################

            Round related

    ################################################## */

    public void CheckZombiesLeft()
    {
        if(ZombiesAlive() == 0 )
        {
            StartCoroutine(EndRound());
        }
    }

    public void BeginNewRound(int round)
    {
        _currentRound = round;

        SpawnZombies();
        roundBegun?.Invoke(round, _waitTimeBetweenRounds);
    }

    public IEnumerator EndRound()
    {
        ZombieCleanUp();
        roundEnded?.Invoke(_currentRound, _waitTimeBetweenRounds);
        yield return new WaitForSeconds(_waitTimeBetweenRounds);
        BeginNewRound(_currentRound + 1);
    }

    public IEnumerator BeginGame()
    {
        gameBegun?.Invoke(_beginningCountdown);
        yield return new WaitForSeconds(_beginningCountdown);
        BeginNewRound(1);
    }

    public int GetCurrentRound()
    {
        return _currentRound;
    }

}
