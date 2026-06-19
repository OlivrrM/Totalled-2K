using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    List<SubtitleSpeech> currentSubtitleQueue;

    List<SubtitleGraphicEffect> defaultSubtitleGraphicEffects = new List<SubtitleGraphicEffect>();
    List<SubtitleGraphicEffect> currentSubtitleGraphicEffects;

    Vector2 defaultSubtitleTextPos;
    RectTransform subtitleTextRect;
    Vector2 currentShakeAmount;
    float currentShakeLifetime;

    float currentQueueEndFadeOutSpeed;

    bool subtitleQueueCurrentlyActive;

    float previousSubtitleTime;

    Coroutine currentRunningSubtitles;
    Coroutine currentRunningSubtitleGraphics;
    private void Start()
    {
        Cache.subtitleManager = this;
        subtitleTextRect = subtitleText.GetComponent<RectTransform>();
        defaultSubtitleTextPos = subtitleTextRect.anchoredPosition;
        defaultSubtitleGraphicEffects = new List<SubtitleGraphicEffect>();
        defaultSubtitleGraphicEffects.Add(new SubtitleGraphicEffect{
            time = 0f,
            color = Color.white,
        });
        currentQueueEndFadeOutSpeed = 5f;
    }
    public void QueueNewSubtitles(List<SubtitleSpeech> titles, List<SubtitleGraphicEffect> subtitleGraphicEffects,SubtitleQueueInfo subtitleQueueInfo)
    {
        //if (subtitleQueueInfo == null) { subtitleQueueInfo = new SubtitleQueueInfo { queueEndFadeOutSpeed = 99f }; }
        //if (subtitleGraphicEffects == null) { subtitleGraphicEffects = defaultSubtitleGraphicEffects; }
        currentSubtitleGraphicEffects = subtitleGraphicEffects;
        currentSubtitleQueue = titles;
        currentQueueEndFadeOutSpeed = subtitleQueueInfo.queueEndFadeOutSpeed;
        subtitleQueueCurrentlyActive = true;
        currentRunningSubtitles = StartCoroutine(RunSubtitles());
        currentRunningSubtitleGraphics = StartCoroutine(RunSubtitleGraphics());
    }
    IEnumerator RunSubtitles() // This probably breaks when playing a second subtitle queue in same instance
    {
        for (int i = 0; i < currentSubtitleQueue.Count; i++)
        {
            yield return new WaitForSeconds(currentSubtitleQueue[i].time-previousSubtitleTime);
            subtitleText.text = currentSubtitleQueue[i].text;
            previousSubtitleTime = currentSubtitleQueue[i].time;
        }
        SubtitlesEndCleanup();
    }
    void SubtitlesEndCleanup()
    {
        subtitleQueueCurrentlyActive = false;
        previousSubtitleTime = 0f;
        subtitleText.text = "";
    }
    IEnumerator RunSubtitleGraphics()
    {
        for (int i = 0; i < currentSubtitleGraphicEffects.Count; i++){
            yield return new WaitForSeconds(currentSubtitleGraphicEffects[i].time);
            if (currentSubtitleGraphicEffects[i].color!=new Color()){
                subtitleText.color = currentSubtitleGraphicEffects[i].color;
            }
            if (currentSubtitleGraphicEffects[i].shakeAmount != Vector2.zero) { currentShakeAmount = currentSubtitleGraphicEffects[i].shakeAmount; }
            if (currentSubtitleGraphicEffects[i].shakeLifetime != 0f) { currentShakeLifetime = currentSubtitleGraphicEffects[i].shakeLifetime; }
        }
    }
    public void CancelCurrentSubtitles()
    {
        if (currentRunningSubtitles != null){
            StopCoroutine(currentRunningSubtitles);
            SubtitlesEndCleanup();
        }
        if (currentRunningSubtitleGraphics != null){
            StopCoroutine(currentRunningSubtitleGraphics);
        }
    }
    private void Update()
    {
        subtitleTextRect.anchoredPosition = defaultSubtitleTextPos + new Vector2(Random.Range(-currentShakeAmount.x, currentShakeAmount.x), Random.Range(-currentShakeAmount.y, currentShakeAmount.y));
        currentShakeAmount = Vector2.Lerp(currentShakeAmount, Vector2.zero, Time.deltaTime * currentShakeLifetime);
        if (!subtitleQueueCurrentlyActive){
            subtitleText.color = Color.Lerp(subtitleText.color,Utilities.Invisible(subtitleText.color),Time.deltaTime*currentQueueEndFadeOutSpeed);
            //print(currentQueueEndFadeOutSpeed);
        }
        else { subtitleText.color = Utilities.Visible(subtitleText.color); }
    }
}
