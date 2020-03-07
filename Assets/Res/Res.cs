using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class Res : MonoBehaviour
{
    #region static
    static readonly Dictionary<Type, List<Res>> resDic = new Dictionary<Type, List<Res>>();
    static public Dictionary<Type, ResProp> propDic = new Dictionary<Type, ResProp>();
    public float Interact => propDic[GetType()].interact;
    public string PickupAnim => propDic[GetType()].pickupAnim;
    public string PlaceAnim => propDic[GetType()].placeAnim;
    public string CarryAnim => propDic[GetType()].carryAnim;
    public BuildCost BuildCost => propDic[GetType()].buildCost;
    public static IEnumerable<T> Get<T>(Type type) where T : Res
    {
        if (resDic.TryGetValue(type, out List<Res> value))
            return value.OfType<T>();
        else
            return null;
    }
    public static IEnumerable<T> Get<T>() where T : Res
    {
        if (resDic.TryGetValue(typeof(T), out List<Res> value))
            return value.OfType<T>();
        else
            return null;
    }
    public static IEnumerable<T> GetWhere<T>(Func<T, bool> predicate) where T : Res
    {
        if (resDic.TryGetValue(typeof(T), out List<Res> value))
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
            resDic.Add(type, new List<Res>());
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
