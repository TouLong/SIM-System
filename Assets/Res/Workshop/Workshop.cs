using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Workshop : Res
{
    public Res product;
    protected float complete;
    protected float processing;
    public bool IsComplete => processing >= complete;
    public bool IsEmpty => processing == 0;
    public bool CanProcessing => processing > 0 && processing < complete;
    public virtual void Process()
    {

    }
    public virtual void Input(Res res)
    {
    }
    public virtual Res Output()
    {
        return default;
    }
    public static T GetEmptyNear<T>(Vector3 position) where T : Workshop
    {
        return GetWhereNear<T>(a => a.IsEmpty && !a.hasInteracted, position);
    }
    public static T GetCanProcessingNear<T>(Vector3 position) where T : Workshop
    {
        return GetWhereNear<T>(a => a.CanProcessing && !a.hasInteracted, position);
    }
    public static T GetDoneNear<T>(Vector3 position) where T : Workshop
    {
        return GetWhereNear<T>(a => a.IsComplete && !a.hasInteracted, position);
    }
}
