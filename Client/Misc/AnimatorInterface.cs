using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorInterface : MonoBehaviour // This is literally useless as UnityEvents dont allow you to pass more than one value in the inspector. WHY?
{
    public Animator animator;
    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }
    public void Setint(string name, int value)
    {
        animator.SetInteger(name, value);
    }
}
