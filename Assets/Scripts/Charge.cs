using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCharge(float amount)
    {
        if (playerController != null)
        {
            playerController.ChargeAmount += amount;
        }
        else
        {
            Debug.Log("PLAYER_CONTROLLER_NOT_FOUND");
        }
    }
}
