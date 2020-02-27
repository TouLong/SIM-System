using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class Res : MonoBehaviour
{
    #region static
    static readonly Dictionary<Type, List<object>> resDic = new Dictionary<Type, List<object>>();

    public static List<object> Get(Type type)
    {
        resDic.TryGetValue(type, out List<object> value);
        return value;
    }
    public static IEnumerable<T> GetAll<T>()
    {
        if (resDic.TryGetValue(typeof(T), out List<object> value))
            return value.OfType<T>();
        else
            return null;
    }
    public static IEnumerable<T> GetWhere<T>(Func<T, bool> predicate)
    {
        if (resDic.TryGetValue(typeof(T), out List<object> value))
            return value.OfType<T>().Where(predicate);
        else
            return null;
    }
    public static T GetNear<T>(Vector3 position) where T : Res
    {
        IEnumerable<T> enumerable = GetAll<T>();
        if (enumerable == null)
            return null;
        else if (enumerable.Any())
            return enumerable.OrderBy(r => Vector3.Distance(r.transform.position, position)).First();
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
    public static T GetWhereNear<T>(Type type, Func<T, bool> predicate, Vector3 position) where T : Res
    {
        IEnumerable<T> enumerable = Get(type).OfType<T>().Where(predicate);
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

    public static void Add(object obj)
    {
        Type type = obj.GetType();
        if (!resDic.ContainsKey(type))
            resDic.Add(type, new List<object>());
        resDic[type].Add(obj);
    }
    public static void Remove(object obj)
    {
        Type type = obj.GetType();
        if (!resDic.ContainsKey(type))
            return;
        resDic[type].Remove(obj);
    }

    #endregion
    [HideInInspector]
    public bool hasInteracted = false;
    [HideInInspector]
    public bool isSmallObject;
    public int radius;
    protected void Start()
    {
        Add(this);
    }
    void OnDisable()
    {
        Remove(this);
    }
}