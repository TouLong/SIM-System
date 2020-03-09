using System.Collections.Generic;
using UnityEngine;
public struct UnitAnim
{
    static public readonly string Empty = "empty";
    static public readonly string Idle = "idle";
    static public readonly string Walk = "walk";
    static public readonly string AxeH = "axe-h";
    static public readonly string AxeV = "axe-v";
    static public readonly string Pickup1 = "left-grab";
    static public readonly string Pickup2 = "take-both";
    static public readonly string Place2 = "place-both";
    static public readonly string Carry = "carry";
    static public readonly string Punch = "punch";
}
public class UnitAI : Unit
{
    public Transform rightHand, leftHand, bothHand;
    [HideInInspector]
    public bool isPending = true;
    public Res holdingRes;
    public List<Task> tasks;
    public Task task;
    new void Awake()
    {
        base.Awake();
        tasks = new List<Task>();
    }
    public void Pickup(Res res)
    {
        if (res.CarryAnim == UnitAnim.Carry)
            res.transform.SetParent(bothHand);
        else
            res.transform.SetParent(leftHand);
        res.transform.localPosition = Vector3.zero;
        res.transform.localRotation = Quaternion.identity;
        Rigidbody rigidbody = res.GetComponent<Rigidbody>();
        if (rigidbody != null)
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        Collider collider = res.GetComponent<Collider>();
        collider.enabled = false;
        holdingRes = res;
    }
    public void Drop()
    {
        if (holdingRes == null)
            return;
        holdingRes.transform.SetParent(null);
        holdingRes.hasInteracted = false;
        Rigidbody rigidbody = holdingRes.GetComponent<Rigidbody>();
        if (rigidbody != null)
            rigidbody.constraints = RigidbodyConstraints.None;
        Collider collider = holdingRes.GetComponent<Collider>();
        collider.enabled = true;
    }
}
