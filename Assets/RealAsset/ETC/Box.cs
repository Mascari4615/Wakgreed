using UnityEngine;
using FMODUnity;
public class Box : MonoBehaviour, IHitable
{
    [SerializeField] private GameObject[] fragments;

    public void ReceiveHit(int damage)
    {
        RuntimeManager.PlayOneShot("event:/SFX/ETC/Box", Wakgood.Instance.attackPosition.position);
        ObjectManager.Instance.PopObject("HealObject", transform);
        foreach (var fragment in fragments)
        {
            fragment.SetActive(true);
            fragment.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(300f, 500f) * (1 + -2 * Random.Range(0, 2)), Random.Range(300f, 500f) * (1 + -2 * Random.Range(0, 2))));
        }
        gameObject.SetActive(false);
    }
}