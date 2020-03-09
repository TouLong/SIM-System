using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using static UnityEngine.UI.Button;

public class CommandPanel : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public Transform commandPrefab;
    public List<Res> resList;
    public List<Res> buildList;
    List<SubCommandPanel> panelLsit;
    SubCommandPanel BuildPanel;
    SubCommandPanel ProcessingPanel;
    SubCommandPanel GatherPanel;
    SubCommandPanel ObjectPanel;
    SubCommandPanel TransPanel;

    void Awake()
    {
        panelLsit = new List<SubCommandPanel>();

        BuildPanel = SetupSubPanel("建造");
        ProcessingPanel = SetupSubPanel("製造");
        GatherPanel = SetupSubPanel("採集");
        TransPanel = SetupSubPanel("運輸");
        ObjectPanel = SetupSubPanel("物品");

        SetTaskCommand<Task.Gather<Tree>>("劈樹", GatherPanel);
        SetTaskCommand<Task.Gather<Leaves>>("清樹", GatherPanel);
        SetTaskCommand<Task.Gather<Log>>("砍木", GatherPanel);
        SetTaskCommand<Task.Make<Sawhorse>>("木板", ProcessingPanel);
        SetTaskCommand<Task.Make<ChoppingSpot>>("木柴", ProcessingPanel);
        SetTaskCommand<Task.SupplyToWorkshop<ChoppingSpot, Wood, WoodPile>>("木板站", TransPanel);
        SetTaskCommand<Task.SupplyToWorkshop<Sawhorse, Wood, WoodPile>>("木柴站", TransPanel);
        SetTaskCommand<Task.Storage<Wood, WoodPile>>("原木", TransPanel);
        SetTaskCommand<Task.Storage<Branches, BranchesHeap>>("樹枝", TransPanel);
        SetTaskCommand<Task.Storage<WoodenPlank, WoodenPlankPile>>("木板", TransPanel);
        SetTaskCommand<Task.Storage<Firewood, FirewoodPile>>("木柴", TransPanel);
        foreach (Res res in resList)
        {
            SetCommand(res.ZHTW, ObjectPanel).AddListener(() => { ObjectPlacement.Request(res); });
        }
        foreach (Res res in buildList)
        {
            SetCommand(res.ZHTW, BuildPanel).AddListener(() => { ObjectPlacement.Request(res, true); });
        }
    }

    SubCommandPanel SetupSubPanel(string panelName)
    {
        SubCommandPanel panel = new SubCommandPanel(transform.Find(panelName));
        panelLsit.Add(panel);
        panel.button.onClick.AddListener(() =>
        {
            ClickPanel(panel);
        });
        return panel;
    }

    void SetTaskCommand<T>(string commandName, SubCommandPanel panel) where T : Task, new()
    {
        Transform command = Instantiate(commandPrefab.gameObject, panel.content).transform;
        command.Find("Button/Title").GetComponent<Text>().text = commandName;
        command.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            TaskManager.Add<T>(Game.selectedUnit);
        });
    }

    ButtonClickedEvent SetCommand(string commandName, SubCommandPanel panel)
    {
        Transform command = Instantiate(commandPrefab.gameObject, panel.content).transform;
        command.Find("Button/Title").GetComponent<Text>().text = commandName;
        return command.Find("Button").GetComponent<Button>().onClick;
    }

    void ClickPanel(SubCommandPanel selectPanel)
    {
        foreach (SubCommandPanel panel in panelLsit)
        {
            if (panel != selectPanel)
                panel.Hide();
            else
                panel.Switch();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            foreach (SubCommandPanel panel in panelLsit)
            {
                panel.Hide();
            }
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    //Dictionary<Transform, SubCommandPanel> panels;
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    foreach (Transform panel in panels.Keys)
    //    {
    //        if (eventData.hovered.Contains(panel.gameObject))
    //        {
    //            ClickPanel(panels[panel]);
    //            return;
    //        }
    //    }
    //}
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    foreach (SubCommandPanel panel in panels.Values)
    //    {
    //        panel.Hide();
    //    }
    //    eventData.selectedObject = null;
    //}

    class SubCommandPanel
    {
        public readonly Transform panel;
        public readonly Transform content;
        public readonly Button button;
        public readonly string name;
        public bool active;

        public SubCommandPanel(Transform panel)
        {
            this.panel = panel;
            name = panel.name;
            content = panel.Find("Content");
            button = panel.Find("Toggle").GetComponent<Button>();
            Hide();
        }

        public void Hide()
        {
            active = false;
            content.gameObject.SetActive(active);
        }

        public void Show()
        {
            active = true;
            content.gameObject.SetActive(active);
        }

        public void Switch()
        {
            active = !active;
            content.gameObject.SetActive(active);
        }
    }
}
