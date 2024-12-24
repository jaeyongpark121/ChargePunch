using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Hard components
    //[SerializeField] private UIManager uiManagerScript;
    private Rigidbody2D rb;
    private Charge chargeScript;
    
    //Player components
    [SerializeField] private float chargeAmount = 0f;
    [SerializeField] private bool isCharging = false;
    [SerializeField] private KeyCode attackKey = KeyCode.X; // 키설정
    [SerializeField] private KeyCode chargeKey = KeyCode.C; // 키설정
    [SerializeField] private int facing = 1;
    private float maxY;
    private int dashPower;
    private float attackChargeAmount;
    private float attackDistance;

    public enum PlayerState
    {
        Idle,
        Guard,
        Punch,
        Dash,
        Damaged
    }
    
    public float ChargeAmount // chargeAmount property
    {
        get { return chargeAmount; }
        set { /*if (value < 0 || value > 100) Debug.LogWarning($"{value}가 범위를 넘었습니다.");*/
            chargeAmount = Mathf.Clamp(value, 0, 100); } 
    }

    [SerializeField] PlayerState currentState = PlayerState.Guard;
    public PlayerState CurrentState // chargeAmount property
    {
        get { return currentState; }
    }

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
            velocity.x *= 0.9f;
            rb.velocity = velocity;
        }

        if( Mathf.Abs(rb.velocity.x) < 1.0E-05f)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if(currentState == PlayerState.Idle && rb.velocity.x == 0 && !isCharging)
        {
            currentState = PlayerState.Guard;
        }

    }
    void FixedUpdate()
    {
        if (isCharging /*&& chargeScript != null*/) // 충전
        {
            chargeScript.AddCharge(1f);
        }

        Manage_Y(); // y값 붕 뜨지 않게 고정
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (currentState == PlayerState.Dash && collision.gameObject.CompareTag("Player"))
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            currentState = PlayerState.Idle;

            if (otherPlayer != null && otherPlayer.currentState == PlayerState.Idle)
            {
                // 상대 플레이어 밀기
                if (otherRb != null)
                {
                    otherRb.velocity = new Vector2(50 * Mathf.Sqrt(dashPower) * facing, 0); // 밀려나는 속도 설정
                    otherPlayer.ChargeAmount -= 25;
                }
                Debug.Log("power = " + 100 * Mathf.Sqrt(dashPower));
                // 공격 종료 후 상태 복귀
            }

            else if(otherPlayer != null && otherPlayer.currentState == PlayerState.Dash)
            {
                // 약한쪽이 밀리기.
                if (otherPlayer.dashPower > dashPower)
                {
                    rb.velocity = new Vector2(50 * Mathf.Sqrt(dashPower) * facing * -1, 0);
                }
                
                else if (otherPlayer.dashPower == dashPower)
                {
                    otherRb.velocity = new Vector2(25 * Mathf.Sqrt(dashPower) * facing, 0);
                    rb.velocity = new Vector2(25 * Mathf.Sqrt(dashPower) * facing * -1, 0);
                }
            }

            else if (otherPlayer != null && otherPlayer.currentState == PlayerState.Guard)
            {
                // 방어하고 있으면 내가 밀리기.
                rb.velocity = new Vector2(25 * Mathf.Sqrt(dashPower) * facing * -1, 0);
                currentState = PlayerState.Idle;
            }
        }

        if (currentState == PlayerState.Guard && collision.gameObject.CompareTag("Player"))
        {

        }
    }

    void ChargeChecking()
    {
        if (Input.GetKeyDown(chargeKey))
        {
            isCharging = true;
            currentState = PlayerState.Idle;
        }
        else if (Input.GetKeyUp(chargeKey))
        {
            isCharging = false;
            currentState = PlayerState.Guard;
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(attackKey))
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
                attackDistance = Mathf.Log(attackChargeAmount) * dashPower * dashPower; 
                ChargeAmount -= 25;
            }
            else
            {
                Debug.Log("Punch");
            }

        }
    }

    void DashWithPhysics()
    {
        float moveStep = Mathf.Sqrt(dashPower) * 30 * Time.fixedDeltaTime; // FixedUpdate 기준 이동
        attackDistance -= moveStep;
        // Debug.Log(attackDistance);

        if (attackDistance <= 0.0f)
        {
            currentState = PlayerState.Idle; // 이동 종료
            moveStep += attackDistance; // 초과 이동 방지
        }

        Vector3 velocity = rb.velocity;
        velocity.x = moveStep / Time.fixedDeltaTime * facing;
        rb.velocity = velocity;
    }

    void Manage_Y() // Managing Y axis
    {
        if (transform.position.y <= maxY)
        {
            maxY = transform.position.y;
        }

        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
    }
}


