using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
public class ResProp : ScriptableObject
{
    public string zhTW;
    public float interact;
    public bool portable;
    public string pickupAnim;
    public string carryAnim;
    public string placeAnim;
    public bool storable;
    public Type storageBy;
    public bool buildable;
    public BuildCost buildCost;
}
[System.Serializable]
public class BuildCost : Dictionary<Type, int>
{
    public BuildCost() : base() { }
    public BuildCost(IDictionary<Type, int> dictionary) : base(dictionary) { }
    public BuildCost Clone => new BuildCost(this);
    public bool IsEmpty => Values.Sum() == 0;
    public int ValueCount => Values.Sum();
    public bool IsEqual(BuildCost buildCost)
    {
        return Values == buildCost.Values;
    }
    public void Modify(Type res, int amount)
    {
        if (TryGetValue(res, out int v))
            this[res] += Mathf.Max(0, v + amount);
    }
}