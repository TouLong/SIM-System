using UnityEngine;
using System;

public class ObjectPlacement : MonoBehaviour
{
    public BuildSpot buildSpot;
    Res res, request;
    bool build;
    static ObjectPlacement instance;
    public bool IsPlacing => res != null;
    static public void Request(Res res, bool build = false)
    {
        instance.request = res;
        instance.build = build;
        ObjectSelection.enable = false;
    }
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (IsPlacing)
        {
            if (Mouse.LeftDown)
                CreateObject();
            else if (Input.GetKeyUp(KeyCode.Escape) || Mouse.RightUp)
                Cancel();
            else
                Move();
        }
        else if (request != null)
        {
            NewObject(request);
        }
    }

    void NewObject(Res newRes)
    {
        if (build)
        {
            res = Instantiate(buildSpot, Mouse.HitPoint(), Quaternion.identity);
            (res as BuildSpot).SetBuildObject(newRes);
        }
        else
        {
            res = Instantiate(newRes, Mouse.HitPoint(), Quaternion.identity);
        }
        res.enabled = false;
        Rigidbody rig = res.GetComponent<Rigidbody>();
        if (rig != null)
            rig.detectCollisions = false;
    }

    void CreateObject()
    {
        Rigidbody rig = res.GetComponent<Rigidbody>();
        if (rig != null)
            rig.detectCollisions = true;
        res.enabled = true;
        res = null;
    }

    void Move()
    {
        res.transform.position = Mouse.HitPoint();
        if (!build)
            res.transform.position += Vector3.up * 3;
    }

    void Cancel()
    {
        Destroy(res.gameObject);
        res = null;
        request = null;
        ObjectSelection.enable = true;
    }
}

