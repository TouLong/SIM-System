using System.Collections.Generic;
using System.Linq;
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
                Task task = unit.tasks.FirstOrDefault(t => t.GetReady());
                if (task != null)
                    task.Execute();
            }
        }
    }
    static public void Logging(UnitAI unit)
    {
        Add<Task.Gather<Wood>>(unit);
        Add<Task.Gather<Leaves>>(unit);
        Add<Task.Gather<Tree>>(unit);
    }
    static public void MakingWoodenPlank(UnitAI unit)
    {
        Add<Task.SupplyToWorkshop<Sawhorse, Log, WoodPile>>(unit);
        Add<Task.Make<Sawhorse, WoodenPlank>>(unit);
    }
    static public void MakingFirewood(UnitAI unit)
    {
        Add<Task.SupplyToWorkshop<ChoppingSpot, Log, WoodPile>>(unit);
        Add<Task.Make<ChoppingSpot, Firewood>>(unit);
    }
    static public void StorageWood(UnitAI unit)
    {
        Add<Task.Storage<Log, WoodPile>>(unit);
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
    public virtual void Execute()
    {

    }

    public virtual void Start()
    {
        unit.isPending = false;
    }

    public virtual void Complete()
    {
        unit.isPending = true;
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
            unit.MoveAndActionPeriod(res.transform, res.radius, UnitAnim.Walk, UnitAnim.Punch, () =>
            {
                res.Gather();
                return res.durability > 0;
            }, () =>
            {
                res.hasInteracted = false;
                Complete();
            });
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
            void Storage(string pickup, string carry, string place)
            {
                unit.MoveTo(res.transform, res.radius, UnitAnim.Walk, () =>
                {
                    unit.Action(pickup, 0.5f, () =>
                    {
                        unit.Pickup(res);
                    }, () =>
                    {
                        unit.MoveTo(storage.interact, 2f, carry, () =>
                        {
                            unit.Action(place, 0.5f, () =>
                            {
                                storage.Input(res);
                            }, () =>
                            {
                                Complete();
                            });
                        });
                    });
                });
            }
            if (res.isSmallObject)
                Storage(UnitAnim.Puckup1, UnitAnim.Walk, UnitAnim.Puckup1);
            else
                Storage(UnitAnim.Puckup2, UnitAnim.Carry, UnitAnim.Place2);
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
            void Supply(string pickup, string carry, string place)
            {
                float dist;
                Res resource;
                Transform moveTo;
                if (byStorage)
                {
                    resource = null;
                    moveTo = storage.interact;
                    dist = 2f;
                }
                else
                {
                    resource = this.resource;
                    moveTo = resource.transform;
                    dist = resource.radius;
                }
                unit.MoveTo(moveTo, dist, UnitAnim.Walk, () =>
                {
                    unit.Action(pickup, 0.5f, () =>
                    {
                        if (byStorage)
                            resource = storage.Output();
                        unit.Pickup(resource);
                        resource.hasInteracted = true;
                    }, () =>
                    {
                        unit.MoveTo(workshop.transform, workshop.radius, carry, () =>
                        {
                            unit.Action(place, 0.5f, () =>
                            {
                                workshop.Input(resource);
                            }, () =>
                            {
                                Complete();
                            });
                        });
                    });
                });
            }
            bool isSmallObject;
            if (byStorage)
                isSmallObject = storage.item.isSmallObject;
            else
                isSmallObject = this.resource.isSmallObject;
            if (isSmallObject)
                Supply(UnitAnim.Puckup1, UnitAnim.Walk, UnitAnim.Puckup1);
            else
                Supply(UnitAnim.Puckup2, UnitAnim.Carry, UnitAnim.Place2);
        }
    }

    public class Make<W, P> : Task where W : Workshop where P : Res
    {
        public Workshop workshop;
        public Res product;

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
            unit.MoveAndActionPeriod(workshop.workLocation, workshop.radius, UnitAnim.Walk, UnitAnim.AxeV, () =>
            {
                workshop.Process();
                return !workshop.IsComplete;
            }, () =>
            {
                workshop.hasInteracted = false;
                Complete();
            });
        }
    }
}