using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawning : MonoBehaviour
{
    public float RequiredWaitingTime = 1f;
    public int SpawnAtATime = 3;
    public GameObject enemyPrefab;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        BeginSpawning();
    }

    public void BeginSpawning()
    {
        StartCoroutine(SpawningCycle());
    }

    private IEnumerator SpawningCycle()
    {
        while (true)
        {
            for (int i = 0; i < SpawnAtATime; i++)
            {
                Instantiate(enemyPrefab, GetRandomSpawn(), Quaternion.identity);
            }
            yield return new WaitForSeconds(RequiredWaitingTime);
        }
    }

    private Vector3 GetRandomSpawn()
    {
        float x = 0f;
        float y = 0f;

        if (Random.value > 0.5f)
        {
            x = Random.Range(3.7f, 4.7f);
        } else
        {
            x = Random.Range(-4.7f, -3.7f);
        }

        if (Random.value > 0.5f)
        {
            y = Random.Range(2.5f, 3.5f);
        }
        else
        {
            y = Random.Range(-3.5f, -2.5f);
        }
        Vector3 pos = new Vector3(x + cam.transform.position.x, y + cam.transform.position.y, 0);
        return pos;
    }
}
