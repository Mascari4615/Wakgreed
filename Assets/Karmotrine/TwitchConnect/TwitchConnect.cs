using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class TwitchConnect : ScriptableObject
{
    private const string URL = "irc.chat.twitch.tv";
    private const int Port = 6667;

    [SerializeField] private string channelName;
    [SerializeField] private string userName;

    [FormerlySerializedAs("Token")] [SerializeField]
    private string token;

    private StreamReader reader;
    private TcpClient twitch;
    private StreamWriter writer;

    public async void ConnectToTwitch()
    {
        twitch = new TcpClient();
        await twitch.ConnectAsync(URL, Port);

        try
        {
            reader = new StreamReader(twitch.GetStream());
            writer = new StreamWriter(twitch.GetStream()) { NewLine = "\r\n", AutoFlush = true };
        }
        catch (IOException ioException)
        {
            Debug.Log("! :" + ioException);
        }

        await writer.WriteLineAsync("PASS " + token);
        await writer.WriteLineAsync("NICK " + userName.ToLower());
        await writer.WriteLineAsync("JOIN #" + channelName.ToLower());

        ReadMessages();
    }

    private async void ReadMessages()
    {
        while (true)
        {
            string lastLine = null;

            try
            {
                lastLine = await reader.ReadLineAsync();
            }
            catch (IOException ioException)
            {
                Debug.Log("! :" + ioException);
                Temp();
                return;
            }

            if (lastLine != null && lastLine.Contains("PRIVMSG"))
            {
                string user = lastLine.Split(' ')[0].Split('!')[0][1..];
                string message = lastLine.Split(':')[2];
                StreamingManager.Instance.Chat(message, user);
            }
        }
    }

    public async void Temp()
    {
        try
        {
            await writer.WriteLineAsync("PART #" + channelName);
        }
        catch (IOException ioException)
        {
            Debug.Log("! :" + ioException);
        }

        ConnectToTwitch();
    }

    public async void WriteToChannel(string messageToSend)
    {
        await writer.WriteLineAsync($"PRIVMSG #{channelName} :{messageToSend}");
    }

    public async void LeaveChannel()
    {
        await writer.WriteLineAsync("PART #" + channelName);
    }
}