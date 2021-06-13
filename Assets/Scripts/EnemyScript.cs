using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject laserObj;
    public Vector2 upperBounds = new Vector2(40, 40);
    public Vector2 lowerBounds = new Vector2(-40, -40);
    public float detectionRange = 25;
    public float idleSpeed = 2;
    public float attackSpeed = 12;
    public float attackInterval = 5;
    public int health = 2;

    Transform player;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        StartCoroutine(mainAI());
    }

    //Switches from Wander to Attack
    IEnumerator mainAI ()
    {
        Coroutine currentAI = StartCoroutine(wanderAI());
        yield return new WaitUntil(() => Vector3.Distance(transform.position, player.position) <= detectionRange);
        StopCoroutine(currentAI);
        StartCoroutine(attackAI());
    }

    IEnumerator wanderAI ()
    {
        while (true)
        {
            //This is so it normally moves in a forward direction
            Vector3 targetPos = transform.position + transform.forward * 5;

            //This is so they don't congregate near walls
            if (upperBounds.x < targetPos.x || targetPos.x < lowerBounds.x || upperBounds.y < targetPos.y || targetPos.y < lowerBounds.y)
                targetPos -= transform.forward * 15;

            targetPos += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 10;
            targetPos.x = Mathf.Clamp(targetPos.x, lowerBounds.x, upperBounds.x);
            targetPos.y = Mathf.Clamp(targetPos.y, lowerBounds.y, upperBounds.y);


            float targetRot;

            targetRot = 180 + Mathf.Atan2(transform.position.x - targetPos.x, transform.position.z - targetPos.z) * Mathf.Rad2Deg;

            //Moves Enemy
            while (Vector3.Distance(transform.position, targetPos) > .1)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, idleSpeed * Time.deltaTime);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, lowerBounds.x, upperBounds.x), .5f, Mathf.Clamp(transform.position.z, lowerBounds.y, upperBounds.y));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Vector3.up * targetRot), Time.deltaTime * 100);
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(0, 5) < 3 ? Random.Range(0f, .5f) : Random.Range(5f, 15f));
        }
    }

    IEnumerator attackAI ()
    {
        while (true)
        {
            //Get Close
            while (Vector3.Distance(transform.position, player.position) >= detectionRange)
            {
                //Moves in at a Wave Pattern
                transform.position = Vector3.MoveTowards(transform.position, player.position + transform.right * Mathf.Sin(Time.time * 2) * 10, attackSpeed * Time.deltaTime);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, lowerBounds.x, upperBounds.x), .5f, Mathf.Clamp(transform.position.z, lowerBounds.y, upperBounds.y));

                float rotation = 180 + Mathf.Atan2(transform.position.x - player.position.x, transform.position.z - player.position.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Vector3.up * rotation), Time.deltaTime * 200);

                yield return null;
            }

            //Attack
            GetComponent<AudioSource>().Play();
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                float rotation = 180 + Mathf.Atan2(transform.position.x - player.position.x, transform.position.z - player.position.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Vector3.up * rotation), Time.deltaTime * 200);
                yield return null;
            }
            Instantiate(laserObj, transform.position, transform.rotation * Quaternion.Euler(90, 0, 0));

            //Flee!!
            Coroutine fleeing = StartCoroutine(FleeAI());
            yield return new WaitForSeconds(attackInterval);
            StopCoroutine(fleeing);
        }
    }

    IEnumerator FleeAI ()
    {
        bool direction = Random.Range(0, 2) == 0;
        while (true)
        {
            //Turns a direction
            float rotation = Mathf.Atan2(transform.position.x - player.position.x, transform.position.z - player.position.z) * Mathf.Rad2Deg + 70 * (direction ? 1 : -1);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Vector3.up * rotation), Time.deltaTime * 200);

            //Moves that direction
            transform.position += transform.forward * attackSpeed * Time.deltaTime;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, lowerBounds.x, upperBounds.x), .5f, Mathf.Clamp(transform.position.z, lowerBounds.y, upperBounds.y));

            //Switches Direction
            if (Random.Range(0, 150) < 2)
                direction = !direction;
            yield return null;
        }
    }
}
