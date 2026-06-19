using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class GuardNet : MonoBehaviour
{
    public AltimaRoboticsSecuritySystem sys;
    public string localUrl = "http://localhost:8000/message.php";

    public bool currentlyConnected;
    [HideInInspector] public bool attemptingToConnect;

    bool outputConnectedMessage;

    string lastMessage;

    string uuid;

    public PlaySound dialUpSfx;
    public PlaySound messageReceivedSfx;
    public void Connect()
    {
        StartCoroutine(ConnectionSequence());
    }
    IEnumerator ConnectionSequence()
    {
        attemptingToConnect = true;
        dialUpSfx.Play();
        sys.Output("Connecting to GuardNet. . .\n");
        outputConnectedMessage = false;
        uuid = GenerateUUID();
        yield return new WaitForSeconds(5f);
        sys.Output("Status: Dialing. . .\n");
        yield return new WaitForSeconds(5f);
        sys.Output("Logging on to network. . .\n");
        yield return new WaitForSeconds(5f);
        InvokeRepeating(nameof(GetMessage), 0f, .5f);
    }

    public void Disconnect()
    {
        CancelInvoke(nameof(GetMessage));
        currentlyConnected = false;
        sys.Cls();
        sys.Output("Disconnected from GuardNet\n");
    }
    string GenerateUUID()
    {
        char randomLetter = (char)Random.Range('A', 'Z' + 1);
        char randomNumber = (char)Random.Range('0', '9' + 1);
        return randomLetter.ToString() + randomNumber.ToString();
    }

    public void SendMessageToServer(string message)
    {
        message = $"{uuid}: {message}@{Random.Range(10000, 99999)}";
        StartCoroutine(SendMessageRoutine(message));
    }

    IEnumerator SendMessageRoutine(string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("message", message);

        UnityWebRequest www = UnityWebRequest.Post(localUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError("Send failed: " + www.error);
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SendMessageToServer($"{uuid}: Hi!@{Random.Range(10000,99999)}");
        }
    }
    */

    void GetMessage()
    {
        StartCoroutine(GetMessageRoutine());
    }

    IEnumerator GetMessageRoutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(localUrl);
        yield return www.SendWebRequest();
        bool messageReceived = false;
        if (www.result == UnityWebRequest.Result.Success){
            if (www.downloadHandler.text != "") 
            {
                if (www.downloadHandler.text != lastMessage){
                    messageReceived = true;
                }
            }
        }
        else{
            Disconnect();
            sys.Output("<color=red>Error connecting to network: " + www.error + "</color>");
            attemptingToConnect = false;
            yield break;
        }
        if (!outputConnectedMessage){
            sys.Output("<color=#80ff99>Connected. Type a message to send to your security network</color>\n");
            outputConnectedMessage = true;
        }
        if (messageReceived){
            sys.Output(ReturnMessage(www.downloadHandler.text));
            lastMessage = www.downloadHandler.text;
        }
        attemptingToConnect = false;
        currentlyConnected = true;
    }
    string ReturnMessage(string input)
    {
        try
        {
            string[] parts = input.Split('@');
            if (parts[0].Substring(0, 2) == uuid) { parts[0] = parts[0].Replace(parts[0].Substring(0, 2), "You"); }
            else { messageReceivedSfx.Play(); }
            return parts[0];
        }
        catch
        {
            return "<color=yellow>Unknown message received";
        }
    }
}