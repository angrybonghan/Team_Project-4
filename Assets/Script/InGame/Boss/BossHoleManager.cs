using UnityEngine;
using System.Collections;

public class BossHoleManager : MonoBehaviour
{
    [Header("이 구멍의 애니메이션 프리팹")]
    public GameObject animationPrefabs;

    [Header("8볼 프리팹")]
    public GameObject eightBallPrefabs;

    private Rigidbody2D rb;

    private Vector3 originalLocalScale;
    private float scaleUpDuration = 0.2f;

    private void Awake()
    {
        originalLocalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUpCoroutine(originalLocalScale, scaleUpDuration));
    }

    private IEnumerator ScaleUpCoroutine(Vector3 targetScale, float duration)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "MergeBall":   // 이 구멍에 머지볼이 들어옴
                PlayAnimation(other.gameObject); // 애니메이션 실행 함수
                Destroy(other.gameObject);
                GameManager.scoredBallInChalk++; //현재 초크에 들어간 공의 수를 늘리기
                displayBall.DisplayBallCount++;
                break;
            case "PlayerBall":  // 플레이어 공이 구멍에 들어가면 그냥 저멀리 보내버림
                PlayAnimation(other.gameObject);
                if (BallController.isShieldExistence) // 방어막이 있을 경우, 점수를 1 올려 상쇄
                {
                    SkillGuideManager.summonGuidePaper("초크 추가 감소 방어됨!");
                    GameManager.attemptsLeft++;
                    BallController.unShield();
                }
                else
                {
                    SkillGuideManager.summonGuidePaper("초크 1 추가 감소");
                }

                GameManager.attemptsLeft--; // 플레이어 공이 들어갈 시 남은 초크 --
                GameManager.attemptsText.text = GameManager.attemptsLeft.ToString(); // 텍스트 UI 업데이트
                rb = other.GetComponent<Rigidbody2D>();
                other.transform.position = new Vector2(999999, 999999);
                rb.velocity = Vector2.zero;
                
                break;

            case "8Ball":
                PlayAnimation(other.gameObject);
                Destroy(other.gameObject);

                if (GameManager.isBallEight)
                {
                    GameManager.isGameOver = true;
                }
                else
                {
                    GameManager.isGameOver = true;
                }
                break;

            case "OB_Level_Up": //레벨 업 볼
                PlayAnimation(other.gameObject); // 애니메이션 실행
                Destroy(other.gameObject);
                GameManager.scoredBallInChalk += 2; //현재 초크에 들어간 공의 수를 두개 늘리기
                displayBall.DisplayBallCount += 2;
                SkillGuideManager.summonGuidePaper("보드에 볼 두 개 추가");
                break;

            case "OB_Level_Down":   //레벨 다운 볼
                PlayAnimation(other.gameObject); // 애니메이션 실행
                Destroy(other.gameObject);
                GameManager.isBallEight = false;
                if (GameManager.ballNumber >= 2)    // 공 레벨이 0 아래로 내려가지는 않음
                {
                    GameManager.ballNumber -= 2;    // 전체 공 레벨을 2 내린다
                    GameManager.scoredBallInChalk++;    // 초크에 공을 하나 넣은 판정을 준다 (공 업데이트를 시키기 위함)
                    // 고로 -2 + 1 = -1
                    // 최종적으로 1만 빠짐
                }

                SkillGuideManager.summonGuidePaper("레벨 1 감소");
                break;

            case "OB_Copy":
                PlayAnimation(other.gameObject); // 애니메이션 실행

                for (int i = 0; i < 2; i++)
                {
                    Vector3 CopyPos = new Vector3
                        (
                        UnityEngine.Random.Range(GameManager.boardMaxX-1, GameManager.boardMinX+1),
                        UnityEngine.Random.Range(GameManager.boardMaxY-1, GameManager.boardMinY+1),
                        0
                        );
                    GameObject EightBall = Instantiate(eightBallPrefabs, CopyPos, transform.rotation);
                }
                Destroy(other.gameObject);

                SkillGuideManager.summonGuidePaper("8볼 복제");
                break;

            case "OB_Chaos":
                PlayAnimation(other.gameObject); // 애니메이션 실행
                Destroy(other.gameObject);
                GameManager.isChaosBallActivate = true;
                SkillGuideManager.summonGuidePaper("뒤죽박죽");
                break;

            case "OB_SpeedUp":
                PlayAnimation(other.gameObject); // 애니메이션 실행
                Destroy(other.gameObject);
                GameManager.SpeedBallChance = 2;
                SkillGuideManager.summonGuidePaper("다음에 치는 힘 증가");
                break;

            case "OB_Shield":
                PlayAnimation(other.gameObject); // 애니메이션 실행
                Destroy(other.gameObject);
                BallController.GetShield();
                SkillGuideManager.summonGuidePaper("초크 감소 1회 방어");
                break;

            case "OB_NoFunction":
                PlayAnimation(other.gameObject); // 애니메이션 실행
                Destroy(other.gameObject);
                break;

            default:
                Debug.LogError("[???] 태그가 뭣도 아닌 것이 구멍에 들어옴");
                break;
        }
        GameManager.canPlay = false;
        if (BossSkillManager.bossAcivate)
        {
            GameManager.BossSkip = true;
        }
    }

    void PlayAnimation(GameObject targetObject)
    {
        Vector2 spawnPosition = transform.position;
        Vector2 direction = targetObject.transform.position - (Vector3)spawnPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg+90;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        GameObject spawnedAnimation = Instantiate(animationPrefabs, spawnPosition, targetRotation);
    }
}
