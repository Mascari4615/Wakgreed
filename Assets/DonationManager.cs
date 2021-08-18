using System.Collections;
using TMPro;
using UnityEngine;

public class DonationManager : MonoBehaviour
{
    private bool isDoing = false;
    [SerializeField] private GameObject donation;
    [SerializeField] private IntVariable Nyang;
    [SerializeField] private GameEvent nyangChange;

    private IEnumerator Donation()
    {
        isDoing = true;
        float t = 0;
        while (GameManager.Instance.isGaming)
        {
            if (t >= 5)
            {
                if (Random.Range(0, 100) < 30)
                {
                    Debug.Log("Donation!");
                    int donationAmount = Random.Range(1, 1001);
                    Nyang.RuntimeValue += donationAmount;
                    donation.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Ttmdacl´Ô ²¾¼­ {donationAmount}°ñµå Á¶°ø";
                    donation.SetActive(true);
                    nyangChange.Raise();
                    StartCoroutine(TurnOffDonation());
                }
                Debug.Log("Chance");
                t = 0;
            }
            t += 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private IEnumerator TurnOffDonation()
    {
        yield return new WaitForSeconds(6f);
        donation.GetComponent<Animator>().SetTrigger("OFF");
        yield return new WaitForSeconds(1f);
        donation.SetActive(false);
    }

    private void Update()
    {
        if (isDoing) return;
        if (GameManager.Instance.isGaming && !isDoing) { Debug.Log("Start"); StartCoroutine(Donation()); }
    }
}
