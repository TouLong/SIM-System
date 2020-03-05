using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ResProp : ScriptableObject
{
    public string zhTW;
    public bool portable;
    public string pickupAnim;
    public string carryAnim;
    public string placeAnim;
    public float interact;
    public bool buildable;
    public BuildCost buildCost;
}
[System.Serializable]
public class BuildCost
{
    public int log;
    public int woodenPlank;
    public int firewood;
    public int branches;
    public int rock;
}