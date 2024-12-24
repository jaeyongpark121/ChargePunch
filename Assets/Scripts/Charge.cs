using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{

    private PlayerController playerController1;
    private PlayerController playerController2;

    // Start is called before the first frame update
    void Start()
    {
        playerController1 = GetComponent<PlayerController>();
        playerController2 = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCharge(float amount)
    {
        if (playerController1 != null)
        {
            playerController1.ChargeAmount += amount;
        }
        else
        {
            Debug.Log("PLAYER_CONTROLLER_NOT_FOUND");
        }
    }
}
