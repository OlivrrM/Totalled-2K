using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

public class InputManager
{
    public static float mouseSensX;
    public static float mouseSensY;

    public static bool enabled = true;

    public static InputBinds inputBinds = new InputBinds();

    public static float GetMouseMoveX(bool raw = false)
    {
        float moveX = 0;
        if (raw) { moveX = Input.GetAxisRaw("Mouse X") * mouseSensX; }
        else { moveX = Input.GetAxis("Mouse X") * mouseSensX; }
        return moveX * Utilities.BoolToInt(Active());
    }
    public static float GetMouseMoveY(bool raw = false)
    {
        float moveY = 0;
        if (raw) { moveY = Input.GetAxisRaw("Mouse Y")* mouseSensY; }
        else { moveY = Input.GetAxis("Mouse Y")* mouseSensY; }
        return moveY * Utilities.BoolToInt(Active());
    }
    /*
    public static bool GetJumpKey()
    {
        return Input.GetKey(KeyCode.Space) && Active();
    }
    public static bool GetJumpKeyDown()
    {
        return Input.GetKeyDown(KeyCode.Space) && Active();
    }
    */
    public static bool GetJumpKey()
    {
        //return inputBinds.IsBindActive("Jump") && Active();
        return inputBinds.IsBindActive("Jump") && Active();
    }
    public static bool GetVaultKeyDown()
    {
        return inputBinds.IsBindActive("Vault") && Active();
    }
    public static bool GetMoveLeftKey()
    {
        return inputBinds.IsBindActive("MoveLeft") && Active();
    }
    public static bool GetMoveRightKey()
    {
        return inputBinds.IsBindActive("MoveRight") && Active();
    }
    public static bool GetMoveForwardKey()
    {
        return inputBinds.IsBindActive("MoveForward") && Active();
    }
    public static bool GetMoveForwardKeyDown()
    {
        return inputBinds.IsBindActive("MoveForward",KeybindType.Down) && Active();
    }
    public static bool GetMoveForwardKeyUp()
    {
        return inputBinds.IsBindActive("MoveForward", KeybindType.Up) && Active();
    }
    public static bool GetMoveBackKey()
    {
        return inputBinds.IsBindActive("MoveBack") && Active();
    }
    public static bool GetYawLeftKey()
    {
        return inputBinds.IsBindActive("YawLeft") && Active();
    }
    public static bool GetYawRightKey()
    {
        return inputBinds.IsBindActive("YawRight") && Active();
    }
    public static bool GetCrouchKey()
    {
        return inputBinds.IsBindActive("Crouch") && Active();
    }
    public static bool GetStabilizeKey()
    {
        return inputBinds.IsBindActive("Stabilize") && Active();
    }
    public static bool GetCrouchKeyDown()
    {
        return inputBinds.IsBindActive("Crouch",KeybindType.Down) && Active();
    }
    public static bool GetCrouchKeyUp()
    {
        return inputBinds.IsBindActive("Crouch", KeybindType.Up) && Active();
    }
    public static bool GetSprintKey()
    {
        return inputBinds.IsBindActive("Sprint") && Active();
    }
    public static bool GetSprintKeyDown()
    {
        return inputBinds.IsBindActive("Sprint",KeybindType.Down) && Active();
    }
    public static bool GetUnequipKeyDown()
    {
        return inputBinds.IsBindActive("Unequip") && Active();
    }
    public static bool GetMainActionKeyDown()
    {
        return inputBinds.IsBindActive("MainAction",KeybindType.Down) && Active();
    }
    public static bool GetMainActionKey()
    {
        return inputBinds.IsBindActive("MainAction") && Active();
    }
    public static bool GetSecondaryActionKeyDown()
    {
        return inputBinds.IsBindActive("SecondaryAction",KeybindType.Down) && Active();
    }
    public static bool GetSecondaryActionKey()
    {
        return inputBinds.IsBindActive("SecondaryAction") && Active();
    }
    public static bool GetNoClipKeyDown()
    {
        return inputBinds.IsBindActive("NoClip") && Active();
    }
    public static bool GetRestartKeyDown()
    {
        return inputBinds.IsBindActive("ResetOrigin") && Active();
    }
    public static bool GetInteractKeyDown()
    {
        return inputBinds.IsBindActive("Interact") && Active();
    }
    public static bool GetInteractKey()
    {
        return inputBinds.IsBindActive("Interact",KeybindType.Key) && Active();
    }
    public static bool GetGrenadeKeyDown()
    {
        return inputBinds.IsBindActive("Grenade") && Active();
    }
    public static bool GetFocusKey()
    {
        return inputBinds.IsBindActive("Focus") && Active();
    }
    public static bool GetReloadKeyDown()
    {
        return inputBinds.IsBindActive("Reload") && Active();
    }
    public static bool GetToggleInterfaceKeyDown()
    {
        return inputBinds.IsBindActive("ToggleInterface") && ActiveExcludingEscape();
    }
    public static bool GetFlashlightKeyDown()
    {
        return inputBinds.IsBindActive("Flashlight") && Active();
    }
    public static bool GetThrowItemKeyDown()
    {
        return inputBinds.IsBindActive("Throw") && Active();
    }
    public static bool GetScreenshotKeyDown()
    {
        return inputBinds.IsBindActive("Screenshot") && Active();
    }
    public static bool GetEscapeKeyDown()
    {
        return inputBinds.IsBindActive("Escape");
    }
    public static bool GetEscapeKey()
    {
        return inputBinds.IsBindActive("Escape",KeybindType.Key);
    }
    public static bool GetInspectKeyDown()
    {
        return inputBinds.IsBindActive("Inspect") && Active();
    }
    #region
    public static bool GetInventorySlotOneKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotOne") && Active();
    }
    public static bool GetInventorySlotTwoKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotTwo") && Active();
    }
    public static bool GetInventorySlotThreeKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotThree") && Active();
    }
    public static bool GetInventorySlotFourKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotFour") && Active();
    }
    public static bool GetInventorySlotFiveKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotFive") && Active();
    }
    public static bool GetInventorySlotSixKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotSix") && Active();
    }
    public static bool GetInventorySlotSevenKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotSeven") && Active();
    }
    public static bool GetInventorySlotEightKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotEight") && Active();
    }
    public static bool GetInventorySlotNineKeyDown()
    {
        return inputBinds.IsBindActive("InventorySlotNine") && Active();
    }
    #endregion

    public static bool Active()
    {
        return enabled && !Terminal.terminalActive && !Escape.active;
    }
    static bool ActiveExcludingEscape()
    {
        return enabled && !Terminal.terminalActive;
    }
}
