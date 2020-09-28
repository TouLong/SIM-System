using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tree : MapResource
{
    public Branches branches;
    public Leaves leaves;
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
        FreezeOnTriggerEnter leavesFreezeTrigger = leaves.gameObject.AddComponent<FreezeOnTriggerEnter>();
        leavesFreezeTrigger.delayTime = 0.2f;
        leavesFreezeTrigger.onEnter = () =>
        {
            rigidbody.Sleep();
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            leaves.transform.SetParent(Game.Group("Leaves"));
            leaves.onGathered = () =>
            {
                if (rigidbody != null)
                {
                    rigidbody.WakeUp();
                    rigidbody.constraints = RigidbodyConstraints.None;
                }
                while (branchesAmount > 0)
                {
                    Instantiate(branches, leaves.transform.position + V3Random.Range(-1, 1), Q4Random.Value(), Game.Group("Branches"));
                    branchesAmount--;
                }
            };
            leaves.enabled = true;
        };
        FreezeOnTriggerEnter logFreezeTrigger = log.gameObject.AddComponent<FreezeOnTriggerEnter>();
        logFreezeTrigger.onEnter = () =>
        {
            log.transform.SetParent(Game.Group("Logs"));
            log.enabled = true;
            Destroy(gameObject);
        };
    }

}
