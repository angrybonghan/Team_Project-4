using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class displayBall : MonoBehaviour
{
    public static displayBall Instance { get; private set; }

    [Header("ǥ�ÿ� �� ������")]
    public GameObject displayBallPrefab;

    [Header("���� ���� ���� ĭ�� �߾� ��ġ")]
    public static Vector2 startPosition = new Vector2(-96.41f, -13f);

    [Header("ĭ�� ĭ ������ �Ÿ�")]
    public float cellGap = 24.1025f;

    [Header("�޺� ǥ�ÿ� �ؽ�Ʈ")]
    public TextMeshProUGUI comboTextReference;
    public static TextMeshProUGUI ComboText; // static �� �ν����Ϳ� �� ���.

    public static int DisplayBallCount = 0;
    private static int existingDisplayBallCount=0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ComboText = comboTextReference;
        DisplayBallReset();
    }

    private void Update()
    {
        if (DisplayBallCount > 0)
        {
            DisplayBallCount--;
            SpawnDisplayBall(); // �� �Լ��� static�� �ƴϹǷ� Update���� ���� ȣ�� ����
        }
    }

    public static void DisplayBallReset() // ���� UI�� �����ϴ� ǥ�ÿ� �� ����
    {
        DisplayBallCount = 0;
        existingDisplayBallCount = 0;

        GameObject[] displayBalls = GameObject.FindGameObjectsWithTag("DisplayBall");
        foreach (GameObject ball in displayBalls)
        {
            Destroy(ball);
        }

        ComboText.gameObject.SetActive(false); // �ؽ�Ʈ ����
        ComboText.text = "UHH, NOPE.";
        Instance.transform.position = startPosition;
    }

    public void SpawnDisplayBall() // �� �ϳ� ����
    {
        GameObject Display = Instantiate(displayBallPrefab, new Vector2(transform.position.x, 3.5f), transform.rotation);
        Display.transform.SetParent(this.transform.parent, false);
        transform.position = new Vector2(transform.position.x + cellGap, transform.position.y);
        existingDisplayBallCount++;

        if (existingDisplayBallCount > 1 && GameManager.ballNumber < 8)
        {
            ComboText.gameObject.SetActive(true);
            ComboText.text = $"[COMBO �� {existingDisplayBallCount}]";
        }
    }
}