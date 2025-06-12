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

    private Vector3 levelPos;
    private Vector3 CameraTargetPos;

    void Awake()
    {
        levelPos = levelCoin[selectedLevel-1].transform.position;
        transform.position = levelPos;
        Camera.transform.position = levelPos;
    }
    private void Start()
    {
        CoinSet(); // 초기 코인 형태 설정
    }


    void Update()
    {
        // 카메라 위치 결정
        CameraTargetPos = new Vector3(levelPos.x,levelPos.y,levelPos.z-10f);
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, CameraTargetPos, CameraSpeed*Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow)) // 왼쪽 화살표 : 선택된 스테이지 감소
        {
            if (selectedLevel == 1)
            {
                return;
            }
            selectedLevel--;
            CoinSet();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) // 오른쪽 화살표 : 선택된 스테이지 증가
        {
            if (selectedLevel == 8)
            {
                return;
            }
            selectedLevel++;
            CoinSet();
        }

        if (Input.GetKeyDown(KeyCode.Return)) // 엔터 키 감지 : 해당 신으로 이동
        {
            SceneManager.LoadScene("Stage_" + selectedLevel);
        }
    }

    void CoinSet() // 코인의 형태 결정
    {
        foreach (GameObject coin in levelCoin)
        {
            coin.SetActive(true);
        }
        levelCoin[selectedLevel - 1].SetActive(false);

        levelPos = levelCoin[selectedLevel - 1].transform.position;
        transform.position = levelPos;
    }
}


// 밤을 새버려서
// 지능이 감소한다!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// 나의뇌가살아있는가? = hamburger;