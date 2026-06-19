using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmAnimator : MonoBehaviour
{
    public Firearm firearm;
    public Animator animator;
    private void Update()
    {
        animator.SetBool("Reloading", firearm.currentReloadTime > 0);
    }
}
