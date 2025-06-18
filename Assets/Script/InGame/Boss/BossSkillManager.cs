using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossSkillManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject UI;

    [Header("패턴 주기")]
    public int BossSkill1 = 6;
    public int BossSkill2 = 3;
    public int BossSkill3 = 2;

    [Header("보스 Idle")]
    public GameObject BossIdle;
    [Header("보스 GunUp")]
    public GameObject BossGunUp;
    [Header("보스 GunShot")]
    public GameObject BossGunShot;
    [Header("보스 GunSpin")]
    public GameObject BossGunSpin;
    [Header("보스 Dead")]
    public GameObject BossDead;
    [Header("보스 BallThrow")]
    public GameObject BossBallThrow;

    [Header("생성할 보스 구멍 프리팹")]
    public GameObject holePrefab;
    [Header("조준점 프리팹")]
    public GameObject Aim;
    [Header("공중에서 내려오는 총알")]
    public GameObject FireToHole;

    [Header("구멍 생성 범위")]
    public float minX = -2.5f;
    public float maxX = 2.5f;
    public float maxY = -2.5f;
    public float minY = -0.2f;
    [Header("구멍과 공의 최소 거리")]
    public float minDistanceToBall = 0.2f;

    [Header("총알 볼")]
    public GameObject BulletBall;
    [Header("총알 볼 생성 이펙트")]
    public GameObject BulletBallEffect;
    [Header("총알 볼이 생성하는 총알")]
    public GameObject BulletBallBullet;

    private List<Vector3> potentialHolePositions = new List<Vector3>();
    private List<GameObject> PatternTwoAims = new List<GameObject>();
    private List<GameObject> PatternThreeAims = new List<GameObject>();
    private List<GameObject> AllBulletBall = new List<GameObject>();

    private int BossCooldown1;
    private int BossCooldown2;
    private int BossCooldown3;
    private int aimCount;


    public static bool gotoNextTurn = false;
    public static bool bossAcivate = false;

    private void Start()
    {
        SetBossAnimation(1);

        BossCooldown1 = BossSkill1;
        BossCooldown2 = BossSkill2;
        BossCooldown3 = BossSkill3;
    }

    void Update()
    {
        if (gotoNextTurn)
        {
            gotoNextTurn=false;
            bossAcivate = true;


            GameObject[] bossHoles = GameObject.FindGameObjectsWithTag("BossHole");
            if (bossHoles.Length > 0)
            {
                foreach (GameObject bossHole in bossHoles)
                {
                    Destroy(bossHole);
                }
            }

            BossCooldown1--;
            BossCooldown2--;
            BossCooldown3--;

            AllBulletBall.Clear();
            GameObject[] foundBulletBalls = GameObject.FindGameObjectsWithTag("BulletBallTagMarker");
            foreach (GameObject ball in foundBulletBalls)
            {
                AllBulletBall.Add(ball);
            }
            //AllBulletBall = new GameObject.FindGameObjectsWithTag("BulletBallTagMarker");

            if (BossCooldown1 != 0 && BossCooldown2 != 0 && BossCooldown3 != 0 && AllBulletBall.Count == 0)
            {
                DataManager.isGameActionable = true;
            }
            else
            {
                UI.SetActive(false);
                StartCoroutine(activateSkill());
            }
        }
    }

    public static void NextTurn()
    {
        gotoNextTurn = true;
    }

    IEnumerator activateSkill()
    {
        if (AllBulletBall.Count >= 1)
        {

            yield return CameraMovement.LerpGoto(new Vector3(0, -1, -10), 1.94f, 0.125f);

            Quaternion[] directions = new Quaternion[]
            {
                Quaternion.Euler(0, 0, 90),  // 위쪽
                Quaternion.Euler(0, 0, 0),   // 오른쪽
                Quaternion.Euler(0, 0, 270), // 아래쪽
                Quaternion.Euler(0, 0, 180)  // 왼쪽
            };

            for (int i = 0; i < AllBulletBall.Count; i++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Instantiate(BulletBallBullet, AllBulletBall[i].transform.position, directions[x]);
                }
            }
            yield return new WaitForSeconds(0.6f);

            while (true) //내부에서 break 조건으로 제어
            {
                bool anyBallCurrentlyMoving = false;
                BallDeceleration[] allBilliardGameObjects = FindObjectsOfType<BallDeceleration>();

                foreach (BallDeceleration ball in allBilliardGameObjects)
                {
                    Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
                    if (rb != null && rb.IsAwake())
                    {
                        anyBallCurrentlyMoving = true;
                        break;
                    }
                }

                if (anyBallCurrentlyMoving)
                {
                    yield return new WaitForFixedUpdate();
                }
                else 
                {
                    break;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        if (BossCooldown2 <= 0)
        {
            // GameObject.FindGameObjectsWithTag("BulletBallTagMarker").Length < 2
            // ↑ 얘는 안된대;;

            BossCooldown2 = BossSkill2;
            GameObject[] foundBulletBalls = GameObject.FindGameObjectsWithTag("BulletBallTagMarker");
            if (foundBulletBalls.Length < 2)
            {
                yield return CameraMovement.LerpGoto(new Vector3(0, 1.25f, -10), 1.5f, 0.4f);
                SetBossAnimation(6);
                yield return new WaitForSeconds(1.4f);
                CameraMovement.Shake(0.1f, 0.2f, 0.05f);
                yield return new WaitForSeconds(0.25f);

                SetBossAnimation(1);
                CalculateSummonPositions(1);
                Instantiate(BulletBall, potentialHolePositions[0], Quaternion.identity);
                Instantiate(BulletBallEffect, potentialHolePositions[0], Quaternion.identity);
                yield return CameraMovement.LerpGoto(new Vector3(potentialHolePositions[0].x, potentialHolePositions[0].y, -10f), 0.5f, 0.2f);
                yield return new WaitForSeconds(0.75f);
            }
        }


        if (BossCooldown1 <= 0)
        {
            BossCooldown1 = BossSkill1;

            int holeRandomCount = Random.Range(2, 4);
            CalculateSummonPositions(holeRandomCount);

            for (int i = 0; i < holeRandomCount; i++)
            {
                yield return CameraMovement.LerpGoto(new Vector3(potentialHolePositions[i].x, potentialHolePositions[i].y,-10f), 0.75f, 0.2f);
                GameObject newAim = Instantiate(Aim, potentialHolePositions[i], Quaternion.identity);
                PatternTwoAims.Add(newAim);
                yield return new WaitForSeconds(0.255f);
            }
            yield return CameraMovement.LerpGoto(new Vector3(0,1.25f, -10), 1.5f, 0.4f);
            yield return new WaitForSeconds(0.1f);

            SetBossAnimation(2);
            yield return new WaitForSeconds(0.28f);

            for (int i = 0; i < holeRandomCount-1; i++)
            {
                SetBossAnimation(3);
                CameraMovement.Shake(0.1f, 0.1f, 0.05f);
                yield return new WaitForSeconds(0.28f);
            }

            SetBossAnimation(4);
            yield return new WaitForSeconds(0.28f);

            SetBossAnimation(1);
            yield return CameraMovement.LerpGoto(new Vector3(0, -1, -10), 1.94f, 0.125f);

            for (int i = 0; i < holeRandomCount; i++)
            {
                CameraMovement.Shake(0.2f, 0.25f, 0.05f);
                Instantiate(FireToHole, potentialHolePositions[i],Quaternion.identity);
                Instantiate(holePrefab, potentialHolePositions[i], Quaternion.identity);
                Destroy(PatternTwoAims[i]); // 리스트에 저장된 Aim 오브젝트 파괴
                PatternTwoAims[i] = null;
                yield return new WaitForSeconds(0.4f);
            }
            PatternTwoAims.Clear();
        }

        if (BossCooldown3 == 0)
        {
            if (GameManager.ballNumber >= 6)
            {
                int ballNumber = GameManager.ballNumber;
                switch (ballNumber)
                {
                    case 6:
                        aimCount = 3;
                        break;
                    case 7:
                        aimCount = 5;
                        break;
                    case 8:
                        aimCount = 10;
                        break;
                }
                yield return CameraMovement.LerpGoto(new Vector3(0, -1.25f, -10), 1.7f, 0.25f);

                CalculateSummonPositions(aimCount);
                PatternThreeAims.Clear();
                for (int i = 0; i < aimCount; i++)
                {
                    GameObject newAim = Instantiate(Aim, potentialHolePositions[i], Quaternion.identity);
                    newAim.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                    PatternThreeAims.Add(newAim);
                    yield return new WaitForSeconds(0.1f);
                }

            }
            else
            {
                BossCooldown3 = BossSkill3;
            }
        }

        if (BossCooldown3 <= -1)
        {
            BossCooldown3 = BossSkill3;


        }

        yield return CameraMovement.LerpGoto(new Vector3(0, 0, -10), 3, 0.2f);
        DataManager.isGameActionable = true;
        bossAcivate = false;
        UI.SetActive(true);
    }


    public void CalculateSummonPositions(int count)
    {
        // 기존에 저장된 위치들 초기화
        potentialHolePositions.Clear();

        if (count <= 0)
        {
            return;
        }

        List<Vector2> currentDecelerationBallPositions = new List<Vector2>();
        BallDeceleration[] decelerationBalls = FindObjectsOfType<BallDeceleration>();
        foreach (BallDeceleration ball in decelerationBalls)
        {
            currentDecelerationBallPositions.Add(ball.transform.position);
        }

        int maxAttempts = count * 100; // 무한 루프 방지를 위한 최대 시도 횟수
        int currentAttempt = 0; //현재 시도하는 루프 수
        int positionsCalculated = 0; // 계산된 위치의 수

        while (positionsCalculated < count && currentAttempt < maxAttempts)
        {
            currentAttempt++;

            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector2 potentialPosition2D = new Vector2(randomX, randomY);

            bool isTooClose = false;
            foreach (Vector2 ballPos in currentDecelerationBallPositions)
            {
                if (Vector2.Distance(potentialPosition2D, ballPos) < minDistanceToBall)
                {
                    isTooClose = true;
                    break;
                }
            }

            if (!isTooClose)
            {
                Vector3 finalPosition = new Vector3(potentialPosition2D.x, potentialPosition2D.y, 0f);
                potentialHolePositions.Add(finalPosition); // 계산된 위치를 리스트에 저장

                positionsCalculated++;
            }
        }
    }

    public void SetBossAnimation(int AnimationKey)
    {
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        if (AnimationKey == 1)
        {
            GameObject newAnimation = Instantiate(BossIdle);
            newAnimation.transform.SetParent(this.transform);
        }
        else if (AnimationKey == 2)
        {
            GameObject newAnimation = Instantiate(BossGunUp);
            newAnimation.transform.SetParent(this.transform);
        }
        else if (AnimationKey == 3)
        {
            GameObject newAnimation = Instantiate(BossGunShot);
            newAnimation.transform.SetParent(this.transform);
        }
        else if (AnimationKey == 4)
        {
            GameObject newAnimation = Instantiate(BossGunSpin);
            newAnimation.transform.SetParent(this.transform);
        }
        else if (AnimationKey == 5)
        {
            GameObject newAnimation = Instantiate(BossDead);
            newAnimation.transform.SetParent(this.transform);
        }
        else if (AnimationKey == 6)
        {
            GameObject newAnimation = Instantiate(BossBallThrow);
            newAnimation.transform.SetParent(this.transform);
        }
    }
}