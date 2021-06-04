using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStartPos : MonoBehaviour
{
    public GameObject startPosGameObject;

    // Start is called before the first frame update
    void Start()
    {
        if (startPosGameObject != null)
        {
            transform.position = startPosGameObject.transform.position;
        }
    }
}
