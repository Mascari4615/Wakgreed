using System.Collections;
using UnityEngine;
using TMPro;

public class Connect : MonoBehaviour
{
    [SerializeField] private TwitchConnect twitchConnect;
    [SerializeField] private TMP_InputField channelNameBox;
    [SerializeField] private string channelName;
    [SerializeField] private TMP_InputField messageBox;
    [SerializeField] private string message;
    [SerializeField] private TextMeshProUGUI lastMessage;

    private void Awake()
    {
        //Application.targetFrameRate = 60;
        //Application.runInBackground = true;

        // StartCoroutine(ConnectWoowakgoodChannel());   
    }

    private IEnumerator ConnectWoowakgoodChannel()
    {
        yield return new WaitForSeconds(1f);
        twitchConnect.ConnectToTwitch();
        yield return new WaitForSeconds(1f);
        //twitchConnect.JoinChannel("woowakgood");
        twitchConnect.JoinChannel("ttmdacl");
    }

    private void Update()
    {
        /*
        if (twitchConnect.NewTwitchMessage(out string newMassage))
        {
            lastMessage.text = MessageFormate(newMassage);
        }
        */
        //lastMessage.text = MessageFormate(twitchConnect.lastLine);
    }

    [ContextMenu("ConnectToTwich")]
    public void ConnectToTwich()
    {
        twitchConnect.ConnectToTwitch();
    }
    [ContextMenu("JoinChannel")]
    public void JoinChannel()
    {
        //twitchConnect.JoinChannel(channelNameBox.text);
        twitchConnect.JoinChannel(channelName);
    }
    [ContextMenu("LeaveChannel")]
    public void LeaveChannel()
    {
        //twitchConnect.LeaveChannel(channelNameBox.text);
        twitchConnect.LeaveChannel(channelName);
    }
    [ContextMenu("SendMassage")]
    public void SendMassage()
    {
        //twitchConnect.WriteToChannel(channelNameBox.text, messageBox.text);
        //twitchConnect.WriteToChannel(channelName, message);
        twitchConnect.WriteToChannel(channelName, messageBox.text);
    }

    private string MessageFormate(string twitchLine)
    {
        string[] messageDetails = twitchConnect.messageDetails(twitchLine);
        if (messageDetails.Length == 1)
        {
            return "";
        }

        return "Channel: " + messageDetails[0] + "\n" +
                "User: " + messageDetails[1] + "\n" +
                "Message: " + messageDetails[2];
    }

    void OnApplicationQuit()
    {
        //twitchConnect.LeaveChannel(channelName);
        //twitchConnect.logs.Clear();
        //for (int i = 1; i < LogManager.Instance.chatPanel.transform.childCount; i++)
        //{
        //    Destroy(LogManager.Instance.chatPanel.transform.GetChild(i).gameObject);
            //Debug.Log(i);
       // }
        //twitchConnect.LeaveChannel("woowakgood");
      //  twitchConnect.LeaveChannel("ttmdacl");
       // Debug.Log("Application ending after " + Time.time + " seconds");
    }
}
