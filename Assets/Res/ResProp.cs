using UnityEngine;
using System.Collections.Generic;
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
    public string storageByMate;
    public Type storageBy;
    public bool buildable;
    public List<StringInt> buildCostMeta;
    public BuildCost buildCost;
    public bool isBuildRes;
}
[Serializable]
public class StringInt
{
    public string key = "";
    public int value = 0;
    public StringInt(Type key, int value)
    {
        this.key = key.Name;
        this.value = value;
    }
}
