using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
    static Game instance;
    static public ObjectPlacement ObjectPlacement;

    static public List<UnitAI> units;
    static public List<UnitAI> selectedUnit = new List<UnitAI>();
    void Awake()
    {
        instance = this;
        ObjectPlacement = GetComponent<ObjectPlacement>();
        transform.Find("MainCanvas").gameObject.SetActive(true);
        units = transform.Find("Units").GetComponentsInChildren<UnitAI>().ToList();
    }

    void Start()
    {
        TaskManager.Logging(units[0]);
        TaskManager.Logging(units[1]);
        TaskManager.Logging(units[2]);
        TaskManager.MakingFirewood(units[3]);
        TaskManager.MakingWoodenPlank(units[4]);
        TaskManager.StorageWood(units[5]);
        TaskManager.Add<Task.Storage<Branches, BranchesHeap>>(units[6]);
        TaskManager.Add<Task.Storage<WoodenPlank, WoodenPlankPile>>(units[7]);
        TaskManager.Add<Task.Storage<WoodenPlank, WoodenPlankPile>>(units[8]);
        TaskManager.Add<Task.Storage<Firewood, FirewoodPile>>(units[9]);
        TaskManager.Add<Task.Storage<Firewood, FirewoodPile>>(units[10]);
    }

    void Update()
    {
        TaskManager.Update();
        if (Input.GetMouseButtonDown(0) && !ObjectPlacement.IsPlacing && !MouseRay.IsOverUI)
        {
            GameObject gameObject = MouseRay.HitObject();
            if (gameObject != null)
            {
                selectedUnit.Clear();
                UnitWindow.DeselectAll();
                UnitAI unit = gameObject.GetComponent<UnitAI>();
                if (unit != null)
                    UnitWindow.Select(unit);
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
        }
    }

    public static Transform Group(string name)
    {
        Transform ret = instance.transform.Find(name);
        if (ret != null)
        {
            return instance.transform.Find(name);
        }
        else
        {
            GameObject group = new GameObject(name);
            group.transform.SetParent(instance.transform);
            return group.transform;
        }
    }
}
