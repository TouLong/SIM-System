using UnityEngine;

public class ChoppingSpot : Workshop
{
    public GameObject log;
    public Transform[] firewoods;
    new void Start()
    {
        base.Start();
        complete = 8;
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
        log.SetActive(true);
        processing = 1;
        Destroy(res.gameObject);
    }

    void ProcessComplete()
    {
        log.SetActive(false);
        processing = complete;
        foreach (Transform trans in firewoods)
        {
            Instantiate(product, trans.position, trans.rotation, Game.Group(product.name));
        }
        Timer.Set(0.1f, () => processing = 0);
    }
}
