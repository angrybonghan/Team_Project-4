using System.Collections.Generic;
using System.Linq;
using SeongnamSiGyeonggiDoSouthKorea;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool anyBallMoving;
    public static bool canPlay = true;

    void Start()
    {
        
    }


    void Update()
    {
        if (!canPlay)
        {
            CheckAllBalls();
            if (!anyBallMoving)
            {
                canPlay = true;
                Debug.Log("�÷��� ����");
            }
        }
    }

    void CheckAllBalls()
    {
        anyBallMoving = false; // ������ ����

        GameObject[] ballObjects = GameObject.FindGameObjectsWithTag("Ball");
        // Ball �±� ���� ��� GameObject ã�� �迭�� �ھƵ�
        GameObject[] playerBallObjects = GameObject.FindGameObjectsWithTag("PlayerBall");
        // PlayerBall �±� ���� ��� GameObject ã�� �迭�� �ھƵ�

        GameObject[] allBilliardGameObjects = ballObjects.Concat(playerBallObjects).ToArray();
        // �� GameObject �迭�� �ϳ��� ��ħ (Concat)

        foreach (GameObject obj in allBilliardGameObjects) // GameObject foreach
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>(); // ������ Rigidbody2D ��
            if (rb != null && Mathf.Abs(rb.velocity.magnitude) >= 0.5f) // �ش� ���� �����̰� �ִ��� Ȯ��
            {
                anyBallMoving = true; // �ϳ��� �����̸� true
                break; // �ϳ� ������������ �ٸ��� üũ �ʿ� ��, foreach ����
            }
        }
    }
}
