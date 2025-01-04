using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Guard : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f; // ȿ�� ���� �ð�
    [SerializeField] private float elapsedTime = 0f;
    public GameObject guard;
    SpriteRenderer guardSpriteRenderer;

    void Start()
    {
        // ���� �ð��� ������ ������Ʈ �ı�
        guardSpriteRenderer = guard.GetComponent<SpriteRenderer>();
        guardSpriteRenderer.color = new Color(guardSpriteRenderer.color.r, guardSpriteRenderer.color.g, guardSpriteRenderer.color.b, 1f);
    }

    void Update()
    {
        if (elapsedTime < duration)
        {
            // ��� �ð� ����
            elapsedTime += Time.deltaTime;

            // ���� ��� (�ð��� ���� ���� ����)
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            guardSpriteRenderer.color = new Color(guardSpriteRenderer.color.r, guardSpriteRenderer.color.g, guardSpriteRenderer.color.b, alpha);
        }
        else
        {
            // ���� �ð��� ������ guard ����
            Destroy(guard.transform.parent.gameObject);
        }
    }
}
