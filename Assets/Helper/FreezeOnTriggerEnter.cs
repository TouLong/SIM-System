using UnityEngine;
using System;

public class FreezeOnTriggerEnter : MonoBehaviour
{
    public float delayTime = 3;
    Coroutine freeze;
    public Action onEnter;
    public bool destroyGameObject;
    void OnTriggerEnter(Collider other)
    {
        if (freeze == null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                freeze = Timer.Set(delayTime, () =>
                {
                    transform.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    onEnter?.Invoke();
                    if (destroyGameObject)
                        Destroy(gameObject);
                    else
                        Destroy(this);
                });
            }
        }
    }
}