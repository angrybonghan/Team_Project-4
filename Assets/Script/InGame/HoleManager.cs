using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("게임매니저")]
    public GameManager gameManager;
    [Header("이 구멍의 애니메이션 프리팹")]
    public GameObject animationPrefabs;

    private Rigidbody2D rb;


    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "MergeBall":   // 이 구멍에 머지볼이 들어옴
                Destroy(other.gameObject);
                PlayAnimation(); // 애니메이션 실행 함수
                GameManager.scoredBallInChalk += 1; //현재 초크에 들어간 공의 수를 늘리기
                displayBall.DisplayBallCount += 1;
                break;
            case "PlayerBall":  // 플레이어 공이 구멍에 들어가면 그냥 저멀리 보내버림
                gameManager.attemptsLeft--; // 플레이어 공이 들어갈 시 남은 초크 --
                gameManager.attemptsText.text = gameManager.attemptsLeft.ToString(); // 텍스트 UI 업데이트
                rb = other.GetComponent<Rigidbody2D>();
                other.transform.position = new Vector2(0, 999999);
                rb.velocity = Vector2.zero;

                PlayAnimation();
                break;
            case "8Ball":
                PlayAnimation();
                Destroy(other.gameObject);

                if (GameManager.ballNumber == 8)
                {
                    GameManager.isGameWin = true;
                }
                else
                {
                    GameManager.isGameOver = true;
                }
                break;
            default:
                Debug.LogError("[!!!] 태그가 뭣도 아닌 것이 구멍에 들어옴");
                break;
        }
    }

    void PlayAnimation()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
    }
}
