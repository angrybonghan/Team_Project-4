using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("이 구멍의 애니메이션 프리팹")]
    public GameObject animationPrefabs;

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
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
    }
}
