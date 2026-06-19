using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PolaroidFilm : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public Material sourceMaterial;

    public Rigidbody rb;

    bool startPrinting;

    [Header("Printing")]
    public float printingTime;
    public float printingSpeed;
    public Vector3 printFinishedPushAmountA;
    public Vector3 printFinishedPushAmountB;
    public Vector3 printFinishedSpinAmountA;
    public Vector3 printFinishedSpinAmountB;
    public void AssignTexture(Texture2D texture)
    {
        Material newMat = new Material(sourceMaterial);
        newMat.mainTexture = texture;
        meshRenderer.material = newMat;

        StartCoroutine(Print());
    }
    private void Update()
    {
        if (startPrinting && rb.isKinematic){
            rb.transform.localPosition+=new Vector3(0f, 0f, Time.deltaTime * printingSpeed);
        }
    }
    public IEnumerator Print()
    {
        startPrinting = true;
        yield return new WaitForSeconds(printingTime);
        rb.isKinematic = false;
        rb.AddRelativeForce(Utilities.RandomRangeVector3(printFinishedPushAmountA, printFinishedPushAmountB));
        rb.AddTorque(Utilities.RandomRangeVector3(printFinishedSpinAmountA, printFinishedSpinAmountB));
    }
}
