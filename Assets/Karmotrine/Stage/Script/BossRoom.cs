using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private GameObject boss;

    public void SummonBoss()
    {
        foreach (GameObject hider in DoorHiders)
        {
            hider.SetActive(true);
        }

        foreach (GameObject particle in DoorParticles)
        {
            particle.SetActive(false);
        }

        if (StageManager.Instance.currentStage.id == 666)
        {
            GameManager.Instance.isRealBossing.RuntimeValue = true;
        }

        GameManager.Instance.isBossing.RuntimeValue = true;
        AudioManager.Instance.StopMusic();
        ObjectManager.Instance.PopObject(boss.name, transform.Find("BossSpawnPoint"));
    }

    public void RoomClear()
    {
        GameManager.Instance.isBossing.RuntimeValue = false;

        if (StageManager.Instance.currentStage.id == 666)
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.Ending());
        }
        else
        {
            // AudioManager.Instance.StopMusic();
            int asd = GameManager.Instance.enemyRunTimeSet.Items.Count;
            for (int i = 0; i < asd; i++)
            {
                GameManager.Instance.enemyRunTimeSet.Items[0].SetActive(false);
            }

            ObjectManager.Instance.PopObject("BossChest", transform.Find("ChestPoint"));
            ObjectManager.Instance.PopObject("HealOrb30", transform.Find("ChestPoint").position + (Vector3.up * 5));
            foreach (GameObject hider in DoorHiders)
            {
                hider.SetActive(false);
            }

            foreach (GameObject particle in DoorParticles)
            {
                particle.SetActive(true);
            }

            StartCoroutine(UIManager.Instance.SpeedWagon_RoomClear());
        }
    }

    public void OpenPortal()
    {
        transform.Find("Portal").gameObject.SetActive(true);
    }
}