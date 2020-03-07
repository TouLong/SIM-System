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
            if (Input.GetMouseButtonUp(0))
                CreateObject();
            else if (Input.GetKeyUp(KeyCode.Escape) || Input.GetMouseButtonUp(1))
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
            res = Instantiate(buildSpot, MouseRay.Point(), Quaternion.identity);
            (res as BuildSpot).SetBuildObject(newRes);
        }
        else
        {
            res = Instantiate(newRes, MouseRay.Point(), Quaternion.identity);
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
        res.transform.position = MouseRay.Point();
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

