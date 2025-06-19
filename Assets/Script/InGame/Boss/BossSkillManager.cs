using System.Collections;
using System.Collections.Generic;
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
    [Header("보스 총 맞음")]
    public GameObject BossGunHit;
    [Header("보스 스턴")]
    public GameObject BossStun;
    [Header("보스 사망 이미지")]
    public GameObject BossDeadImage;

    [Header("생성할 보스 구멍 프리팹")]
    public GameObject holePrefab;
    [Header("조준점 프리팹")]
    public GameObject Aim;
    [Header("공중에서 내려오는 총알")]
    public GameObject BulletPrefabs;

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

    [Header("조준점 소리")]
    public AudioClip _AimSound;
    [Header("총 드는 소리")]
    public AudioClip _GunDrawSound;
    [Header("총 쏘는 소리")]
    public AudioClip _ShootSound;
    [Header("총 돌리는 소리")]
    public AudioClip _GunSpinSound;
    [Header("총알 땅에 박히는 소리")]
    public AudioClip _BulletImpactSound;
    [Header("불 소리")]
    public AudioClip _FireSound;
    [Header("보스 스턴 소리 (삐~)")]
    public AudioClip _StunSound;

    private List<Vector3> potentialHolePositions = new List<Vector3>();
    private List<GameObject> PatternTwoAims = new List<GameObject>();
    private List<GameObject> PatternThreeAims = new List<GameObject>();
    private List<GameObject> AllBulletBall = new List<GameObject>();

    private int BossCooldown1;
    private int BossCooldown2;
    private int BossCooldown3;
    private int aimCount;

    public static int StunCooldown = 0;

    public static bool isStuned = false;
    public static bool gotoNextTurn = false;
    public static bool StartStun = false;
    public static bool EndStun = false;
    public static bool bossAcivate = false;
    public static bool bossDeath = false;

    private void Start()
    {
        SetBossAnimation(1);

        BossCooldown1 = BossSkill1;
        BossCooldown2 = BossSkill2;
        BossCooldown3 = BossSkill3;

        StunCooldown = 0;
        isStuned = false;
    }

    void Update()
    {
        if (bossDeath)
        {
            bossDeath = false;
            UI.SetActive(false);
            StartCoroutine(PlayBossDeathAnimation());
        }

        if (StartStun)
        {
            StartStun = false;
            UI.SetActive(false);
            StartCoroutine(activateStun());
        }

        if (EndStun)
        {
            EndStun = false;
            UI.SetActive(false);
            StartCoroutine(endStun());
        }


        if (gotoNextTurn)
        {
            gotoNextTurn = false;
            bossAcivate = true;

            GameObject[] bossHoles = GameObject.FindGameObjectsWithTag("BossHole");
            if (bossHoles.Length > 0)
            {
                foreach (GameObject bossHole in bossHoles)
                {
                    Destroy(bossHole);
                }
            }
            AllBulletBall.Clear();
            GameObject[] foundBulletBalls = GameObject.FindGameObjectsWithTag("BulletBallTagMarker");
            foreach (GameObject ball in foundBulletBalls)
            {
                AllBulletBall.Add(ball);
            }

            BossCooldown1--;
            BossCooldown2--;
            BossCooldown3--;
            Debug.Log($"현재 보스 쿨다운: Skill1={BossCooldown1}, Skill2={BossCooldown2}, Skill3={BossCooldown3}");

            if (BossCooldown1 <= 0 || BossCooldown2 <= 0 || BossCooldown3 <=0 || AllBulletBall.Count >= 1)
            {
                UI.SetActive(false);
                StartCoroutine(activateSkill());
                
            }
            else
            {
                DataManager.isGameActionable = true;
            }
        }
    }

    public static void NextTurn()
    {
        Debug.Log($"현 StunCooldown - {StunCooldown}");
        if (isStuned)
        {
            StunCooldown--;
            if (StunCooldown == 0)
            {
                EndStun = true;
            }
            else
            {
                DataManager.isGameActionable = true;
            }
            return;
        }
        Debug.Log($"P - 1");
        gotoNextTurn = true;
    }

    public static void StartBossStun(int SetStunLength)
    {
        if (isStuned || SetStunLength < 0) // 이미 스턴이거나 값이 잘못되면 무시
        {
            return;
        }

        StunCooldown += SetStunLength;
        StartStun = true;
        isStuned = true;
    }


    public static void PlayBossDeath()
    {
        bossDeath = true;
    }

    IEnumerator activateStun()
    {
        yield return CameraMovement.LerpGoto(new Vector3(0, 1.35f, -10), 1.35f, 0.3f);
        SetBossAnimation(7);
        SoundManager.PlaySound(_GunDrawSound);
        yield return new WaitForSeconds(0.75f);
        CameraMovement.Shake(0.05f, 0.15f, 0.025f);
        SoundManager.PlaySound(_ShootSound);
        yield return new WaitForSeconds(0.1f);
        SetBossAnimation(8);
        SoundManager.PlaySound(_StunSound);
        yield return new WaitForSeconds(0.5f);
        CameraMovement.LerpGoto(new Vector3(0, 0, -10), 3f, 0.4f);
        yield return new WaitForSeconds(0.65f);

        DataManager.isGameActionable = true;
        UI.SetActive(true);
    }


    IEnumerator endStun()
    {
        // 1-2-4-1
        CameraMovement.Goto(new Vector3(0, 1.3f, -10), 1);
        yield return CameraMovement.LerpGoto(new Vector3(0, 1.3f, -10), 0.7f, 1f);
        SetBossAnimation(1);
        yield return CameraMovement.LerpGoto(new Vector3(0, 1.3f, -10), 1.5f, 0.25f);
        yield return new WaitForSeconds(0.3f);
        SetBossAnimation(2);
        CameraMovement.LerpGoto(new Vector3(-1.15f, 1.3f, -10), 0.9f, 0.15f);
        yield return new WaitForSeconds(0.28f);
        SetBossAnimation(4);
        SoundManager.PlaySound(_GunSpinSound);
        yield return new WaitForSeconds(0.28f);
        SoundManager.PlaySound(_GunSpinSound);
        yield return new WaitForSeconds(0.28f);
        SetBossAnimation(1);
        CameraMovement.LerpGoto(new Vector3(0, 0, -10), 3, 0.45f);
        yield return new WaitForSeconds(0.3f);


        isStuned = false;
        DataManager.isGameActionable = true;
        UI.SetActive(true);
    }

    IEnumerator PlayBossDeathAnimation()
    {
        yield return CameraMovement.LerpGoto(new Vector3(0, 1.3f, -10), 1.5f, 0.25f);
        SetBossAnimation(5);
        yield return CameraMovement.Shake(0.01f, 1, 0.08f);
        yield return new WaitForSeconds(1f);
        SetBossAnimation(9);
        CameraMovement.LerpGoto(new Vector3(0.3f, 0.12f, -10), 0.1f, 1.9f);
        ScreenTransition.Goto("Cutscene_ED", 2f, 0.05f);
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
            yield return new WaitForSeconds(0.35f);
            SoundManager.PlaySound(_ShootSound, 1, 0.7f);
            yield return new WaitForSeconds(0.25f);

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
                yield return new WaitForSeconds(1f);
                SoundManager.PlaySound(_FireSound, 1, 1);
                yield return new WaitForSeconds(0.4f);
                CameraMovement.Shake(0.1f, 0.2f, 0.05f);
                yield return new WaitForSeconds(0.25f);

                SetBossAnimation(1);
                CalculateSummonPositions(1);
                Instantiate(BulletBall, potentialHolePositions[0], Quaternion.identity);
                Instantiate(BulletBallEffect, potentialHolePositions[0], Quaternion.identity);
                SoundManager.PlaySound(_FireSound, 1, 2);
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
                SoundManager.PlaySound(_AimSound);
                yield return new WaitForSeconds(0.255f);
            }
            yield return CameraMovement.LerpGoto(new Vector3(0,1.25f, -10), 1.5f, 0.4f);
            yield return new WaitForSeconds(0.1f);

            SetBossAnimation(2);
            SoundManager.PlaySound(_GunDrawSound);
            yield return new WaitForSeconds(0.28f);

            for (int i = 0; i < holeRandomCount; i++)
            {
                SetBossAnimation(3);
                CameraMovement.Shake(0.1f, 0.1f, 0.05f);
                SoundManager.PlaySound(_ShootSound);
                yield return new WaitForSeconds(0.21f);
            }
            SoundManager.PlaySound(_GunSpinSound);
            SetBossAnimation(4);
            yield return new WaitForSeconds(0.28f);


            SetBossAnimation(1);
            yield return CameraMovement.LerpGoto(new Vector3(0, -1, -10), 1.94f, 0.125f);

            for (int i = 0; i < holeRandomCount; i++)
            {
                CameraMovement.Shake(0.2f, 0.25f, 0.05f);
                Instantiate(BulletPrefabs, potentialHolePositions[i],Quaternion.identity);
                Instantiate(holePrefab, potentialHolePositions[i], Quaternion.identity);
                SoundManager.PlaySound(_BulletImpactSound);
                Destroy(PatternTwoAims[i]); // 리스트에 저장된 Aim 오브젝트 파괴
                PatternTwoAims[i] = null;
                yield return new WaitForSeconds(0.4f);
            }
            PatternTwoAims.Clear();
        }

        if (PatternThreeAims.Count > 0)
        {
            yield return CameraMovement.LerpGoto(new Vector3(0, -1, -10), 1.9f, 0.125f);

            for (int i = 0; i < PatternThreeAims.Count; i++)
            {
                // 이게 데채 왜 또 작동했는지 모르겠는데 일단됨 GG
                // 근데 로직상 작동하는 게 맞음
                // PatternThreeAims 수만큼 반복 + PatternThreeAims[i] 참조하기 때문에 맞을수밖에 없긴함
                if (BulletPrefabs != null)  
                {
                    
                    GameObject PatternThreeBullet = Instantiate(BulletPrefabs, PatternThreeAims[i].transform.position, Quaternion.identity);
                    PatternThreeBullet.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    Destroy(PatternThreeAims[i]);

                    //Collider[] hitColliders = Physics.OverlapSphere(PatternThreeAims[i].transform.position, 0.25f);
                    //foreach (Collider hitCollider in hitColliders)
                    //{
                    //    // 감지된 콜라이더의 GameObject가 "PlayerBall" 태그를 가지고 있는지 확인
                    //    if (hitCollider.gameObject.CompareTag("PlayerBall"))
                    //    {
                    //        GameManager.attemptsLeft--;
                    //        GameManager.attemptsText.text = GameManager.attemptsLeft.ToString();
                    //        Debug.Log("HIT!");
                    //        break;
                    //    }
                    //}

                    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(PatternThreeAims[i].transform.position, 0.175f);
                    foreach (Collider2D hitCollider in hitColliders)
                    {
                        if (hitCollider.transform.CompareTag("PlayerBall"))
                        {
                            GameManager.attemptsLeft--;
                            GameManager.attemptsText.text = GameManager.attemptsLeft.ToString();
                            Debug.Log("HIT!");
                            break;
                        }
                    }


                    PatternThreeAims[i] = null;
                    CameraMovement.Shake(0.1f, 0.075f, 0.05f);
                    SoundManager.PlaySound(_ShootSound, 1, 1.4f);

                    yield return new WaitForSeconds(0.05f);
                }
                else
                {
                    Debug.LogError($"{i} 번째 루프 NULL");
                }

            }
            PatternThreeAims.Clear();
            yield return new WaitForSeconds(0.5f);
            //transform.position = PatternThreeAims[0].transform.position;
        }

        if (BossCooldown3 <= 0)
        {
            BossCooldown3 = BossSkill3;
            int ballNumber = GameManager.ballNumber;
            if (ballNumber < 5)
            {
                aimCount = 1;
            }
            else
            {
                switch (ballNumber)
                {
                    case 5:
                        aimCount = 3;
                        break;
                    case 6:
                        aimCount = 5;
                        break;
                    case 7:
                        aimCount = 20;
                        break;
                    case 8:
                        aimCount = 30;
                        break;
                }
            }
            
            yield return CameraMovement.LerpGoto(new Vector3(0, -1.25f, -10), 1.7f, 0.25f);

            CalculateSummonPositions(aimCount);
            PatternThreeAims.Clear();
            for (int i = 0; i < aimCount; i++)
            {
                GameObject newAim = Instantiate(Aim, potentialHolePositions[i], Quaternion.identity);
                newAim.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                PatternThreeAims.Add(newAim);
                SoundManager.PlaySound(_AimSound, 1, 1.2f);
                yield return new WaitForSeconds(0.1f);
            }
            
        }

        yield return CameraMovement.LerpGoto(new Vector3(0, 0, -10), 3, 0.2f);
        DataManager.isGameActionable = true;
        bossAcivate = false;
        GameManager.canPlay = false;
        GameManager.BossSkip = true;

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

        int maxAttempts = count * 100; // 무한 루프 방지를 위한 최대 시도 횟수 (원래의 백 배)
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
        else if (AnimationKey == 7)
        {
            GameObject newAnimation = Instantiate(BossGunHit);
            newAnimation.transform.SetParent(this.transform);
        }
        else if (AnimationKey == 8)
        {
            GameObject newAnimation = Instantiate(BossStun);
            newAnimation.transform.SetParent(this.transform);
        }
        else if (AnimationKey == 9)
        {
            GameObject newAnimation = Instantiate(BossDeadImage);
            newAnimation.transform.SetParent(this.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.175f);
    }
}