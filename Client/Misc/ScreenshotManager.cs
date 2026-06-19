using UnityEngine;
using System.IO;

public class ScreenshotManager : MonoBehaviour
{
    private void Update()
    {
        if (InputManager.GetScreenshotKeyDown())
        {
            SaveScreenshot();
        }
    }
    public static void SaveScreenshot()
    {
        string folderPath = Path.Combine(Application.dataPath, "../Screenshots");
        if (!Directory.Exists(folderPath)){
            Directory.CreateDirectory(folderPath);
        }
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string filePath = Path.Combine(folderPath, fileName);
        ScreenCapture.CaptureScreenshot(filePath);
        Cache.terminal.Print($"Screenshot saved to {filePath}");
    }
}