using UnityEngine;

public class BuildSpot : Workshop
{

    void Start()
    {
        complete = 5;
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
        processing = 1;
        Destroy(res.gameObject);
    }
    public void SetCost()
    {

    }
    void ProcessComplete()
    {
        processing = complete;
        Instantiate(product, transform.position, transform.rotation);
        Timer.Set(0.1f, () => Destroy(gameObject));
    }
}
