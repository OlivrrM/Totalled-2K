using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDontDestroyOnLoad : MonoBehaviour
{
    public void Set(){
        DontDestroyOnLoad(gameObject);
    }
    public void SetTarget(GameObject target){
        DontDestroyOnLoad(target);
    }
}
