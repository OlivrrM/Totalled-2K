using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelCurrentRunningSubtitlesOnDestroy : MonoBehaviour {
    private void OnDestroy() { Cache.subtitleManager.CancelCurrentSubtitles(); }
}
