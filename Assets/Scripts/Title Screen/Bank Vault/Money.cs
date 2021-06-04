using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private bool holding = false;

    private Vector3 mouseScreenPos;

    //physics variables
    public float followSpeed = 10f;
    public float lookAtSpeed = 10f;
    private float throwForce = 5f;

    private Vector3 spawnPos;
    private float distanceFromSpawn = 10;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (holding)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            mouseScreenPos = Camera.main.ScreenToWorldPoint(mousePos);

            transform.position = Vector3.Slerp(transform.position,
                                               mouseScreenPos - Vector3.up,
                                               followSpeed * Vector3.Distance(transform.position, mouseScreenPos - Vector3.up) * Time.deltaTime);

            transform.LookAt(mouseScreenPos, Vector3.up);
        }

        //Respawn();
    }

    private void OnMouseDrag()
    {
        if (!holding)
        {
            rb.velocity = Vector3.zero;
        }
        StopAllCoroutines();
        holding = true;
        rb.useGravity = false;
    }

    private void OnMouseUp()
    {
        holding = false;
        rb.useGravity = true;

        Debug.Log("money distance: " + Vector3.Distance(transform.position, mouseScreenPos - Vector3.up));
        if(Vector3.Distance(transform.position, mouseScreenPos - Vector3.up) > .7f)
        {
            rb.AddForce((mouseScreenPos - transform.position) * throwForce * Vector3.Distance(transform.position, mouseScreenPos - Vector3.up),
                        ForceMode.Impulse);
        }
        Respawn();
    }

    private void Respawn()
    {
        StartCoroutine(RespawnMoney());
    }

    IEnumerator RespawnMoney()
    {
        yield return new WaitForSeconds(5);
        //vector3.zero because the bank is located at (0,0,0)
        if (Mathf.Abs(transform.position.z - distanceFromSpawn) > distanceFromSpawn ||
            Mathf.Abs(transform.position.x - distanceFromSpawn) > distanceFromSpawn ||
            transform.position.y < 0)
        {
            //reset velocity
            rb.isKinematic = true;
            yield return null;
            rb.isKinematic = false;
            //reset position;
            transform.position = spawnPos;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
