using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    float speed = .75f;
    float deadTime = 10;

    void Update()
    {
        transform.position += transform.up * speed;
        deadTime -= Time.deltaTime;
        if (deadTime <= 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "Player")
        {
            OxygenMeter.changeOxygen(-10);
            Destroy(gameObject);
        }
    }
}
