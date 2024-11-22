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

        if (currentState == PlayerState.Dash) // 공격 상태시 돌진
        {
            DashWithPhysics();
        }

        if (currentState != PlayerState.Dash && currentState != PlayerState.Damaged) // 공격/쳐맞지 않았으면 속도 감소
        {
            Vector3 velocity = rb.velocity;
            velocity.x *= 0.8f;
            rb.velocity = velocity;
        }

        uiManagerScript.UpdateChargeText(ChargeAmount); // 충전량 보여주기
    }
    void FixedUpdate()
    {
        if (isCharging /*&& chargeScript != null*/) // 충전
        {
            chargeScript.AddCharge(1f);
        }

        Manage_Y(); // y값 붕 뜨지 않게 고정
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
        float moveStep = dashPower * 30 * Time.fixedDeltaTime; // FixedUpdate 기준 이동
        attackDistance -= moveStep;
        Debug.Log(attackDistance);

        if (attackDistance <= 0.0f)
        {
            currentState = PlayerState.Idle; // 이동 종료
            moveStep += attackDistance; // 초과 이동 방지
        }

        Vector3 velocity = rb.velocity;
        velocity.x = moveStep / Time.fixedDeltaTime;
        rb.velocity = velocity;
        
    }
}


