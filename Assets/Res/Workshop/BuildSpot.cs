using UnityEngine;

public class BuildSpot : Workshop
{
    [HideInInspector]
    public BuildCost neededCost;
    BuildCost currentCost;
    static public BuildSpot GetNeedSupplyNear(Vector3 position)
    {
        return Res.GetWhereNear<BuildSpot>(x => !x.neededCost.IsEmpty, position);
    }
    public void SetBuildObject(Res res)
    {
        product = res;
        neededCost = product.BuildCost.Clone;
        currentCost = new BuildCost();
        complete = neededCost.Count;
    }
    public override void Process()
    {
        processing = Mathf.Min(complete, ++processing);
        if (IsComplete)
        {
            ProcessComplete();
        }
    }
    public override void Input(Res res)
    {
        if (neededCost.IsEmpty && product.BuildCost.IsEqual(currentCost))
            processing = 1;
        currentCost.Modify(res.GetType(), 1);
        Destroy(res.gameObject);
    }
    void ProcessComplete()
    {
        processing = complete;
        Instantiate(product, transform.position, transform.rotation);
        Timer.Set(0.1f, () => Destroy(gameObject));
    }
}
