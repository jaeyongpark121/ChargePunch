using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Hard components
    [SerializeField] private UIManager uiManagerScript;
    private Rigidbody2D rb;
    private Charge chargeScript;

    //Player components
    [SerializeField] private float chargeAmount = 0f;
    [SerializeField] private bool isCharging = false;
    private float maxY;
    private int dashPower;
    private float attackChargeAmount;
    private float attackDistance;
    private enum PlayerState
    {
        Idle,
        Defending,
        Punch,
        Dash,
        Damaged
    }
    
    public float ChargeAmount // chargeAmount property
    {
        get { return chargeAmount; }
        set { chargeAmount = Mathf.Clamp(value, 0, 100); } 
    }

    PlayerState currentState = PlayerState.Idle;
    void Start()
    {
        chargeScript = GetComponent<Charge>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ChargeChecking();
        Attack();

        if (currentState == PlayerState.Dash) // ���� ���½� ����
        {
            DashWithPhysics();
        }

        if (currentState != PlayerState.Dash && currentState != PlayerState.Damaged) // ����/�ĸ��� �ʾ����� �ӵ� ����
        {
            Vector3 velocity = rb.velocity;
            velocity.x *= 0.8f;
            rb.velocity = velocity;
        }

        uiManagerScript.UpdateChargeText(ChargeAmount); // ������ �����ֱ�
    }
    void FixedUpdate()
    {
        if (isCharging /*&& chargeScript != null*/) // ����
        {
            chargeScript.AddCharge(1f);
        }

        Manage_Y(); // y�� �� ���� �ʰ� ����
    }

    void ChargeChecking()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCharging = true;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            isCharging = false;
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (chargeAmount >= 25f) {
                currentState = PlayerState.Dash;
                attackChargeAmount = chargeAmount;
                if (chargeAmount >= 100f)
                {
                    dashPower = 4;
                }
                else if (chargeAmount >= 75f)
                {
                    dashPower = 3;
                }
                else if (chargeAmount >= 50f)
                {
                    dashPower = 2;
                }
                else
                {
                    dashPower = 1;
                }
                attackDistance = Mathf.Log(dashPower * attackChargeAmount) * dashPower * dashPower; 
                ChargeAmount -= 25;
                Debug.Log("Dash");
            }
            else
            {
                Debug.Log("Punch");
            }

        }
    }

    void Manage_Y() // Managing Y axis
    {
        if(transform.position.y <= maxY)
        {
            maxY = transform.position.y;
        }

        if(transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
    }

    void DashWithPhysics()
    {
        float moveStep = dashPower * 30 * Time.fixedDeltaTime; // FixedUpdate ���� �̵�
        attackDistance -= moveStep;
        Debug.Log(attackDistance);

        if (attackDistance <= 0.0f)
        {
            currentState = PlayerState.Idle; // �̵� ����
            moveStep += attackDistance; // �ʰ� �̵� ����
        }

        Vector3 velocity = rb.velocity;
        velocity.x = moveStep / Time.fixedDeltaTime;
        rb.velocity = velocity;
        
    }
}


