using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("이 구멍의 번호 (1..6)")]
    public int holeID = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "MergeBall":   // 이 구멍에 머지볼이 들어옴
                Destroy(other.gameObject);
                PlayAnimation(); // 애니메이션 실행 함수
                GameManager.scoredBallInChalk += 1; //현재 초크에 들어간 공의 수를 늘리기
                break;
            case "PlayerBall":  // 플레이어 공이 구멍에 들어가면 GameManager.cs 의 게임 오버 호출
                GameManager.isGameOver = true;
                break;
        }
    }

    void PlayAnimation()
    {
        if (holeID == null) // 변수가 작성되지 않았을 경우, 즉시 반환
        {
            Debug.LogError($"ID 변수가 NULL 임");
            return;
        }

        switch (holeID) // 구멍의 위치 ID에 따른 다른 동작
        {
            case 1:
                Debug.Log("Hole 1"); // 구멍 1 : 왼쪽 위
                break;
            case 2:
                Debug.Log("Hole 2"); // 구멍 2 : 중앙 위
                break;
            case 3:
                Debug.Log("Hole 3"); // 구멍 3 : 오른쪽 위
                break;
            case 4:
                Debug.Log("Hole 4"); // 구멍 4 : 왼쪽 아래
                break;
            case 5:
                Debug.Log("Hole 5"); // 구멍 5 : 중앙 아래
                break;
            case 6:
                Debug.Log("Hole 6"); // 구멍 6 : 오른쪽 아래
                break;
            default:
                Debug.LogError($"ID 변수가 1부터 6 사이의 정수가 아님! // 현재 값 : {holeID}");
                // 혹시 모르니까 예외 처리.
                break;
        }
    }
}
