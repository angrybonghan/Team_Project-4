using UnityEngine;

public class BossSkillManager : MonoBehaviour
{
    [Header("패턴 1 주기")]
    public int BossSkill1 = 6;
    [Header("패턴 2 주기")]
    public int BossSkill2 = 3;
    [Header("패턴 3 주기")]
    public int BossSkill3 = 2;

    private int BossCooldown1;
    private int BossCooldown2;
    private int BossCooldown3;


    public static bool gotoNextTurn=false;

    private void Start()
    {
        BossCooldown1 = BossSkill1;
        BossCooldown2 = BossSkill2;
        BossCooldown3 = BossSkill3;
    }

    void Update()
    {
        if (gotoNextTurn)
        {
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


            DataManager.isGameActionable=true;
        }
    }

    public static void NextTurn()
    {
        gotoNextTurn = true;
    }
}
