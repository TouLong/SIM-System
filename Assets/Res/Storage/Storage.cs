using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Storage : Res
{
    protected Transform[] items;
    public uint capacity;
    public uint count;
    public Transform inventory;
    public Transform interact;
    public Res item;
    void Start()
    {
        items = new Transform[inventory.childCount];
        for (int i = 0; i < inventory.childCount; ++i)
        {
            items[i] = inventory.GetChild(i);
        }
        UpdateInventory();
    }
    void UpdateInventory()
    {
        for (int i = 0; i < capacity; ++i)
        {
            items[i].gameObject.SetActive(i < count);
        }
    }
    public bool IsEmpty => count == 0;
    public bool IsFull => count >= capacity;
    public static Storage GetNotFullNear<T>(Vector3 position) where T : Storage
    {
        return GetWhereNear<T>(a => !a.IsFull, position);
    }
    public static Storage GetNotEmptyNear<T>(Vector3 position) where T : Storage
    {
        return GetWhereNear<T>(a => !a.IsEmpty, position);
    }
    public virtual void Input(Res res)
    {
        Destroy(res.gameObject);
        if (!IsFull)
            count++;
        UpdateInventory();
    }
    public virtual Res Output()
    {
        if (!IsEmpty)
            count--;
        UpdateInventory();
        Res res = Instantiate(item);
        FreezeOnTriggerEnter freezeOnTrigger = res.GetComponentInChildren<FreezeOnTriggerEnter>();
        if (res.GetComponentInChildren<FreezeOnTriggerEnter>() != null)
            Destroy(freezeOnTrigger);
        return res;
    }
}
