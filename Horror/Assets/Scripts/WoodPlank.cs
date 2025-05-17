using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WoodPlank : MonoBehaviour
{
    public float fallForce = 2f;

    private bool isFalling = false;

    private static int fallenPlanksCount = 0;

    void Update()
    {
        if(isFalling)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.AddForce(transform.forward * fallForce, ForceMode.Impulse);
            isFalling = false;

            fallenPlanksCount++;
            if(fallenPlanksCount >= 2)
            {
                Debug.Log("Door unlocked");
               
            }
        }
    }
    public void Fall()
    {
        isFalling = true;
    }
}
