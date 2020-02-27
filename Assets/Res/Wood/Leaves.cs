using System;
using UnityEngine;

public class Leaves : MapResource
{
    public Action onGathered;

    protected override void Gathered()
    {
        base.Gathered();
        onGathered?.Invoke();
        Destroy(gameObject);
    }
}
