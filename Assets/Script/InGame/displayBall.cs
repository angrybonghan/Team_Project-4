using UnityEngine;

public class displayBall : MonoBehaviour
{
    [Header("ǥ�ÿ� �� ������")]
    public GameObject displayBallPrefab;

    [Header("���� ���� ���� ĭ�� �߾� ��ġ")]
    public Vector2 startPosition = new Vector2(-96.41f, -13f);

    [Header("ĭ�� ĭ ������ �Ÿ�")]
    public float cellGap = 24.1025f;

    public static int DisplayBallCount = 0;

    void Awake()
    {
        DisplayBallReset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DisplayBallCount++;
        }

        if (DisplayBallCount > 0)
        {
            DisplayBallCount--;
            SpawnDisplayBall();
        }
    }

    public void DisplayBallReset() // ���� UI�� �����ϴ� ǥ�ÿ� �� ����
    {
        transform.position = startPosition; // ������ ��Ŀ ��ġ ������
        DisplayBallCount = 0;

        GameObject[] displayBalls = GameObject.FindGameObjectsWithTag("DisplayBall");
        foreach (GameObject ball in displayBalls)
        {
            Destroy(ball);
        }
    }

    public void SpawnDisplayBall()
    {
        GameObject Display = Instantiate(displayBallPrefab, new Vector2(transform.position.x, 3.5f), transform.rotation);
        Display.transform.SetParent(this.transform.parent, false);
        transform.position = new Vector2(transform.position.x + cellGap, transform.position.y);
    }
}
