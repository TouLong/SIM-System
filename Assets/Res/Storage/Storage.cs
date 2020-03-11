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
        return GetWhereNear<T>(x => !x.IsFull, position);
    }
    public static Storage GetNotEmptyNear<T>(Vector3 position) where T : Storage
    {
        return GetWhereNear<T>(x => !x.IsEmpty, position);
    }
    public static Storage GetNotEmptyNear(Type type, Vector3 position)
    {
        IEnumerable<Storage> storages = Get<Storage>(type);
        if (storages == null)
            return null;
        if (storages.Any())
        {
            storages = storages.Where(x => !x.IsEmpty);
            if (storages.Any())
                return storages.OrderBy(r => Vector3.Distance(r.transform.position, position)).First();
        }
        return null;
    }
    public static Storage GetNotFullNear(Type type, Vector3 position)
    {
        IEnumerable<Storage> storages = Get<Storage>(type);
        if (storages == null)
            return null;
        if (storages.Any())
        {
            storages = storages.Where(x => !x.IsFull);
            if (storages.Any())
                return storages.OrderBy(r => Vector3.Distance(r.transform.position, position)).First();
        }
        return null;
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
