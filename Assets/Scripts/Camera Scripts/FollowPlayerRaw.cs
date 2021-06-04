using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerRaw : MonoBehaviour
{
    public GameObject targetPos;
    public GameObject targetLook;
    public float maxDistance = 10;
    public float lerpValue = 50;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position,
                                          targetPos.transform.position,
                                          (Vector3.Distance(transform.position, targetPos.transform.position) / maxDistance) * lerpValue * Time.deltaTime);
        transform.LookAt(targetLook.transform);
    }
}
