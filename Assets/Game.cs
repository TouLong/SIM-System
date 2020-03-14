using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{
    static Game instance;
    static public ObjectPlacement ObjectPlacement;
    public List<ResProp> resProps;

    static public List<UnitAI> units;
    static public List<UnitAI> selectedUnit = new List<UnitAI>();
    void Awake()
    {
        instance = this;
        Res.SetResProp(resProps);
        ObjectPlacement = GetComponent<ObjectPlacement>();
        transform.Find("MainCanvas").gameObject.SetActive(true);
        units = transform.Find("Units").GetComponentsInChildren<UnitAI>().ToList();
        ObjectSelection.onSelect += (GameObject select) =>
        {
            UnitAI unit = select.transform.GetComponent<UnitAI>();
            if (unit != null)
                UnitWindow.Select(unit);
        };
        ObjectSelection.onDeselect += () =>
        {
            UnitWindow.DeselectAll();
        };
        ObjectSelection.selectebleObjects = units.Select(x => x.gameObject).ToList();
    }
    void Start()
    {
        TaskManager.Logging(units[0]);
        TaskManager.Logging(units[1]);
        TaskManager.MakingFirewood(units[2]);
        TaskManager.MakingWoodenPlank(units[3]);
        TaskManager.StorageWood(units[4]);
        TaskManager.StorageWood(units[5]);
        TaskManager.Add<Task.SupplyToWorkshop<BuildSpot>>(units[6]);
        TaskManager.Add<Task.Storage<WoodenPlank>>(units[7]);
        TaskManager.Add<Task.Storage<WoodenPlank>>(units[8]);
        TaskManager.Add<Task.Storage<Firewood>>(units[9]);
        TaskManager.Add<Task.Storage<Firewood>>(units[10]);
        TaskManager.Add<Task.Make<BuildSpot>>(units[11]);
    }

    void Update()
    {
        TaskManager.Update();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TaskManager.stop = !TaskManager.stop;
            if (TaskManager.stop)
                TaskManager.AllStop();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
        }
        if (Input.GetKeyDown(KeyCode.R))
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
