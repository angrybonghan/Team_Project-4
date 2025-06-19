using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [Header("현재 선택된 레벨")]
    public int selectedLevel = 1;
    [Header("레벨 동전의 위치")]
    public GameObject[] levelCoin;
    [Header("카메라")]
    public GameObject Camera;
    public int CameraSpeed = 5;
    [Header("빨간 코인")]
    public Sprite RedCoin;
    [Header("뛰는 코인")]
    public GameObject JumpingCoin;

    private Vector3 levelPos;
    private Vector3 CameraTargetPos;
    private int levelAccess;

    void Awake()
    {
        levelPos = levelCoin[selectedLevel-1].transform.position;
        transform.position = levelPos;
        Camera.transform.position = levelPos;
    }
    private void Start()
    {
        levelAccess = DataManager.GetLevelAccess()-1;
        for (int i = 0; i < levelCoin.Length; i++)
        {
            if (i > levelAccess)
            {
                GameObject coin = levelCoin[i];
                SpriteRenderer spriteRenderer = coin.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = RedCoin;
            }
        }


        CoinSet();
    }


    void Update()
    {
        // 카메라 위치 결정
        CameraTargetPos = new Vector3(levelPos.x,levelPos.y,levelPos.z-10f);
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, CameraTargetPos, CameraSpeed*Time.deltaTime);

        if (DataManager.isGameActionable)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) // 왼쪽 화살표 : 선택된 스테이지 감소
            {
                if (selectedLevel == 1)
                {
                    return;
                }
                selectedLevel--;
                CoinSet();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) // 오른쪽 화살표 : 선택된 스테이지 증가
            {
                if (selectedLevel == 8)
                {
                    return;
                }
                selectedLevel++;
                CoinSet();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space)) // 엔터 키
            {
                int levelAccess = DataManager.GetLevelAccess(); // 레벨 입장 가능 여부 판단

                if (selectedLevel <= levelAccess)
                {
                    DataManager.SetPreviousLevel(selectedLevel);
                    ScreenTransition.Goto("Stage_" + selectedLevel, 0.5f, 0.5f);
                }
            }
        }
        
    }

    void CoinSet() // 코인의 형태 결정
    {
        foreach (GameObject coin in levelCoin)
        {
            coin.SetActive(true);
        }

        if (selectedLevel - 1 < DataManager.GetLevelAccess())
        {
            JumpingCoin.SetActive(true);
            levelCoin[selectedLevel - 1].SetActive(false);
        }
        else
        {
            JumpingCoin.SetActive(false);
            levelCoin[selectedLevel - 1].SetActive(true);
        }

        levelPos = levelCoin[selectedLevel - 1].transform.position;
        transform.position = levelPos;
    }
}


// 밤을 새버려서
// 지능이 감소한다!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// 나의뇌가살아있는가? = hamburger;