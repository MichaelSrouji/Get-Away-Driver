using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankTransition : MonoBehaviour
{
    public RobberSpeech robber;
    private bool entered = false;

    void OnTriggerEnter()
    {
        if (!entered)
        {
            entered = true;
            robber.GoToCutscene();
        }

    }
}
