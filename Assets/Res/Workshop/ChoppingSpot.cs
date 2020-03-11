using UnityEngine;

public class ChoppingSpot : Workshop
{
    public GameObject wood;
    public Transform[] firewoods;
    void Start()
    {
        complete = 8;
        neededCost = Material.Clone;
        currentCost = new BuildCost();
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
        if (neededCost.IsEmpty)
            processing = 1;
        if (processing == 1)
            wood.SetActive(true);
    }

    void ProcessComplete()
    {
        wood.SetActive(false);
        processing = complete;
        foreach (Transform trans in firewoods)
        {
            Instantiate(product, trans.position, trans.rotation, Game.Group(product.name));
        }
        Timer.Set(0.1f, () =>
        {
            processing = 0;
            neededCost = Material.Clone;
            currentCost = new BuildCost();
        });
    }
}
