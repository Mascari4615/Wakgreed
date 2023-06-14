using FMODUnity;
using UnityEngine;

public class Box : MonoBehaviour, IHitable
{
    [SerializeField] private GameObject[] fragments;

    public void ReceiveHit(int damage, HitType hitType = HitType.Normal)
    {
        RuntimeManager.PlayOneShot("event:/SFX/ETC/Box", Wakgood.Instance.AttackPosition.position);

        fragments[0].transform.parent.transform.position = transform.position;

        foreach (GameObject fragment in fragments)
        {
            fragment.SetActive(true);
            fragment.GetComponent<Rigidbody2D>().AddForce(new Vector2(
                Random.Range(300f, 500f) * (1 + (-2 * Random.Range(0, 2))),
                Random.Range(300f, 500f) * (1 + (-2 * Random.Range(0, 2)))));
        }

        gameObject.SetActive(false);

        if (Random.Range(0, 100) <= 20)
        {
            int randCount = Random.Range(1, 2 + 1);
            for (int i = 0; i < randCount; i++)
            {
                ObjectManager.Instance.PopObject("Goldu", transform);
            }
        }

        if (Random.Range(0, 100) <= 5)
        {
            ObjectManager.Instance.PopObject("HealOrb10", transform);
        }
    }
}