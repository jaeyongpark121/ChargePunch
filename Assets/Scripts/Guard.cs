using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Guard : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f; // 효과 지속 시간
    [SerializeField] private float elapsedTime = 0f;
    public GameObject guard;
    SpriteRenderer guardSpriteRenderer;

    void Start()
    {
        // 일정 시간이 지나면 오브젝트 파괴
        guardSpriteRenderer = guard.GetComponent<SpriteRenderer>();
        guardSpriteRenderer.color = new Color(guardSpriteRenderer.color.r, guardSpriteRenderer.color.g, guardSpriteRenderer.color.b, 1f);
    }

    void Update()
    {
        if (elapsedTime < duration)
        {
            // 경과 시간 누적
            elapsedTime += Time.deltaTime;

            // 투명도 계산 (시간에 따라 점점 감소)
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            guardSpriteRenderer.color = new Color(guardSpriteRenderer.color.r, guardSpriteRenderer.color.g, guardSpriteRenderer.color.b, alpha);
        }
        else
        {
            // 일정 시간이 지나면 guard 삭제
            Destroy(guard.transform.parent.gameObject);
        }
    }
}
