using System.Collections.Generic;
using System;
using UnityEngine;
public class TaskManager
{
    static public bool stop;
    static public void Add<T>(UnitAI unit) where T : Task, new()
    {
        unit.tasks.Add(new T() { unit = unit });
        TaskWindow.UpdateContent();
    }
    static public void Add<T>(List<UnitAI> units) where T : Task, new()
    {
        foreach (UnitAI unit in units)
            unit.tasks.Add(new T() { unit = unit });
        TaskWindow.UpdateContent();
    }
    static public void Update()
    {
        if (stop) return;
        foreach (UnitAI unit in Game.units)
        {
            if (unit.isPending)
            {
                foreach (Task task in unit.tasks)
                {
                    if (task.GetReady())
                    {
                        task.Execute();
                        break;
                    }
                }
            }
        }
    }
    static public void AllStop()
    {
        foreach (UnitAI unit in Game.units)
        {
            unit.task?.Cancel();
        }
    }
    static public void Logging(UnitAI unit)
    {
        Add<Task.Gather<Log>>(unit);
        Add<Task.Gather<Leaves>>(unit);
        Add<Task.Gather<Tree>>(unit);
    }
    static public void MakingWoodenPlank(UnitAI unit)
    {
        Add<Task.Make<Sawhorse>>(unit);
        Add<Task.SupplyToWorkshop<Sawhorse>>(unit);
    }
    static public void MakingFirewood(UnitAI unit)
    {
        Add<Task.Make<ChoppingSpot>>(unit);
        Add<Task.SupplyToWorkshop<ChoppingSpot>>(unit);
    }
    static public void StorageWood(UnitAI unit)
    {
        Add<Task.Storage<Wood>>(unit);
        Add<Task.Storage<Branches>>(unit);
    }
}

public class Task
{
    #region base
    public UnitAI unit;
    public string name;
    public virtual bool GetReady()
    {
        return default;
    }
    public virtual void Execute() { }
    public virtual void Cancel()
    {
        End();
        unit.StopAction();
    }
    public void Start()
    {
        unit.isPending = false;
        unit.task = this;
    }
    public void End()
    {
        unit.isPending = true;
        unit.task = null;
    }
    #endregion

    public class Gather<T> : Task where T : MapResource
    {
        MapResource res;

        public override bool GetReady()
        {
            res = MapResource.GetCanGatherNear<T>(unit.transform.position);
            if (res != null)
            {
                res.hasInteracted = true;
                Start();
                return true;
            }
            else
                return false;
        }

        public override void Execute()
        {
            unit.MoveAndActionPeriod(res.transform, res.Interact, UnitAnim.Walk, UnitAnim.Punch, () =>
            {
                res.Gather();
                return res.durability > 0;
            }, () =>
            {
                res.hasInteracted = false;
                End();
            });
        }
        public override void Cancel()
        {
            base.Cancel();
            res.hasInteracted = false;
        }
    }

    public class Storage<R> : Task where R : Res
    {
        Res res;
        Storage storage;

        public override bool GetReady()
        {
            res = Res.GetAccessNear<R>(unit.transform.position);
            if (res != null)
                storage = Storage.GetNotFullNear(res.StorageBy, unit.transform.position);
            if (res != null && storage != null)
            {
                res.hasInteracted = true;
                Start();
                return true;
            }
            else
                return false;
        }

        public override void Execute()
        {
            unit.MoveTo(res.transform, res.Interact, UnitAnim.Walk, () =>
            {
                unit.Action(res.PickupAnim, 0.5f, () =>
                {
                    unit.Pickup(res);
                }, () =>
                {
                    unit.MoveTo(storage.transform, 2f, res.CarryAnim, () =>
                    {
                        unit.Action(res.PlaceAnim, 0.2f, () =>
                        {
                            storage.Input(res);
                        }, () =>
                        {
                            End();
                        });
                    });
                });
            });
        }
        public override void Cancel()
        {
            unit.Drop();
            base.Cancel();
            res.hasInteracted = false;
        }
    }

    public class SupplyToWorkshop<W> : Task where W : Workshop
    {
        Workshop workshop;
        Res res;
        Res getRes;
        Type resType;
        public override bool GetReady()
        {
            workshop = Workshop.GetEmptyNear<W>(unit.transform.position);
            if (workshop == null)
                return false;
            foreach (Type type in workshop.neededCost.Keys)
            {
                getRes = Storage.GetNotEmptyNear(Res.Prop(type).storageBy, unit.transform.position);
                if (getRes != null)
                {
                    resType = type;
                    workshop.neededCost.Modify(type, -1);
                    Start();
                    return true;
                }
                getRes = Res.GetAccessNear(type, unit.transform.position);
                if (getRes != null)
                {
                    res = getRes;
                    res.hasInteracted = true;
                    resType = type;
                    workshop.neededCost.Modify(type, -1);
                    Start();
                    return true;
                }
            }
            return false;
        }

        public override void Execute()
        {
            unit.MoveTo(getRes.transform, getRes.Interact, UnitAnim.Walk, () =>
            {
                unit.Action(res == null ? (getRes as Storage).item.PickupAnim : res.CarryAnim, 0.5f, () =>
                {
                    if (res == null)
                    {
                        res = (getRes as Storage).Output();
                        res.hasInteracted = true;
                    }
                    unit.Pickup(res);
                }, () =>
                {
                    unit.MoveTo(workshop.transform, workshop.Interact, res.CarryAnim, () =>
                    {
                        unit.Action(res.PlaceAnim, 0.5f, () =>
                        {
                            workshop.Input(res);
                        }, () =>
                        {
                            End();
                        });
                    });
                });
            });
        }
        public override void Cancel()
        {
            base.Cancel();
            workshop.neededCost.Modify(resType, 1);
            res = null;
        }
    }

    public class Make<W> : Task where W : Workshop
    {
        public Workshop workshop;

        public override bool GetReady()
        {
            workshop = Workshop.GetCanProcessingNear<W>(unit.transform.position);
            if (workshop != null)
            {
                workshop.hasInteracted = true;
                Start();
                return true;
            }
            else
                return false;
        }

        public override void Execute()
        {
            unit.MoveAndActionPeriod(workshop.transform, workshop.Interact, UnitAnim.Walk, UnitAnim.AxeV, () =>
            {
                workshop.Process();
                return !workshop.IsComplete;
            }, () =>
            {
                workshop.hasInteracted = false;
                End();
            });
        }
        public override void Cancel()
        {
            workshop.hasInteracted = false;
            End();
            unit.StopAction();
        }
    }

}