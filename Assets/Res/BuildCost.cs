using System.Collections.Generic;
using System.Linq;
using System;

public class BuildCost : Dictionary<Type, int>
{
    static public List<Type> AllTypes;
    public BuildCost() : base() { }
    public BuildCost(IDictionary<Type, int> dictionary) : base(dictionary) { }
    public BuildCost(List<StringInt> metas)
    {
        foreach (StringInt stringInt in metas)
        {
            if (stringInt.value != 0)
                Add(Type.GetType(stringInt.key), stringInt.value);
        }
    }
    public BuildCost Clone => new BuildCost(this);
    public bool IsEmpty => Values.Sum() == 0;
    public int ValueCount => Values.Sum();
    public bool IsEqual(BuildCost buildCost)
    {
        return buildCost.SequenceEqual(buildCost);
    }
    public void Modify(Type res, int amount)
    {
        if (TryGetValue(res, out int v))
        {
            int value = amount + v;
            if (value > 0)
                this[res] = value;
            else
                Remove(res);
        }
        else if (amount > 0)
            Add(res, amount);
    }
}