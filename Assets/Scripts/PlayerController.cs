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
    [SerializeField] private KeyCode attackKey = KeyCode.X; // Ű����
    [SerializeField] private KeyCode chargeKey = KeyCode.C; // Ű����

    [SerializeField] private GameObject chargeEffectPrefab;
    [SerializeField] private GameObject dashEffectPrefab;
    private ParticleSystem chargeParticleSystem;
    private ParticleSystem dashParticleSystem;

    public int facing = 1;
    private float maxY;
    public int dashPower;
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
        set { /*if (value < 0 || value > 100) Debug.LogWarning($"{value}�� ������ �Ѿ����ϴ�.");*/
            chargeAmount = Mathf.Clamp(value, 0, 100); } 
    }

    public PlayerState currentState = PlayerState.Guard;

    void Start()
    {
        chargeScript = GetComponent<Charge>();
        rb = GetComponent<Rigidbody2D>();

        chargeParticleSystem = Instantiate(chargeEffectPrefab, transform.position, Quaternion.Euler(-90, 0, 0) , this.transform).GetComponent<ParticleSystem>();
        chargeParticleSystem.Stop();

        dashParticleSystem = Instantiate(dashEffectPrefab, transform.position, Quaternion.Euler(0, -90 * facing, 0), this.transform).GetComponent<ParticleSystem>();
        dashParticleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        ChargeChecking();
        Attack();

        if (currentState == PlayerState.Dash) // ���� ���½� ����
        {
            DashWithPhysics();
            isCharging = false;
        }

        if (currentState != PlayerState.Dash && currentState != PlayerState.Damaged) // ����/�ĸ��� �ʾ����� �ӵ� ����
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.x *= 0.9f;
            rb.linearVelocity = velocity;
        }

        if( Mathf.Abs(rb.linearVelocity.x) < 1.0E-05f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if(currentState == PlayerState.Idle && rb.linearVelocity.x == 0 && !isCharging)
        {
            currentState = PlayerState.Guard;
        }

        if (currentState == PlayerState.Guard) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); }

        Manage_Particle();
    }
    void FixedUpdate()
    {
        if (isCharging /*&& chargeScript != null*/) // ����
        {
            chargeScript.AddCharge(0.5f);
        }

        Manage_Y(); // y�� �� ���� �ʰ� ����
    }

    /* �浹 ����
    void OnCollisionStay2D(Collision2D collision)
    {
        if (currentState == PlayerState.Dash && collision.gameObject.CompareTag("Player"))
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (otherPlayer.currentState == PlayerState.Idle)
            {
                // ��� �÷��̾� �б�
                otherRb.velocity = new Vector2(50 * Mathf.Sqrt(dashPower) * facing, 0); // �з����� �ӵ� ����
                otherPlayer.ChargeAmount -= 25;
                Debug.Log(gameObject.name + " attacked");
                currentState = PlayerState.Idle;
            }

            else if(otherPlayer.currentState == PlayerState.Dash)
            {
                // �������� �и���.
                if (otherPlayer.dashPower > dashPower)
                {
                    rb.velocity = new Vector2(50 * Mathf.Sqrt(dashPower) * facing * -1, 0);
                    Debug.Log(gameObject.name + " attacked");
                    currentState = PlayerState.Idle;
                }
                
                else if (otherPlayer.dashPower == dashPower)
                {
                    rb.velocity = new Vector2(25 * Mathf.Sqrt(dashPower) * facing * -1, 0);
                    //Debug.Log(gameObject.name);
                    Debug.Log(currentState);
                    Debug.Log(otherPlayer.currentState);
                    Debug.Log(gameObject.name + " power was same");
                    currentState = PlayerState.Idle;
                }
            }

            else if (otherPlayer.currentState == PlayerState.Guard)
            {
                // ����ϰ� ������ ���� �и���.
                rb.velocity = new Vector2(25 * Mathf.Sqrt(dashPower) * facing * -1, 0);
                Debug.Log(gameObject.name + " missed");
                currentState = PlayerState.Idle;
            }

        }

    }
    */

    void ChargeChecking()
    {
        if (Input.GetKeyDown(chargeKey) && currentState != PlayerState.Dash)
        {
            isCharging = true;
            currentState = PlayerState.Idle;
        }
        else if (Input.GetKeyUp(chargeKey))
        {
            isCharging = false;
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
                attackDistance = Mathf.Log(attackChargeAmount) * dashPower; 
                ChargeAmount -= 25;
            }
            else
            {
                Debug.Log("Punch");
            }
            // Debug.Log(currentState + "\n" + dashPower + "\n" + attackDistance + "\n");
        }
    }

    /*
    void DashWithPhysics()
    {
        float moveStep = (attackDistance / (4 + dashPower) <= 0.1) ? 0.1f : attackDistance / (4 + dashPower); // FixedUpdate ���� �̵�
        Debug.Log(moveStep+"\n"+attackDistance);
        attackDistance -= moveStep;
        // Debug.Log(attackDistance);

        if (Mathf.Abs(attackDistance) <= 0.05f)
        {
            currentState = PlayerState.Idle; // �̵� ����
            attackDistance = 0; // �ʰ� �̵� ����
        }
        
        Vector3 velocity = rb.velocity;
        velocity.x = moveStep / Time.fixedDeltaTime * facing;
        rb.velocity = velocity;
    }
    */
    void DashWithPhysics()
    {
        float initialSpeedFactor = 0.3f; // �ʱ� �ӵ� ���� (1�̸� �ִ�ӵ�)
        float decelerationRate = 0.8f;  // ���ӷ� (1�� �������� ������ ����)
        float minSpeed = 0.05f;         // �ּ� �̵� �Ÿ� ����

        // �ʱ� �ӵ� ���� (��� ���� �� ������ �ʰ� ����)
        float moveStep = attackDistance * initialSpeedFactor;

        // ���� ����
        moveStep *= decelerationRate;
        moveStep = Mathf.Max(moveStep, minSpeed); // �ּ� �̵� �Ÿ� ����

        // �̵� �Ÿ� ����
        attackDistance -= moveStep;

        // �̵� ���� ����
        if (Mathf.Abs(attackDistance) <= minSpeed)
        {
            currentState = PlayerState.Idle; // �̵� ����
            attackDistance = 0; // �ʰ� �̵� ����
        }

        // Rigidbody �̵� ����
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveStep / Time.fixedDeltaTime * facing; // �̵� ���� ����
        rb.linearVelocity = velocity;
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

    void Manage_Particle()
    {
        // ChargeParticle
        if (isCharging)
        {
            if (!chargeParticleSystem.isEmitting)
            {
                chargeParticleSystem.Play(); // ���� ���� �� ��ƼŬ ���
            }
        }
        else
        {
            if (chargeParticleSystem.isEmitting)
            {
                chargeParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        if (currentState == PlayerState.Dash)
        {
            if (!dashParticleSystem.isEmitting)
            {
                dashParticleSystem.Play(); 
            }
        }
        else
        {
            if (dashParticleSystem.isEmitting)
            {
                dashParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}


