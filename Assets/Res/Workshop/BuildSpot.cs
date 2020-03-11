using UnityEngine;

public class BuildSpot : Workshop
{
    static public BuildSpot GetNeedSupplyNear(Vector3 position)
    {
        return GetWhereNear<BuildSpot>(x => !x.neededCost.IsEmpty, position);
    }
    public void SetBuildObject(Res res)
    {
        product = res;
        neededCost = product.BuildCost.Clone;
        currentCost = new BuildCost();
        complete = neededCost.ValueCount + 1;
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
        base.Input(res);
        if (neededCost.IsEmpty && product.BuildCost.IsEqual(currentCost))
            processing = 1;
    }
    void ProcessComplete()
    {
        processing = complete;
        Instantiate(product, transform.position, transform.rotation);
        Timer.Set(0.1f, () => Destroy(gameObject));
    }
}
