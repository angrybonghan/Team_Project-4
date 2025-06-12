using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [Header("���� ���õ� ����")]
    public int selectedLevel = 1;
    [Header("���� ������ ��ġ")]
    public GameObject[] levelCoin;
    [Header("ī�޶�")]
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
        CoinSet(); // �ʱ� ���� ���� ����
    }


    void Update()
    {
        // ī�޶� ��ġ ����
        CameraTargetPos = new Vector3(levelPos.x,levelPos.y,levelPos.z-10f);
        Camera.transform.position = Vector3.Lerp(Camera.transform.position, CameraTargetPos, CameraSpeed*Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow)) // ���� ȭ��ǥ : ���õ� �������� ����
        {
            if (selectedLevel == 1)
            {
                return;
            }
            selectedLevel--;
            CoinSet();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) // ������ ȭ��ǥ : ���õ� �������� ����
        {
            if (selectedLevel == 8)
            {
                return;
            }
            selectedLevel++;
            CoinSet();
        }

        if (Input.GetKeyDown(KeyCode.Return)) // ���� Ű ���� : �ش� ������ �̵�
        {
            SceneManager.LoadScene("Stage_" + selectedLevel);
        }
    }

    void CoinSet() // ������ ���� ����
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


// ���� ��������
// ������ �����Ѵ�!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// ���ǳ�������ִ°�? = hamburger;