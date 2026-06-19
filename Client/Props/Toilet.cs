using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : Interactable
{
    public Animator animator;
    public float flushTime;
    public PlaySound flushSfx;
    public override void Interact()
    {
        base.Interact();
        interactable = false;
        description = "";
        StartCoroutine(Flush());
    }
    IEnumerator Flush()
    {
        flushSfx.Play();
        animator.SetBool("Flush",true);
        yield return new WaitForSeconds(flushTime);
        animator.SetBool("Flush", false);
        interactable = true;
        description = "Flush Toilet";
    }
}
