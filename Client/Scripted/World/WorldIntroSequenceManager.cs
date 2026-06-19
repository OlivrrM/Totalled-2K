using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Fragsurf.Movement;
using UnityEngine.UI;
using System.Linq.Expressions;
using TMPro;

public class WorldIntroSequenceManager : MonoBehaviour
{
    public bool sequenceActive;
    public CinemachineVirtualCamera sequenceCam;
    Transform introSequencePlayer;

    Vector2 camDir;

    public float yCamCenter;
    public Vector2 xCamAmounts;
    public Vector2 yCamAmounts;

    public Vector2 cursorDirection;

    CanvasGroup crosshairCanvasGroup;

    public float smoothSpeed = 5f;

    Animator introFadeInScreen;
    CanvasGroup introFadeInScreenCanvasGroup;

    public AltimaRoboticsSecuritySystem altimaSecuritySystem;

    [Header("Events")]
    public Robot runningLeia;
    public Transform runningLeiaPos;
    public CctvInspectableScreen leiaCamScreen;

    public Transform camThrownObjects;
    Rigidbody[] thrownObjectRbs;
    public CctvInspectableScreen throwObjectsCam;
    public Animator cam4Animator;

    public Robot[] robotsInvolved;
    RobotHealth[] robotsInvolvedHealth;
    public CctvInspectableScreen[] robotsInvolvedCameras;
    public Robot[] leisInvolvedToRunAway;
    public Transform leisInvolvedToRunAwayPos;
    public TurretHealth[] turretsToKill;

    public GameObject[] militaryCars;
    public CctvInspectableScreen[] militaryCarCameras;

    public AudioSource distantWarSfx;
    public AudioSource distantShotsSfx;

    public Transform CEO;
    public CctvInspectableScreen CeoScreen;
    public Animator ceoCamAnimator;

    public CctvInspectableScreen[] cctvCamerasToTurnOff;

    public Animator leiaJumpScareAnimator;
    public Transform leiaJumpscareTargetCam;
    public CctvCameraRenderManager cctvCamRenderManager;
    bool forceFocusTarget;
    public Material screenOffMaterial;
    public PlaySound screensOffSfx;
    public SpriteRenderer[] screenBloomFx;

    ScreenViewerManager screenViewerManager;

    public AudioSource scrapyardBossSpeechSfx;

    GameObject flashlight;

    public GameObject exitSeatPromptInterface;
    TextMeshProUGUI exitSeatPromptText;
    bool scriptedSequencesComplete;

    //public PingPongOutline[] pingPongOutlines;
    public Material itemHighlightMaterial;

    public GameObject[] camPositions;

    [SerializeField] public List<SubtitleSpeech> bossScrapyardSpeechSubtitleQueue = new List<SubtitleSpeech>();
    [SerializeField] public List<SubtitleGraphicEffect> bossScrapyardEffectsSubtitleQueue = new List<SubtitleGraphicEffect>();

    Coroutine currentLeiaSequence;
    Coroutine currentCeoSequence;
    Coroutine currentEventsSequence;

    // Clear robot gibs off floor after player gets up
    // Change color of item highlights
    // Add subtitles
    // Deactivate camera positions
    // Add new content to LOD

    public GameObject skipIntroSequenceUi;
    CanvasGroup skipIntroSequenceCanvasGroup;
    Slider skipIntroSeqeunceSlider;
    float skipIntroSequenceTime;

    public AudioSource y2kRadioSpeech;

    public TextMeshProUGUI timeIntroFadeText;
    private void Start()
    {
        screenViewerManager = GameObject.Find("ScreenViewer").GetComponent<ScreenViewerManager>();
        flashlight = GameObject.Find("Flashlight");
        introFadeInScreen = GameObject.Find("IntroFadeInScreen").GetComponent<Animator>();
        introFadeInScreen.SetBool("StartFadeIn", true);
        introFadeInScreenCanvasGroup = introFadeInScreen.GetComponent<CanvasGroup>();
        for (int i = 0; i < militaryCars.Length; i++){
            militaryCars[i].SetActive(false);
        }
        robotsInvolvedHealth = new RobotHealth[robotsInvolved.Length];
        for (int i = 0; i < robotsInvolved.Length; i++){
            robotsInvolvedHealth[i] = robotsInvolved[i].GetComponent<RobotHealth>();
        }
        leiaJumpScareAnimator.transform.parent.parent.gameObject.SetActive(false); // This whole script is extremely hard coded. But who cares its a scripted sequence
        GameObject dynamicCanvas = Cache.dynamicCanvas.gameObject;
        Instantiate(exitSeatPromptInterface, dynamicCanvas.transform.position, Quaternion.identity,dynamicCanvas.transform);
        exitSeatPromptText = GameObject.Find("ExitSeatPromptText").GetComponent<TextMeshProUGUI>();
        exitSeatPromptText.color = Utilities.Invisible(exitSeatPromptText.color);
        exitSeatPromptText.text = $"[{InputManager.inputBinds.savedBinds.binds["Jump"][0].keycode.ToString()}] to exit seat";
        /*for (int i = 0; i < pingPongOutlines.Length; i++){
            pingPongOutlines[i].colorA = Utilities.Invisible(pingPongOutlines[i].colorA);
            pingPongOutlines[i].colorB = Utilities.Invisible(pingPongOutlines[i].colorB);
        }*/
        itemHighlightMaterial.SetFloat("_Scale", 0f);
        timeIntroFadeText = GameObject.Find("FadeIntroTimeText").GetComponent<TextMeshProUGUI>();
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        Cache.walkSpeedManager.AddValue("BeginningSequenceSpeed", 1f);
        Cache.jumpHeightManagerScript.AddValue("BeginningSequenceJump", 1f);
        //Cache.mouseSensitivityManager.AddValue("BeginningSequenceMouse", Vector2.one);
        introSequencePlayer = sequenceCam.transform.parent;
        CursorManager.AddActivator("WorldIntroSequence", false);
        crosshairCanvasGroup = Cache.hitMarkerManager.GetComponent<CanvasGroup>();
        crosshairCanvasGroup.alpha = 0f;
        thrownObjectRbs = new Rigidbody[camThrownObjects.childCount];
        for (int i = 0; i < camThrownObjects.childCount; i++){
            thrownObjectRbs[i] = camThrownObjects.GetChild(i).GetComponent<Rigidbody>();
        }

        Begin();
    }
    public void Begin()
    {
        sequenceActive = true;
        Cache.walkSpeedManager.UpdateValue("BeginningSequenceSpeed", 0f);
        Cache.jumpHeightManagerScript.UpdateValue("BeginningSequenceJump", 0f);
        Cache.moveData.canJump = false;
        Cache.inspect.enabled = false;
        //Cache.inspect.castFrom = sequenceCam.transform;
        //Cache.mouseSensitivityManager.UpdateValue("BeginningSequenceMouse", Vector2.zero);
        sequenceCam.Priority = 98;
        CursorManager.UpdateActivator("WorldIntroSequence", true);
        Cache.hideUi.hidden = true;
        flashlight.SetActive(false);
        Cache.ambienceTrackManager.SetTrackVolume("Indoors0", 0.05f);
        currentCeoSequence = StartCoroutine(CeoSequence());
        currentEventsSequence = StartCoroutine(SequenceEvents());
        currentLeiaSequence = StartCoroutine(LeiaSequence());

        Cache.escape.lockEscapeMenu = true;

        skipIntroSequenceCanvasGroup = Instantiate(skipIntroSequenceUi, Cache.dynamicCanvas.transform.position, Quaternion.identity, Cache.dynamicCanvas.transform).GetComponent<CanvasGroup>();
        skipIntroSequenceCanvasGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(93f, 53.7f);
        skipIntroSeqeunceSlider = skipIntroSequenceCanvasGroup.transform.Find("SkipSlider").GetComponent<Slider>();
    }
    public void SkipIntroSequence()
    {
        Exit();
        StartCoroutine(EndSequence());
    }
    public void Exit()
    {
        sequenceActive = false;
        Cache.inspect.enabled = true;
        sequenceCam.Priority = -1;
        //Cache.inspect.castFrom = Camera.main.transform;
        Cache.walkSpeedManager.UpdateValue("BeginningSequenceSpeed", 1f);
        Cache.jumpHeightManagerScript.UpdateValue("BeginningSequenceJump", 1f);
        Cache.moveData.canJump = true;
        CursorManager.UpdateActivator("WorldIntroSequence", false);
        Cache.hideUi.hidden = false;
        flashlight.SetActive(true);
        Cache.moveData.ViewAngles = new Vector3(28.702652f, 56.823925f, 0.13447918f);
        for (int i = 0; i < robotsInvolved.Length; i++)
        {
            if (robotsInvolved[i] != null){
                Destroy(robotsInvolved[i].gameObject);
            }
        }
        for (int i = 0; i < leisInvolvedToRunAway.Length; i++){
            if (leisInvolvedToRunAway[i] != null){
                Destroy(leisInvolvedToRunAway[i].gameObject);
            }
        }
        for (int i = 0; i < turretsToKill.Length; i++){
            Destroy(turretsToKill[i].gameObject);
        }
        for (int i = 0; i < camPositions.Length; i++){
            camPositions[i].SetActive(false);
        }
        Destroy(runningLeia.gameObject);
        Destroy(camThrownObjects.gameObject);
        Cache.ambienceTrackManager.SetTrackVolume("Indoors0", 0.4f);
        GibManager.Clear();
        StopCoroutine(currentEventsSequence);
        StopCoroutine(currentCeoSequence);
        StopCoroutine(currentLeiaSequence);

        Cache.escape.lockEscapeMenu = false;
        y2kRadioSpeech.Stop();
        Destroy(skipIntroSequenceCanvasGroup.gameObject);
    }
    IEnumerator LeiaSequence()
    {
        yield return new WaitForSeconds(121.1f);
        cctvCamRenderManager.disableCamMovement = true;
        leiaJumpScareAnimator.transform.parent.parent.gameObject.SetActive(true);
        leiaJumpScareAnimator.SetBool("Lunging", true);
        leiaJumpScareAnimator.SetBool("Jumpscaring", false);
        leiaJumpScareAnimator.SetBool("HitWall", false);
        leiaJumpScareAnimator.SetBool("Death", false);
        forceFocusTarget = true;
        yield return new WaitForSeconds(.66f);
        StartCoroutine(EndSequence());
    }
    IEnumerator EndSequence()
    {
        screensOffSfx.Play();
        for (int i = 0; i < cctvCamerasToTurnOff.Length; i++)
        {
            cctvCamerasToTurnOff[i].screenRenderer.material = screenOffMaterial;
            cctvCamerasToTurnOff[i].targetForceStaticVolume = 0f;
            cctvCamerasToTurnOff[i].hummingSfx.Stop();
        }
        CeoScreen.screenRenderer.material = screenOffMaterial;
        CeoScreen.targetForceStaticVolume = 0f;
        CeoScreen.hummingSfx.Stop();
        Destroy(cctvCamRenderManager.cctvCamera);
        cctvCamRenderManager.enabled = false;
        for (int i = 0; i < screenBloomFx.Length; i++)
        {
            screenBloomFx[i].enabled = false;
        }
        //screenViewerManager.active = false; Do this when player gets up
        scrapyardBossSpeechSfx.Play();
        SubtitleQueueInfo subtitleQueueInfo = new SubtitleQueueInfo { queueEndFadeOutSpeed = 1f };
        Cache.subtitleManager.QueueNewSubtitles(bossScrapyardSpeechSubtitleQueue, bossScrapyardEffectsSubtitleQueue, subtitleQueueInfo);
        for (int i = 0; i < cctvCamerasToTurnOff.Length; i++)
        {
            cctvCamerasToTurnOff[i].staticFadeInSfx.SFX.volume = 0f;
            cctvCamerasToTurnOff[i].staticFadeOutSfx.SFX.volume = 0f;
            cctvCamerasToTurnOff[i].GetComponent<Collider>().enabled = false;
        }
        CeoScreen.staticFadeInSfx.SFX.volume = 0f;
        CeoScreen.staticFadeOutSfx.SFX.volume = 0f;
        CeoScreen.GetComponent<Collider>().enabled = false;
        GibManager.Clear();
        yield return new WaitForSeconds(5f);
        scriptedSequencesComplete = true;
    }
    IEnumerator CeoSequence() // 45, 20, 10, 5
    {
        yield return new WaitForSeconds(73);
        CeoScreen.forceStatic = true;
        yield return new WaitForSeconds(2);
        CeoScreen.forceStatic = false;
        CEO.transform.position = new Vector3(959.200012f, -1.17999995f, 113.769997f);
        CEO.eulerAngles = new Vector3(270f, 149.999725f, 0f);
        yield return new WaitForSeconds(23);
        CeoScreen.forceStatic = true;
        yield return new WaitForSeconds(2);
        CeoScreen.forceStatic = false;
        CEO.transform.position = new Vector3(956.789978f, -1.24300003f, 113.080002f);
        CEO.eulerAngles = new Vector3(270f, 111.095421f, 0f);
        yield return new WaitForSeconds(7);
        ceoCamAnimator.SetBool("Stop", true);
        CeoScreen.forceStatic = true;
        yield return new WaitForSeconds(3);
        CeoScreen.forceStatic = false;
        CEO.transform.position = new Vector3(954.530029f, -1.24300003f, 113.879997f);
        CEO.eulerAngles = new Vector3(270f, 151.775375f, 0f);
        yield return new WaitForSeconds(3.5f);
        CeoScreen.forceStatic = true;
        yield return new WaitForSeconds(1.5f);
        CeoScreen.forceStatic = false;
        CEO.transform.position = new Vector3(953.200012f, -0.100000001f, 116.639999f);
        CEO.eulerAngles = new Vector3(272.852661f, 151.775314f, 8.57749765e-05f);
    }
    IEnumerator SequenceEvents()
    {
        yield return new WaitForSeconds(27f);
        leiaCamScreen.forceStatic = true;
        cctvCamRenderManager.SetCamWarning(leiaCamScreen.renderIndex, leiaCamScreen.camName);
        yield return new WaitForSeconds(0.2f);
        altimaSecuritySystem.Output("<color=red>!Error! Camera 6G experiencing connection issues</color>\n");
        yield return new WaitForSeconds(2.8f);
        leiaCamScreen.forceStatic = false;
        cctvCamRenderManager.ClearWarning();
        runningLeia.SetDestination(runningLeiaPos.position);
        yield return new WaitForSeconds(12);
        throwObjectsCam.forceStatic = true;
        cctvCamRenderManager.SetCamWarning(throwObjectsCam.renderIndex, throwObjectsCam.camName);
        //altimaSecuritySystem.Output("<color=red>!Error! Camera 2B experiencing connection issues</color>\n");
        yield return new WaitForSeconds(3);
        cam4Animator.SetBool("Trigger", true);
        throwObjectsCam.forceStatic = false;
        cctvCamRenderManager.ClearWarning();
        for (int i = 0; i < thrownObjectRbs.Length; i++){
            thrownObjectRbs[i].isKinematic = false;
            thrownObjectRbs[i].AddForce(new Vector3(Random.RandomRange(600, 1000), 0, Random.RandomRange(100, 250)));
        }
        yield return new WaitForSeconds(13);
        for (int i = 0; i < robotsInvolvedCameras.Length; i++){
            robotsInvolvedCameras[i].forceStatic = true;
        }
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < robotsInvolvedCameras.Length; i++){
            robotsInvolvedCameras[i].forceStatic = false;
        }
        for (int i = 0; i < robotsInvolved.Length; i++){
            robotsInvolved[i].Stun(1, 60, 1);
            robotsInvolved[i].maxIdleDistanceFromSpawn = 0f;
            robotsInvolved[i].idleRadius = 0f;
            robotsInvolved[i].SetDestination(robotsInvolved[i].transform.position);
        }
        for (int i = 0; i < leisInvolvedToRunAway.Length; i++)
        {
            leisInvolvedToRunAway[i].idleSpeed = 16;
            leisInvolvedToRunAway[i].chasingSpeed = 16;
            leisInvolvedToRunAway[i].idleRadius = 0f;
            leisInvolvedToRunAway[i].maxIdleDistanceFromSpawn = 0f;
            leisInvolvedToRunAway[i].robotSpeedManager.AddValue("JUSTGOGO", 16f);
            leisInvolvedToRunAway[i].bodyAnimator.idleMoveSpeedMultiplier = 0.22f;
            leisInvolvedToRunAway[i].SetDestination(leisInvolvedToRunAwayPos.position);
        }
        yield return new WaitForSeconds(5);
        distantShotsSfx.Play();
        yield return new WaitForSeconds(4);
        militaryCarCameras[0].forceStatic = true;
        cctvCamRenderManager.SetCamWarning(militaryCarCameras[0].renderIndex, militaryCarCameras[0].camName);
        distantWarSfx.Play();
        yield return new WaitForSeconds(1);
        militaryCars[0].SetActive(true);
        militaryCarCameras[0].forceStatic = false;
        cctvCamRenderManager.ClearWarning();
        yield return new WaitForSeconds(1.5f);
        militaryCarCameras[1].forceStatic = true;
        cctvCamRenderManager.SetCamWarning(militaryCarCameras[1].renderIndex, militaryCarCameras[1].camName);
        yield return new WaitForSeconds(1f);
        militaryCars[1].SetActive(true);
        militaryCarCameras[1].forceStatic = false;
        cctvCamRenderManager.ClearWarning();
        yield return new WaitForSeconds(1.5f);
        militaryCarCameras[2].forceStatic = true;
        cctvCamRenderManager.SetCamWarning(militaryCarCameras[2].renderIndex, militaryCarCameras[2].camName);
        yield return new WaitForSeconds(1f);
        militaryCars[2].SetActive(true);
        militaryCarCameras[2].forceStatic = false;
        cctvCamRenderManager.ClearWarning();
        yield return new WaitForSeconds(0.5f);
        Damage shot = new Damage { amount = 9999, type = Totalled.DamageType.Unknown, attacker = gameObject };
        robotsInvolvedHealth[0].Damage(shot);
        yield return new WaitForSeconds(0.25f);
        robotsInvolvedHealth[1].Damage(shot);
        yield return new WaitForSeconds(0.25f);
        robotsInvolvedHealth[2].Damage(shot);
        yield return new WaitForSeconds(0.4f);
        robotsInvolvedHealth[3].Damage(shot);
        yield return new WaitForSeconds(0.1f);
        robotsInvolvedHealth[4].Damage(shot);
        for (int i = 0; i < turretsToKill.Length; i++){
            turretsToKill[i].Damage(shot,true);
        }
        yield return new WaitForSeconds(0.5f);
        robotsInvolvedHealth[5].Damage(shot);
        yield return new WaitForSeconds(3f);
        cctvCamerasToTurnOff[0].targetForceStaticVolume = 0.25f;
        cctvCamerasToTurnOff[0].forceStatic = true;
        yield return new WaitForSeconds(2f);
        cctvCamerasToTurnOff[1].targetForceStaticVolume = 0.25f;
        cctvCamerasToTurnOff[1].forceStatic = true;
        yield return new WaitForSeconds(1.5f);
        cctvCamerasToTurnOff[2].targetForceStaticVolume = 0.25f;
        cctvCamerasToTurnOff[2].forceStatic = true;
        cctvCamRenderManager.SetCamWarning(CeoScreen.renderIndex, CeoScreen.camName);
        yield return new WaitForSeconds(3f);
        cctvCamerasToTurnOff[3].targetForceStaticVolume = 0.25f;
        cctvCamerasToTurnOff[3].forceStatic = true;
        yield return new WaitForSeconds(4f);
        cctvCamerasToTurnOff[4].targetForceStaticVolume = 0.25f;
        cctvCamerasToTurnOff[4].forceStatic = true;
        yield return new WaitForSeconds(1f);
        cctvCamerasToTurnOff[5].targetForceStaticVolume = 0.25f;
        cctvCamerasToTurnOff[5].forceStatic = true;
        altimaSecuritySystem.enabled = false;
    }
    private void Update()
    {
        if (sequenceActive)
        {
            cursorDirection = Utilities.GetCursorDirection();

            if (!Terminal.terminalActive && !Escape.active){
                Quaternion targetIntroRotation = Quaternion.Euler(0f,yCamCenter + (cursorDirection.x > 0f ? yCamAmounts.y * cursorDirection.x : yCamAmounts.x * Mathf.Abs(cursorDirection.x)), 0f);
                Quaternion targetSequenceCamRotation = Quaternion.Euler((cursorDirection.y > 0f ? xCamAmounts.y * cursorDirection.y : xCamAmounts.x * Mathf.Abs(cursorDirection.y)), 0f,0f);
                introSequencePlayer.localRotation = Quaternion.Slerp(introSequencePlayer.localRotation,targetIntroRotation,Time.deltaTime * smoothSpeed);
                sequenceCam.transform.localRotation = Quaternion.Slerp(sequenceCam.transform.localRotation,targetSequenceCamRotation,Time.deltaTime * smoothSpeed);
            }
            if (forceFocusTarget)
            {
                cctvCamRenderManager.transform.position = Vector3.Lerp(cctvCamRenderManager.transform.position, leiaJumpscareTargetCam.position, Time.deltaTime * 20f);
                cctvCamRenderManager.transform.rotation = Quaternion.Lerp(cctvCamRenderManager.transform.rotation, leiaJumpscareTargetCam.rotation, Time.deltaTime * 20f);
                cctvCamRenderManager.FocusTarget(1);
            }
            //if (InputManager.GetEscapeKeyDown()) { Exit(); } // Temp

            if (scriptedSequencesComplete){
                exitSeatPromptText.color = Color.Lerp(exitSeatPromptText.color, new Color(0.8f, 0.8f, 0.8f, 0.8f), Time.deltaTime);
                if (InputManager.GetJumpKey())
                {
                    Exit();
                }
            }

            timeIntroFadeText.text = $"Time:\n <u>{Cache.canonTimeManager.GetTimeAsString()}</u>\n31 December 1999";

            skipIntroSequenceCanvasGroup.alpha = Mathf.Lerp(skipIntroSequenceCanvasGroup.alpha, (StartTime.instanceTime < 5f || skipIntroSequenceTime > 0f) ? 1f : .33f, Time.deltaTime * 4f);
            if (InputManager.GetEscapeKey())
            {
                skipIntroSequenceTime += Time.deltaTime;
                if (skipIntroSequenceTime >= 3f)
                {
                    SkipIntroSequence();
                }
            }
            else
            {
                skipIntroSequenceTime -= Time.deltaTime * 2f;
                if (skipIntroSequenceTime <= 0f) { skipIntroSequenceTime = 0f; }
            }
            skipIntroSeqeunceSlider.value = skipIntroSequenceTime;
        }
        else
        {
            exitSeatPromptText.color = Color.Lerp(exitSeatPromptText.color, Utilities.Invisible(exitSeatPromptText.color), Time.deltaTime * 5f);
            /*for (int i = 0; i < pingPongOutlines.Length; i++){
                if (pingPongOutlines[i] != null){
                    pingPongOutlines[i].colorA = Color.Lerp(pingPongOutlines[i].colorA, Utilities.Visible(pingPongOutlines[i].colorA),Time.deltaTime*3f);
                    pingPongOutlines[i].colorB = Color.Lerp(pingPongOutlines[i].colorB, Utilities.Visible(pingPongOutlines[i].colorB), Time.deltaTime * 3f);
                }
            }*/
            //itemHighlightMaterial.SetColor("_Color", Color.Lerp(itemHighlightMaterial.GetColor("_Color"), Utilities.Visible(itemHighlightMaterial.GetColor("_Color")), Time.deltaTime * 2f));
            itemHighlightMaterial.SetFloat("_Scale", Mathf.Lerp(itemHighlightMaterial.GetFloat("_Scale"), 0.005f, Time.deltaTime));
            introFadeInScreenCanvasGroup.alpha = Mathf.Lerp(introFadeInScreenCanvasGroup.alpha, 0f, Time.deltaTime * 2f);
        }

        crosshairCanvasGroup.alpha = Mathf.Lerp(crosshairCanvasGroup.alpha, Utilities.BoolToInt(!sequenceActive), Time.deltaTime);
        /*
        if (Input.GetKeyDown(KeyCode.F7))
        {
            scrapyardBossSpeechSfx.Play();
            SubtitleQueueInfo subtitleQueueInfo = new SubtitleQueueInfo { queueEndFadeOutSpeed = 1f };
            Cache.subtitleManager.QueueNewSubtitles(bossScrapyardSpeechSubtitleQueue, bossScrapyardEffectsSubtitleQueue, subtitleQueueInfo);
        }*/
    }
}
