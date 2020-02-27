using UnityEngine;
using System;

public class ObjectPlacement : MonoBehaviour
{
    Res res;
    bool build;
    public bool IsPlacing => res != null;
    public void SetObject(Res res, bool build = true)
    {
        this.res = res;
        this.build = build;
    }

    void Update()
    {
        if (res == null) return;
        if (Input.GetMouseButtonDown(0))
            CreateObject();
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            Cancel();
        else
            Move();
    }

    void CreateObject()
    {
        if (build)
            new Build(res);
        res = null;
    }

    void Move()
    {
        res.transform.position = MouseRay.SnapPosition(1);
    }

    void Cancel()
    {
        Destroy(res.gameObject);
        res = null;
    }
}

