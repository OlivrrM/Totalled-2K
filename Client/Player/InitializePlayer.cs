using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;
using Cinemachine;

public class InitializePlayer : MonoBehaviour
{
    [Header("--= References =--")]
    public GameObject Player;

    [Header("--= Movement =--")]
    public float speed = 2;
    public float gravity = 20.32f;
    public float jumpPower = 7.15f;

    [Header("--= Camera =--")]
    public float FOV = 90;

    public static GameObject forcePlayer;
    private void Awake()
    {
        GameObject[] spawnPoses = GameObject.FindGameObjectsWithTag("SpawnPos");
        if (spawnPoses.Length > 0){
            transform.position = spawnPoses[Random.RandomRange(0, spawnPoses.Length)].transform.position;
        }
        if (forcePlayer != null) { Player = forcePlayer; }
        GameObject initialisedPlayer = Instantiate(Player, transform.position, Quaternion.identity);
        initialisedPlayer.name = Player.name;
        try { Cache.surfCharacter = initialisedPlayer.GetComponent<SurfCharacter>(); }
        catch { Debug.LogError("Could not load player movement settings! Could not find 'SurfCharacter' component within player"); }
        if (Cache.surfCharacter != null){
            Cache.moveData = Cache.surfCharacter.MoveData;
            Cache.moveData.WalkFactor = speed;
            //Cache.surfCharacter.FieldOfView = (int) FOV;
            Cache.surfCharacter.MoveConfig.Gravity = gravity;
            Cache.surfCharacter.MoveConfig.JumpPower = jumpPower;

            InputManager.mouseSensX = Cache.surfCharacter.XSens;
            InputManager.mouseSensY = Cache.surfCharacter.YSens;

            CinemachineVirtualCamera vcam = GameObject.Find("Vcam").GetComponent<CinemachineVirtualCamera>();
            vcam.m_Lens.FieldOfView = (int) FOV;
            vcam.LookAt = Cache.surfCharacter.Camera.transform;
            vcam.Follow = Cache.surfCharacter.Camera.transform;
        }

        
        /*
        try { Cache.playerCamera = initialisedPlayer.transform.Find("Cam").GetComponent<Camera>(); }
        catch { Debug.LogError("Could not load player camera settings! 'Cam' could not be found within player"); }
        if (Cache.playerCamera != null){
            Cache.playerCamera.fieldOfView = FOV;
        }
        */

    }
}
