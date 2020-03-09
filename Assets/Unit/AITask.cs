using System.Collections.Generic;
using System;
using UnityEngine;
public class TaskManager
{

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
        Add<Task.SupplyToWorkshop<Sawhorse, Wood, WoodPile>>(unit);
    }
    static public void MakingFirewood(UnitAI unit)
    {
        Add<Task.Make<ChoppingSpot>>(unit);
        Add<Task.SupplyToWorkshop<ChoppingSpot, Wood, WoodPile>>(unit);
    }
    static public void StorageWood(UnitAI unit)
    {
        Add<Task.Storage<Wood, WoodPile>>(unit);
        Add<Task.Storage<Branches, BranchesHeap>>(unit);
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
    public virtual void Cancel() { }
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
        public MapResource res;

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
            res.hasInteracted = false;
            End();
            unit.StopAction();
        }
    }

    public class Storage<R, S> : Task where R : Res where S : Storage
    {
        public Res res;
        public Storage storage;

        public override bool GetReady()
        {
            storage = Storage.GetNotFullNear<S>(unit.transform.position);
            res = Res.GetAccessNear<R>(unit.transform.position);
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
            res.hasInteracted = false;
            End();
            unit.Drop();
            unit.StopAction();
        }
    }

    public class SupplyToWorkshop<W, R, S> : Task where W : Workshop where R : MapResource where S : Storage
    {
        public Workshop workshop;
        public MapResource resource;
        public Storage storage;
        bool byStorage;
        public override bool GetReady()
        {
            workshop = Workshop.GetEmptyNear<W>(unit.transform.position);
            storage = Storage.GetNotEmptyNear<S>(unit.transform.position);
            byStorage = storage != null;
            if (storage == null)
                resource = Res.GetAccessNear<R>(unit.transform.position);

            if (workshop != null)
            {
                if (storage != null)
                {
                    Start();
                    return true;
                }
                else if (resource != null)
                {
                    resource.hasInteracted = true;
                    Start();
                    return true;
                }
            }
            return false;
        }

        public override void Execute()
        {
            float dist;
            Res resource;
            Transform moveTo;
            if (byStorage)
            {
                resource = storage.item;
                moveTo = storage.transform;
                dist = 2f;
            }
            else
            {
                resource = this.resource;
                moveTo = resource.transform;
                dist = resource.Interact;
            }
            unit.MoveTo(moveTo, dist, UnitAnim.Walk, () =>
            {
                unit.Action(resource.PickupAnim, 0.5f, () =>
                {
                    if (byStorage)
                        resource = storage.Output();
                    unit.Pickup(resource);
                    resource.hasInteracted = true;
                }, () =>
                {
                    unit.MoveTo(workshop.transform, workshop.Interact, resource.CarryAnim, () =>
                    {
                        unit.Action(resource.PlaceAnim, 0.5f, () =>
                        {
                            workshop.Input(resource);
                        }, () =>
                        {
                            End();
                        });
                    });
                });
            });
        }
    }

    public class SupplyToBuildSpot : Task
    {
        public BuildSpot buildSpot;
        public Storage storage;
        public override bool GetReady()
        {
            buildSpot = BuildSpot.GetNeedSupplyNear(unit.transform.position);
            if (buildSpot == null)
                return false;
            foreach (Type type in buildSpot.neededCost.Keys)
            {
                storage = Storage.GetNotEmptyNear(Res.Prop(type).storageBy, unit.transform.position);
                if (storage != null)
                {
                    buildSpot.neededCost.Modify(type, -1);
                    Start();
                    return true;
                }
            }
            return false;
        }

        public override void Execute()
        {
            Res resource = null;
            unit.MoveTo(storage.transform, storage.Interact, UnitAnim.Walk, () =>
            {
                unit.Action(storage.item.PickupAnim, 0.5f, () =>
                {
                    resource = storage.Output();
                    unit.Pickup(resource);
                    resource.hasInteracted = true;
                }, () =>
                {
                    unit.MoveTo(buildSpot.transform, buildSpot.Interact, resource.CarryAnim, () =>
                    {
                        unit.Action(resource.PlaceAnim, 0.5f, () =>
                        {
                            buildSpot.Input(resource);
                        }, () =>
                        {
                            End();
                        });
                    });
                });
            });
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

    public class BuildObject : Task
    {
        BuildSpot buildSpot;
        public override bool GetReady()
        {
            buildSpot = Workshop.GetCanProcessingNear<BuildSpot>(unit.transform.position);
            if (buildSpot != null)
            {
                buildSpot.hasInteracted = true;
                Start();
                return true;
            }
            else
                return false;
        }
        public override void Execute()
        {
            unit.MoveAndActionPeriod(buildSpot.transform, buildSpot.Interact, UnitAnim.Walk, UnitAnim.AxeV, () =>
            {
                buildSpot.Process();
                return !buildSpot.IsComplete;
            }, () =>
            {
                End();
            });
        }
        public override void Cancel()
        {
            buildSpot.hasInteracted = false;
            End();
            unit.StopAction();
        }
    }
}