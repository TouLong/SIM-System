using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class Res : MonoBehaviour
{
    public class ResData : List<Res>
    {
        public ResProp prop;
    }
    #region static
    static readonly Dictionary<Type, ResData> resDic = new Dictionary<Type, ResData>();
    public static List<Type> AllTypes => resDic.Keys.ToList();
    public static ResProp Prop(Type type) => resDic[type].prop;
    public static void SetResProp(List<ResProp> resProps)
    {
        BuildCost.AllTypes = new List<Type>();
        foreach (ResProp resProp in resProps)
        {
            Type type = Type.GetType(resProp.name);
            if (!resDic.ContainsKey(type))
                resDic.Add(type, new ResData());
            resDic[type].prop = resProp;
            if (resProp.isBuildRes)
                BuildCost.AllTypes.Add(type);
            if (resProp.buildable)
                resProp.buildCost = new BuildCost(resProp.buildCostMeta);
            if (resProp.storable)
                resProp.storageBy = Type.GetType(resProp.storageByMate);
        }
    }
    public static IEnumerable<Res> Get(Type type)
    {
        resDic.TryGetValue(type, out ResData value);
        return value;
    }
    public static IEnumerable<T> Get<T>(Type type) where T : Res
    {
        if (resDic.TryGetValue(type, out ResData value))
            return value.OfType<T>();
        else
            return null;
    }
    public static IEnumerable<T> Get<T>() where T : Res
    {
        if (resDic.TryGetValue(typeof(T), out ResData value))
            return value.OfType<T>();
        else
            return null;
    }
    public static IEnumerable<T> GetWhere<T>(Func<T, bool> predicate) where T : Res
    {
        if (resDic.TryGetValue(typeof(T), out ResData value))
            return value.OfType<T>().Where(predicate);
        else
            return null;
    }
    public static T GetWhereNear<T>(Func<T, bool> predicate, Vector3 position) where T : Res
    {
        IEnumerable<T> enumerable = GetWhere(predicate);
        if (enumerable == null)
            return null;
        else if (enumerable.Any())
            return enumerable.OrderBy(r => Vector3.Distance(r.transform.position, position)).First();
        else
            return null;
    }
    public static T GetAccessNear<T>(Vector3 position) where T : Res
    {
        return GetWhereNear<T>(a => !a.hasInteracted, position);
    }
    public static void Add(Res obj)
    {
        Type type = obj.GetType();
        if (!resDic.ContainsKey(type))
            resDic.Add(type, new ResData());
        resDic[type].Add(obj);
    }
    public static void Remove(Res obj)
    {
        Type type = obj.GetType();
        if (!resDic.ContainsKey(type))
            return;
        resDic[type].Remove(obj);
    }
    #endregion
    [HideInInspector]
    public bool hasInteracted = false;
    public string ZHTW => resDic[GetType()].prop.zhTW;
    public float Interact => resDic[GetType()].prop.interact;
    public string PickupAnim => resDic[GetType()].prop.pickupAnim;
    public string PlaceAnim => resDic[GetType()].prop.placeAnim;
    public string CarryAnim => resDic[GetType()].prop.carryAnim;
    public BuildCost BuildCost => resDic[GetType()].prop.buildCost;
    public Type StorageBy => resDic[GetType()].prop.storageBy;
    void OnEnable()
    {
        Add(this);
        gameObject.layer = 0;
    }
    void OnDisable()
    {
        Remove(this);
        gameObject.layer = 2;
    }
}
