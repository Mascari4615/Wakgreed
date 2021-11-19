using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class TwitchConnect : ScriptableObject
{
    private TcpClient twitch;
    private StreamReader reader;
    private StreamWriter writer;

    private const string URL = "irc.chat.twitch.tv";
    private const int Port = 6667;

    [SerializeField] private string channelName;
    [SerializeField] private string userName;
    [FormerlySerializedAs("Token")] [SerializeField] private string token;

    public async void ConnectToTwitch()
    {
        twitch = new TcpClient();
        await twitch.ConnectAsync(URL, Port);

        reader = new StreamReader(twitch.GetStream());
        writer = new StreamWriter(twitch.GetStream()) { NewLine = "\r\n", AutoFlush = true };

        await writer.WriteLineAsync("PASS " + token);
        await writer.WriteLineAsync("NICK " + userName.ToLower());
        await writer.WriteLineAsync("JOIN #" + channelName.ToLower());

        ReadMessages();
    }
    
    private async void ReadMessages()
    {
        while (true)
        {
            string lastLine = await reader.ReadLineAsync();
            // Debug.Log(lastLine);

            if ((lastLine != null) && (lastLine.Contains("PRIVMSG")))
            {
                string user = lastLine.Split(' ')[0].Split('!')[0][1..];
                string message = lastLine.Split(':')[2];
                StreamingManager.Instance.TwitchChat(user, message);
            }
        }
    }
    
    public async void WriteToChannel(string messageToSend) => await writer.WriteLineAsync($"PRIVMSG #{channelName} :{messageToSend}");
    public async void LeaveChannel() => await writer.WriteLineAsync("PART #" + channelName);
}
