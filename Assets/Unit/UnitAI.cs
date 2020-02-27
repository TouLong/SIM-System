using System.Collections.Generic;
using UnityEngine;
public struct UnitAnim
{
    static public readonly string Empty = "empty";
    static public readonly string Idle = "idle";
    static public readonly string Walk = "walk";
    static public readonly string AxeH = "axe-h";
    static public readonly string AxeV = "axe-v";
    static public readonly string Puckup1 = "left-grab";
    static public readonly string Puckup2 = "take-both";
    static public readonly string Place2 = "place-both";
    static public readonly string Carry = "carry";
    static public readonly string Punch = "punch";
}
public class UnitAI : Unit
{
    public Transform rightHand, leftHand, bothHand;
    [HideInInspector]
    public bool isPending = true;
    public Res holdingObject;
    public List<Task> tasks;
    new void Awake()
    {
        base.Awake();
        tasks = new List<Task>();
    }
    public void Pickup(Res res)
    {
        if (res.isSmallObject)
            res.transform.SetParent(leftHand);
        else
            res.transform.SetParent(bothHand);
        res.transform.localPosition = Vector3.zero;
        res.transform.localRotation = Quaternion.identity;
        Rigidbody rigidbody = res.GetComponent<Rigidbody>();
        if (rigidbody != null)
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        holdingObject = res;
    }
}
