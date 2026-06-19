using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf;
using Fragsurf.Movement;

public class PlayerPush : MonoBehaviour //Test code for pushing player. Can be manipulated via invoke
{
    public void Push(Vector3 force) { Cache.moveData.Velocity += force; }
}
