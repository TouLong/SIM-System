using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tree : MapResource
{
    public GameObject leavesGO, woodGO;
    public int leavesDurability = 6;
    public int woodDurability = 6;
    public Branches branches;
    public Log log;
    [Range(1, 6)]
    public int branchesAmount;
    protected override void Gathered()
    {
        base.Gathered();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;
        rigidbody.mass = 1;
        rigidbody.AddForce(V3Random.DirectionXZ() * 50);
        //rigidbody.AddForce(Vector3.right * 50);
        FreezeOnTriggerEnter leavesFreezeTrigger = leavesGO.AddComponent<FreezeOnTriggerEnter>();
        leavesFreezeTrigger.delayTime = 0.2f;
        leavesFreezeTrigger.onEnter = () =>
        {
            rigidbody.Sleep();
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Leaves leaves = leavesGO.AddComponent<Leaves>();
            leaves.durability = leavesDurability;
            leaves.radius = 8;
            leaves.onGathered = () =>
            {
                rigidbody.WakeUp();
                rigidbody.constraints = RigidbodyConstraints.None;
                while (branchesAmount > 0)
                {
                    Instantiate(branches, leaves.transform.position, Q4Random.Value(), Game.Group("Branches"));
                    branchesAmount--;
                }
            };
        };
        FreezeOnTriggerEnter woodFreezeTrigger = woodGO.AddComponent<FreezeOnTriggerEnter>();
        woodFreezeTrigger.onEnter = () =>
        {
            Wood wood = woodGO.AddComponent<Wood>();
            wood.log = log;
            wood.durability = leavesDurability;
            wood.radius = 2;
            woodGO.transform.SetParent(Game.Group("Woods"));
            Destroy(gameObject);
        };
    }

}
