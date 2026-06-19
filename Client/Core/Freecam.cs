using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freecam : MonoBehaviour
{
    public Transform camBody;

    Vector2 input;
    public float speed;
    public float speedShiftMultiply;
    float currentSpeed;

    Vector2 mouseInput;
    public float mouseSensitivity;
    float cameraVerticalRotation;

    float upMovement;
    private void Start()
    {
        currentSpeed = speed;
        camBody.rotation = Cache.surfCharacter.transform.rotation;
        transform.rotation = Camera.main.transform.rotation;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) { currentSpeed = speed * speedShiftMultiply; }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) { currentSpeed = speed; }

        input.x = Input.GetAxis("Horizontal") * Utilities.BoolToInt(!InputManager.enabled && !Escape.active);
        input.y = Input.GetAxis("Vertical") * Utilities.BoolToInt(!Terminal.terminalActive && !Escape.active);
        upMovement = Utilities.BoolToInt(Input.GetKey(KeyCode.E)) + (-Utilities.BoolToInt(Input.GetKey(KeyCode.Q)));
        camBody.position += transform.right * input.x * currentSpeed * Time.deltaTime;
        camBody.position += transform.forward * input.y * currentSpeed * Time.deltaTime;
        camBody.position += transform.up * upMovement * currentSpeed * Time.deltaTime;

        mouseInput.x = Input.GetAxis("Mouse X") * Cache.surfCharacter.XSens * .02200f * Utilities.BoolToInt(!Terminal.terminalActive && !Escape.active);
        mouseInput.y = Input.GetAxis("Mouse Y") * Cache.surfCharacter.YSens * .02200f * Utilities.BoolToInt(!Terminal.terminalActive && !Escape.active);

        cameraVerticalRotation -= mouseInput.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90, 90);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        camBody.Rotate(Vector3.up * mouseInput.x);
        camBody.position = transform.position;
    }
}
