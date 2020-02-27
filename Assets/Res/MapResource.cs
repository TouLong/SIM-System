using UnityEngine;
public class MapResource : Res
{
    public int durability = 6;
    public bool CanGather => durability > 0 && !hasInteracted;
    public static T GetCanGatherNear<T>(Vector3 position) where T : MapResource
    {
        return GetWhereNear<T>(a => a.CanGather && !a.hasInteracted, position);
    }
    public void Gather()
    {
        durability--;
        if (durability <= 0)
            Gathered();
    }
    protected virtual void Gathered()
    {
        durability = 0;
    }
}

