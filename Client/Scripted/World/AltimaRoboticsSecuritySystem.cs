using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class AltimaRoboticsSecuritySystem : MonoBehaviour
{
    public TextMeshProUGUI outputText;
    public TextMeshProUGUI infoText;
    //public TextMeshProUGUI timeText;
    string outputTextCache;

    public GameObject connectPrompt;
    float time;
    float cursorTime;

    public GuardNet guardNet;

    //public ScrollRect scrollRect;
    public float scrollAmountPerLine;

    public bool keyboardActive = true;
    string inputText;
    string cursorText;

    public PlaySound keyboardClickSfx;

    public CinemachineVirtualCamera vcam;
    private void Start()
    {
        outputTextCache = outputText.text;
        cursorText = "";
        inputText = "";
    }
    private void Update()
    {
        if (!guardNet.currentlyConnected && !guardNet.attemptingToConnect){
            time += Time.deltaTime;
            if (time >= 0.5f){
                connectPrompt.SetActive(!connectPrompt.activeSelf);
                time -= time;
            }
            if (Input.GetKeyDown(KeyCode.Return) && keyboardActive){
                guardNet.Connect();
            }
        }
        else { connectPrompt.SetActive(false); }

        cursorTime += Time.deltaTime;
        if (cursorTime >= 0.5f) { 
            cursorText = cursorText == "" ? "|" : "";
            cursorTime -= cursorTime;
        }

        outputText.rectTransform.anchoredPosition = new Vector2(0f, outputText.textInfo.lineCount > 17 ? (outputText.textInfo.lineCount-17) * scrollAmountPerLine : 0f);

        keyboardActive = Cache.activeVcamManager.currentVcam == vcam;

        infoText.text = $"{(Cache.canonTimeManager.gameTime.DayOfWeek.ToString()).Substring(0, 3)} {Cache.canonTimeManager.gameTime.ToString("MM/yy")} {Cache.canonTimeManager.GetTimeAsString()} Mem:0015989A00";
    }
    public void LateUpdate()
    {
        if (keyboardActive)
        {
            if (Input.anyKeyDown)
            {
                foreach (char c in Input.inputString)
                {
                    if (char.IsLetter(c))
                    {
                        string letterPressed = c.ToString();
                        if (Input.GetKey(KeyCode.LeftShift)) { inputText += letterPressed.ToUpper(); }
                        else { inputText += letterPressed; }
                        keyboardClickSfx.Play();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Backspace)) { inputText = inputText.Substring(0, inputText.Length - 1); keyboardClickSfx.Play(true, 0.75f); }
            if (Input.GetKeyDown(KeyCode.Space)){ inputText += " "; keyboardClickSfx.Play(true, 1f); }
            if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift)) { inputText += "!"; }
            if (Input.GetKeyDown(KeyCode.Slash) && Input.GetKey(KeyCode.LeftShift)) { inputText += "?"; }
            if (Input.GetKeyDown(KeyCode.Return) && inputText != "") { 
                if (inputText == "cls") { Cls(); }
                else { guardNet.SendMessageToServer(inputText); inputText = ""; }
                keyboardClickSfx.Play(true, 1.15f);
            }
        }
        outputText.text = outputTextCache + inputText + cursorText;
        outputTextCache = outputText.text.Substring(0, outputText.text.Length - (inputText.Length + cursorText.Length)); // This runs loads of errors first few frames FIX
    }
    public void Output(string text)
    {
        UpdateText(text);
        if (!guardNet.currentlyConnected && !guardNet.attemptingToConnect && outputText.textInfo.lineCount > 12){
            Cls();
            UpdateText(text);
        }
    }
    void UpdateText(string text)
    {
        outputText.text = outputText.text.Remove(outputText.text.Length - 2);
        outputText.text += "\n" + text + "\n>";
        outputTextCache = outputText.text;
    }
    public void Cls()
    {
        outputText.text = "\n>";
        inputText = "";
        outputTextCache = outputText.text;
    }
}
