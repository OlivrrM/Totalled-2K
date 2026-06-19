using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WindowManager : MonoBehaviour
{
    // Constants for the Windows API
    private const int SW_MINIMIZE = 6;

    // Import the Windows API function to change the window state
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    // Function to minimize the window
    public void Minimize()
    {
        // Get the current window handle
        IntPtr hWnd = GetActiveWindow();
        // Minimize the window
        ShowWindow(hWnd, SW_MINIMIZE);
    }


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

    // Define constants for MessageBox type
    public const uint MB_OK = 0x00000000;
    public const uint MB_OKCANCEL = 0x00000001;
    public const uint MB_YESNO = 0x00000004;
    public const uint MB_ICONINFORMATION = 0x00000040;

    public void ShowMessage(string title, string message)
    {
        MessageBox(IntPtr.Zero, message, title, MB_OK | MB_ICONINFORMATION);
    }
}
