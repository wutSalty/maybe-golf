using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int enemyHealth = 50;
    private int internalHealth;

    private Canvas canvas;
    private Camera cam;
    private FunnyCharMovement playerScript;

    [SerializeField] private Slider hpBar;
    [SerializeField] private GameObject damageCounter;

    public float spawnRate = 0.3f;
    [SerializeField] private GameObject coinDrop;

    private void Start()
    {
        internalHealth = enemyHealth;
        hpBar.value = enemyHealth;
        cam = Camera.main;
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = cam;
        playerScript = FindObjectOfType<FunnyCharMovement>();
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(cam.transform.position.x, cam.transform.position.y, 0), 0.5f * Time.deltaTime);
    }

    public void TakeDamage(int damage, bool crit)
    {
        internalHealth -= damage;
        hpBar.value = internalHealth;

        GameObject counter = Instantiate(damageCounter, canvas.transform, false);
        Text counterText = counter.GetComponentInChildren<Text>();

        counterText.text = damage.ToString();
        if (crit)
        {
            counterText.color = Color.red;
        }
        else
        {
            counterText.color = Color.white;
        }
        counter.transform.localPosition = GetRandomPosition();

        if (internalHealth <= 0)
        {
            if (Random.value <= spawnRate)
            {
                Instantiate(coinDrop, transform.position, Quaternion.identity);
            }

            playerScript.AddToDeathCounter(1);
            Destroy(gameObject);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-200f, 200f);
        float y = Random.Range(100f, 200f);
        Vector3 pos = new Vector3(x, y, 0);
        return pos;
    }
}
