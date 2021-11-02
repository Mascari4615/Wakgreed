using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

[CreateAssetMenu]
public class TwitchConnect : ScriptableObject
{
    TcpClient Twitch;
    StreamReader Reader;
    StreamWriter Writer;

    const string URL = "irc.chat.twitch.tv";
    const int PORT = 6667;

    public string accountName = "woowakgood";
    public string Token = "woowakgood";

    public async void ConnectToTwitch()
    {
        Twitch = new TcpClient();

        await Twitch.ConnectAsync(URL, PORT);

        Reader = new StreamReader(Twitch.GetStream());
        Writer = new StreamWriter(Twitch.GetStream()) { NewLine = "\r\n", AutoFlush = true };

        await Writer.WriteLineAsync("PASS " + Token);
        await Writer.WriteLineAsync("NICK " + Token); // ���� �������

        ReadMessages();
    }

    public async void WriteToChannel(string channelName, string messageToSend)
    {
        await Writer.WriteLineAsync($"PRIVMSG #{channelName} :{messageToSend}");
    }
    public async void JoinChannel(string channelName)
    {
        await Writer.WriteLineAsync("JOIN #" + channelName);
    }
    public async void LeaveChannel(string channelName)
    {
        await Writer.WriteLineAsync("PART #" + channelName);
    }

    private string lastLine;
    public List<string> logs = new();
    [SerializeField] private int logsIndex;
    [SerializeField] private bool isClearLogs = false;

    private async void ReadMessages()
    {
        logs.Clear();
        logsIndex = 1;
        while (true)
        {
            if (isClearLogs)
            {
                logs.Clear();
                logsIndex = 1;
                isClearLogs = false;

                logs.Add(lastLine);
            }

            lastLine = await Reader.ReadLineAsync();
            logs.Add(lastLine);

            LogManager.Instance.Chat(messageDetails(lastLine));
            // Debug.Log(lastLine);

            /*
            if (lastLine != null && lastLine.StartsWith("PING"))
            {
                lastLine.Replace("PING", "PONG");
                await Writer.WriteLineAsync(lastLine);
            }
            */
        }
    }

    public bool NewTwitchMessage(out string newMessage)
    {
        if (logs.Count < logsIndex)
        {
            newMessage = "";
            return false;
        }

        for (int i = logsIndex; i <= logs.Count; i++)
        {
            if (logs[i - 1].Contains("PRIVMSG"))
            {
                logsIndex = i + 1;
                newMessage = logs[i - 1];
                return true;
            }
        }

        logsIndex = logs.Count + 1;
        newMessage = "";
        return false;
    }

    public string[] messageDetails(string twitchMessage)
    {
        if (twitchMessage == null)
        {
            return new string[1] { "" };
        }
        if (!twitchMessage.Contains("PRIVMSG"))
        {
            return new string[1] { "" };
        }

        string[] firstSplit = twitchMessage.Split(' ');
        string channelName = firstSplit[2].Substring(1);

        string[] messageSplit = twitchMessage.Split(':');
        string message = messageSplit[2];

        string first = firstSplit[0];
        string[] secondSplit = first.Split('!');
        string user = secondSplit[0].Substring(1);

        return new string[3] { channelName, user, message };
    }
}
