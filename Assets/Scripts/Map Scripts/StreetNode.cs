using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetNode : MonoBehaviour
{
    public GameObject[] adjacentIntersections;
    public Color lineColor;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, 4.0f);

        foreach(GameObject sister in adjacentIntersections)
        {
            Gizmos.DrawLine(this.gameObject.transform.position, sister.gameObject.transform.position);
        }
    }
}
