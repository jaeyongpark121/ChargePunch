using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static PlayerController;

public class CollisionManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CollisionManager Instance;

    public Collider2D playerCollider1;
    public Collider2D playerCollider2;
    public Collider2D ground;

    public PlayerController playerController1;
    public PlayerController playerController2;

    public GameObject guardEffectPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerCollider1 = playerController1.GetComponent<Collider2D>();
        playerCollider2 = playerController2.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if(playerCollider1.IsTouching(playerCollider2))
        {
            HandleCollision(playerController1,playerController2);
        }
    }

    // Update is called once per frame
    private void HandleCollision(PlayerController player1, PlayerController player2)
    {
        float attackPower1 = player1.dashPower + Mathf.Sqrt(Mathf.Abs(player1.GetComponent<Rigidbody2D>().linearVelocity.x));
        float attackPower2 = player2.dashPower + Mathf.Sqrt(Mathf.Abs(player2.GetComponent<Rigidbody2D>().linearVelocity.x));

        if (player1.currentState == PlayerState.Dash && player2.currentState == PlayerState.Dash)
        {
            
            // �� �� Dash ������ ��
            if (player1.dashPower > player2.dashPower)
            {
                VelocityLoss(player1);
                KnockBack(player2 , attackPower1);
            }
            else if (player1.dashPower < player2.dashPower)
            {
                VelocityLoss(player1);
                KnockBack(player1, attackPower2);
            }
            else
            {
                KnockBack(player1, attackPower1);
                KnockBack(player2, attackPower2);
            }
        }
        // ���ʸ� Dash ������ ��
        else if (player1.currentState == PlayerState.Dash && player2.currentState == PlayerState.Idle)
        {
            VelocityLoss(player1);
            KnockBack(player2, attackPower1);
        }
        else if (player2.currentState == PlayerState.Dash && player1.currentState == PlayerState.Idle)
        {
            VelocityLoss(player2);
            KnockBack(player1, attackPower2);
        }

        // ���� �����϶�
        else if (player1.currentState == PlayerState.Dash && player2.currentState == PlayerState.Guard)
        {
            KnockBack(player1, player1.dashPower);
            player2.GetComponent<Rigidbody2D>().linearVelocity = new Vector2 (0, 0);
            GameObject guardEffect = Instantiate(guardEffectPrefab, player2.gameObject.transform.position + new Vector3(0.5f * player2.facing,0,0), Quaternion.identity, player2.transform);
        }
        else if (player2.currentState == PlayerState.Dash && player1.currentState == PlayerState.Guard)
        {
            KnockBack(player2, player2.dashPower);
            player1.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
            GameObject guardEffect = Instantiate(guardEffectPrefab, player1.gameObject.transform.position + new Vector3(0.5f * player1.facing, 0, 0), Quaternion.identity, player1.transform);
        }
        player1.currentState = PlayerState.Idle;
        player2.currentState = PlayerState.Idle;
    }

    private void KnockBack(PlayerController player, float power)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(25 * Mathf.Sqrt(power) * player.facing * -1, 0);
    }
    private void VelocityLoss(PlayerController player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = rb.linearVelocity * 0.5f;
    }
}
