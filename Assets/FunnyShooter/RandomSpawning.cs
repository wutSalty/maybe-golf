using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSpawning : MonoBehaviour
{
    public FunnyGameManager gameMan;

    public Text timerText;
    public float currentTime { get; private set; }

    [System.Serializable]
    public class Spawning
    {
        public float RequiredWaitingTime = 1f;
        public int SpawnAtATime = 3;
        public GameObject enemyPrefab;
        public float EndTimeInMinutes;
    }

    public Spawning[] spawnPattern;

    [System.Serializable]
    public class SpecialSpawning
    {
        public float SpawnTimeInMinutes;
        public GameObject EnemyToSpawn;
        public bool HasSpawned;
    }

    public SpecialSpawning[] specialSpawn;

    private Camera cam;
    public GameObject deathParticles;

    [ContextMenu("Set Timer To 27")]
    private void CheatTimerTo25()
    {
        currentTime = 29 * 60f;
    }

    private void Start()
    {
        cam = Camera.main;
        BeginSpawning();
    }

    public void BeginSpawning()
    {
        StartCoroutine(IncrementTimer());
        StartCoroutine(SpawningCycle());
        StartCoroutine(CheckingSpecialSpawns());
    }

    private IEnumerator IncrementTimer()
    {
        while (gameMan.GameIsActive)
        {
            currentTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return null;
        }
    }

    private IEnumerator SpawningCycle()
    {
        bool SpawnPossible = true;
        int NumOfIndex = spawnPattern.Length;

        while (SpawnPossible)
        {
            int phase = 0;
            foreach (var item in spawnPattern)
            {
                phase += 1;
                print("Phase " + phase.ToString() + "Started");

                while (item.EndTimeInMinutes * 60 > currentTime)
                {
                    for (int i = 0; i < item.SpawnAtATime; i++)
                    {
                        Instantiate(item.enemyPrefab, GetRandomSpawn(), Quaternion.identity);
                    }
                    yield return new WaitForSeconds(item.RequiredWaitingTime);
                }
            }

            SpawnPossible = false;
            yield return null;
        }
        print("End of game. Resorting to violence");

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in allEnemies)
        {
            Instantiate(deathParticles, item.transform.position, deathParticles.transform.rotation);
            Destroy(item);
        }

        yield return new WaitForSeconds(3f);

        var finalSpawn = spawnPattern[NumOfIndex - 1];
        while (true)
        {
            for (int i = 0; i < finalSpawn.SpawnAtATime; i++)
            {
                Instantiate(finalSpawn.enemyPrefab, GetRandomSpawn(), Quaternion.identity);
            }
            yield return new WaitForSeconds(finalSpawn.RequiredWaitingTime);
        }
    }

    private IEnumerator CheckingSpecialSpawns()
    {
        if (specialSpawn.Length == 0)
        {
            yield break;
        }

        foreach (var item in specialSpawn) 
        {
            while (currentTime < item.SpawnTimeInMinutes * 60f)
            {
                yield return null;
            }

            Instantiate(item.EnemyToSpawn, GetRandomSpawn(), Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawn()
    {
        float x = 0f;
        float y = 0f;
        
        if (Random.value > 0.5f) //Spawn top or bottom of screen
        {
            x = Random.Range(-5.8f, 5.8f);

            if (Random.value > 0.5f)
            {
                y = Random.Range(2.9f, 3.9f);
            }
            else
            {
                y = Random.Range(-3.9f, -2.9f);
            }
        }
        else //Spawn on the left/right side of the screen
        {
            if (Random.value > 0.5f)
            {
                x = Random.Range(4.8f, 5.8f);
            }
            else
            {
                x = Random.Range(-5.8f, -4.8f);
            }

            y = Random.Range(-3.9f, 3.9f);
        }

        Vector3 pos = new Vector3(x + cam.transform.position.x, y + cam.transform.position.y, 0);
        return pos;
    }
}
