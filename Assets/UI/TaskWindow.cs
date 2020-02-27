﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TaskWindow : ListWindow
{
    static TaskWindow instance;
    static readonly Dictionary<Type, string> taskInfo = new Dictionary<Type, string>()
    {
        {typeof(Task.Gather<Tree>),"劈樹" },
        {typeof(Task.Gather<Leaves>),"清樹" },
        {typeof(Task.Gather<Wood>),"砍木" },
        {typeof(Task.SupplyToWorkshop<Sawhorse, Log, WoodPile>),"供應木板站" },
        {typeof(Task.SupplyToWorkshop<ChoppingSpot, Log, WoodPile>),"供應木柴站" },
        {typeof(Task.Make<Sawhorse, WoodenPlank>),"製作木板" },
        {typeof(Task.Make<ChoppingSpot, Firewood>),"製作木柴" },
        {typeof(Task.Storage<Log, WoodPile>),"儲存原木" },
        {typeof(Task.Storage<Branches, BranchesHeap>),"儲存樹枝" },
        {typeof(Task.Storage<WoodenPlank, WoodenPlankPile>),"儲存木板" },
        {typeof(Task.Storage<Firewood, FirewoodPile>),"儲存木柴" },
    };
    new void Awake()
    {
        base.Awake();
        instance = this;
    }

    public static void UpdateContent()
    {
        if (Game.selectedUnit.Count == 0)
        {
            instance.ClearItems();
            return;
        }
        List<Task> tasks = Game.selectedUnit[0].tasks;
        instance.ClearItems();
        if (tasks != null)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                taskInfo.TryGetValue(tasks[i].GetType(), out string info);
                instance.AddItem(info ?? "無資料");
            }
        }
    }
}
