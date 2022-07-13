using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int enemyHealth = 50;
    private int internalHealth;

    public int damageGiven = 2;
    public float moveSpeed = 0.5f;
    public float timeBetweenChangeTarget = 1f;

    public float maxRad = 0.5f;
    public float minRad = 0.2f;

    private Camera cam;
    private FunnyCharMovement playerScript;

    [SerializeField] private GameObject damageCounter;

    public float spawnRate = 0.3f;
    [SerializeField] private GameObject coinDrop;

    private Vector3 targetLocation;

    private void Start()
    {
        internalHealth = enemyHealth;
        cam = Camera.main;
        playerScript = FindObjectOfType<FunnyCharMovement>();

        StartCoroutine(generateRandomTargetPos());
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetLocation + playerScript.transform.position, moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage, bool crit)
    {
        internalHealth -= damage;

        GameObject counter = Instantiate(damageCounter, transform, false);
        counter.GetComponent<Canvas>().worldCamera = cam;
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

            Canvas[] canvases = GetComponentsInChildren<Canvas>();
            foreach (var item in canvases)
            {
                Vector3 currentTrans = item.transform.position;
                item.transform.SetParent(null, true);
                item.transform.position = currentTrans;
            }

            playerScript.AddToDeathCounter(1);
            Destroy(gameObject);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-0.2f, 0.2f);
        float y = Random.Range(0.1f, 0.2f);
        Vector3 pos = new Vector3(x, y, 0);
        return pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<FunnyPlayerHealth>().TakeDamage(damageGiven);
        }
    }

    private IEnumerator generateRandomTargetPos()
    {
        while (true)
        {
            Vector2 rndPos = Random.insideUnitCircle * (maxRad - minRad);
            rndPos += rndPos.normalized * minRad;
            //targetLocation = new Vector3(playerScript.transform.position.x + rndPos.x, playerScript.transform.position.y, 0);
            targetLocation = rndPos;

            yield return new WaitForSeconds(timeBetweenChangeTarget);
        }
    }
}
