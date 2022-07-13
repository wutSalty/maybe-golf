using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSpawning : MonoBehaviour
{
    public Text timerText;
    [HideInInspector] public float currentTime;

    [System.Serializable]
    public class Spawning
    {
        public float RequiredWaitingTime = 1f;
        public int SpawnAtATime = 3;
        public GameObject enemyPrefab;
        public float EndTimeInMinutes;
    }

    public Spawning[] spawnPattern;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        BeginSpawning();
    }

    public void BeginSpawning()
    {
        StartCoroutine(IncrementTimer());
        StartCoroutine(SpawningCycle());
    }

    private IEnumerator IncrementTimer()
    {
        while (true)
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
        print("Out Of Enemies");
    }

    private Vector3 GetRandomSpawn()
    {
        float x = 0f;
        float y = 0f;
        
        if (Random.value > 0.5f) //Spawn in the corners of the screen
        {
            if (Random.value > 0.5f)
            {
                x = Random.Range(4f, 5f);
            }
            else
            {
                x = Random.Range(-5f, -4f);
            }

            if (Random.value > 0.5f)
            {
                y = Random.Range(2.7f, 3.7f);
            }
            else
            {
                y = Random.Range(-3.7f, -2.7f);
            }
        }
        else //Spawn on the left/right side of the screen
        {
            if (Random.value > 0.5f)
            {
                x = Random.Range(4f, 5f);
            }
            else
            {
                x = Random.Range(-5f, -4f);
            }

            y = Random.Range(-3.7f, 3.7f);
        }

        Vector3 pos = new Vector3(x + cam.transform.position.x, y + cam.transform.position.y, 0);
        return pos;
    }
}
