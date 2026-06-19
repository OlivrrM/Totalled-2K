using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inspect : MonoBehaviour
{
    public LayerMask layerMask;

    public ItemPickup itemPickup;

    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI interactBindText;
    Color defaultDescriptionTextCol;

    Interactable currentInteractable;

    public static bool lookingAtEnemy;

    [HideInInspector] public Transform castFrom;
    private void Start()
    {
        Cache.inspect = this;
        castFrom = Camera.main.transform;
        defaultDescriptionTextCol = descriptionText.color;
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        Cache.walkSpeedManager.AddValue("HeldInteract", 1f);
    }
    private void Update()
    {
        Cache.walkSpeedManager.UpdateValue("HeldInteract", 1f);
        bool setDescription = false;
        bool interactable = false;
        lookingAtEnemy = false;
        RaycastHit hit;
        //Debug.DrawRay(castFrom.position, castFrom.forward * 100, Color.red);
        if (Physics.Raycast(castFrom.position, castFrom.forward,out hit, 9999999f, layerMask))
        //if (Physics.Raycast(Cache.vcam.transform.forward, Cache.vcam.transform.forward, out hit, 9999999f, layerMask))
        {
            switch (hit.transform.tag) // This needs to use transform.CompareTag() for better performance
            {
                case "Pickup":
                    if (hit.distance <= itemPickup.pickupRange)
                    {
                        Pickup pickup = hit.transform.gameObject.GetComponent<Pickup>();
                        descriptionText.text = pickup.description;
                        interactBindText.text = $"[{InputManager.inputBinds.savedBinds.binds["Interact"][0].keycode.ToString()}]";
                        interactable = true;
                        if (InputManager.GetInteractKeyDown()&&!pickup.pickedUp)
                        {
                            itemPickup.Pickup(hit.transform.gameObject);
                        }
                        setDescription = true;
                    }
                    break;

                case "Interactable":
                    if (currentInteractable == null) { currentInteractable = hit.transform.GetComponent<Interactable>(); }
                    else if (hit.transform != currentInteractable.transform) { currentInteractable = hit.transform.GetComponent<Interactable>(); }
                    if (currentInteractable.interactable)
                    {
                        if (hit.distance <= currentInteractable.interactableRange)
                        {
                            descriptionText.text = currentInteractable.description;
                            setDescription = true;
                            if (currentInteractable.showBindKey){
                                interactable = true;
                                interactBindText.text = $"[{InputManager.inputBinds.savedBinds.binds["Interact"][0].keycode.ToString()}]";
                            }
                            if (InputManager.GetInteractKeyDown())
                            {
                                currentInteractable.Interact();
                            }
                            if (InputManager.GetInteractKey())
                            {
                                currentInteractable.HeldInteract();
                                Cache.walkSpeedManager.UpdateValue("HeldInteract", 0.5f);
                            }
                        }
                    }
                    else{
                        setDescription = true;
                        descriptionText.text = currentInteractable.uninteractableDescription;
                    }
                    break;
            }
            lookingAtEnemy = hit.transform.gameObject.layer == 12;
        }
        descriptionText.color = Color.Lerp(descriptionText.color, setDescription ? defaultDescriptionTextCol : Utilities.Invisible(defaultDescriptionTextCol), Time.deltaTime * (setDescription ? 5f : 20f));
        descriptionText.rectTransform.localScale = Vector3.Lerp(descriptionText.rectTransform.localScale, setDescription ? Vector3.one : Utilities.V3All(0.8f), Time.deltaTime * (setDescription ? 5f : 20f));
        interactBindText.color = Color.Lerp(interactBindText.color, interactable ? defaultDescriptionTextCol : Utilities.Invisible(defaultDescriptionTextCol), Time.deltaTime * (interactable ? 5f : 20f));
        interactBindText.rectTransform.localScale = Vector3.Lerp(interactBindText.rectTransform.localScale, interactable ? Vector3.one : Utilities.V3All(0.8f), Time.deltaTime * (interactable ? 5f : 20f));
    }
}
