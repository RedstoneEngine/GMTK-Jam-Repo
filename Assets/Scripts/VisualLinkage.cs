using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualLinkage : MonoBehaviour
{
    //Epic Scripting
    public Transform linkageObject;
    public Transform linkedObject;
    public float cylinderScale = .25f;
    public Vector3 objOffset;
    public Vector3 linkedOffset;


    void Update()
    {
        //Calculations for Cylinder
        Vector3 startPoint = transform.localToWorldMatrix * new Vector4(objOffset.x, objOffset.y, objOffset.z);
        startPoint += transform.position;
        Vector3 endPoint = linkedObject.localToWorldMatrix * new Vector4(linkedOffset.x, linkedOffset.y, linkedOffset.z);
        endPoint += linkedObject.position;

        linkageObject.position = (startPoint + endPoint) * .5f;

        linkageObject.LookAt(endPoint);
        linkageObject.rotation *= Quaternion.Euler(90, 0, 0);

        linkageObject.localScale = new Vector3(cylinderScale, Vector3.Distance(startPoint, endPoint) * .5f, cylinderScale);
    }

    //Suit Hits Enemy
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyScript>())
        {
            other.GetComponent<EnemyScript>().health--;
            if (other.GetComponent<EnemyScript>().health <= 0)
            {
                Destroy(other.gameObject);
                OxygenMeter.changeOxygen(50);
            }
        }
    }
}
