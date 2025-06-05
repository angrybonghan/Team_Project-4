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
                Debug.Log("플레이 가능");
            }
        }
    }

    void CheckAllBalls()
    {
        anyBallMoving = false; // 안전빵 리셋

        GameObject[] ballObjects = GameObject.FindGameObjectsWithTag("Ball");
        // Ball 태그 가진 모든 GameObject 찾아 배열에 박아둠
        GameObject[] playerBallObjects = GameObject.FindGameObjectsWithTag("PlayerBall");
        // PlayerBall 태그 가진 모든 GameObject 찾아 배열에 박아둠

        GameObject[] allBilliardGameObjects = ballObjects.Concat(playerBallObjects).ToArray();
        // 두 GameObject 배열을 하나로 합침 (Concat)

        foreach (GameObject obj in allBilliardGameObjects) // GameObject foreach
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>(); // 볼에서 Rigidbody2D 뽑
            if (rb != null && Mathf.Abs(rb.velocity.magnitude) >= 0.5f) // 해당 볼이 움직이고 있는지 확인
            {
                anyBallMoving = true; // 하나라도 움직이면 true
                break; // 하나 움직였음으로 다른건 체크 필요 없, foreach 폭파
            }
        }
    }
}
