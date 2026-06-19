using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.PostFX;
using Cinemachine.Utility;

public class CinemachineCameraRotationFixer : CinemachineCameraRotation
{
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    )
    {
        if (stage != m_ApplyAfter)
        {
            return;
        }

        state.RawOrientation = state.RawOrientation.ApplyCameraRotation(m_Offset, state.ReferenceUp);
    }
}
