using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UnitWindow : ListWindow
{
    static UnitWindow instance;
    static Dictionary<UnitAI, Toggle> units;
    new void Awake()
    {
        base.Awake();
        units = new Dictionary<UnitAI, Toggle>();
        instance = this;
    }
    void Start()
    {
        foreach (UnitAI unit in Game.units)
        {
            Add(unit);
        }
    }
    static public void Add(UnitAI unit)
    {
        Toggle toggle = instance.AddItem(unit.name).GetComponent<Toggle>();
        units.Add(unit, toggle);
        toggle.onValueChanged.AddListener((bool b) =>
        {
            unit.GetComponent<Outline>().enabled = b;
            if (b)
                Game.selectedUnit.Add(unit);
            else
                Game.selectedUnit.Remove(unit);
            TaskWindow.UpdateContent();
        });
    }
    static public void Select(UnitAI unit)
    {
        units[unit].isOn = true;
    }
    static public void Deselect(UnitAI unit)
    {
        units[unit].isOn = false;
    }
    static public void DeselectAll()
    {
        foreach (UnitAI unit in Game.units)
        {
            Deselect(unit);
        }
    }

}
