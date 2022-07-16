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

    public float KBAmount = 0.2f;
    public float KBTime = 1f;

    private Camera cam;
    private FunnyCharMovement playerScript;

    [SerializeField] private GameObject damageCounter;

    public float spawnRate = 0.3f;
    [SerializeField] private GameObject coinDrop;

    public float TimeBetweenAttack = 0.5f;
    private Vector3 targetLocation;
    private Coroutine coroutine;

    private bool AutoMoveOK = true;

    private Coroutine co;

    private void Start()
    {
        internalHealth = enemyHealth;
        cam = Camera.main;
        playerScript = FindObjectOfType<FunnyCharMovement>();

        StartCoroutine(generateRandomTargetPos());
    }

    private void Update()
    {
        if (AutoMoveOK)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation + playerScript.transform.position, moveSpeed * Time.deltaTime);
        }
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

    public void TakeKnockback(Transform bulletPos)
    {
        if (co != null)
        {
            return;
        }

        float angle = bulletPos.eulerAngles.z;
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 posDifference = new Vector3(x, y, 0);

        posDifference = posDifference.normalized * KBAmount;

        co = StartCoroutine(TakingKnockback(posDifference));
    }

    private IEnumerator TakingKnockback(Vector3 dir)
    {
        AutoMoveOK = false;
        float t = 0;
        Vector3 currentPos = transform.position;
        Vector3 targetPos = transform.position + dir;

        while (t < KBTime)
        {
            transform.position = Vector3.Lerp(currentPos, targetPos, t / KBTime);
            t += Time.deltaTime;

            yield return null;
        }

        AutoMoveOK = true;
        co = null;
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
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(damageingPlayer(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }

    private IEnumerator damageingPlayer(Collider2D collision)
    {
        while (true)
        {
            collision.GetComponent<FunnyPlayerHealth>().TakeDamage(damageGiven);

            yield return new WaitForSeconds(TimeBetweenAttack);
        }
    }

    private IEnumerator generateRandomTargetPos()
    {
        while (true)
        {
            Vector2 rndPos = Random.insideUnitCircle * (maxRad - minRad);
            rndPos += rndPos.normalized * minRad;
            targetLocation = rndPos;

            yield return new WaitForSeconds(timeBetweenChangeTarget);
        }
    }
}
