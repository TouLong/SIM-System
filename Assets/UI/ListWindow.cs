using UnityEngine;
using UnityEngine.UI;

public class ListWindow : MonoBehaviour
{
    protected ScrollRect scrollRect;
    protected Transform itemTemplate;
    protected Transform content;

    protected void Awake()
    {
        content = transform.Find("List/Viewport/Content");
        itemTemplate = transform.Find("List/Viewport/Content/ItemTemplate");
        scrollRect = transform.GetComponentInChildren<ScrollRect>();
    }

    protected GameObject AddItem(string detail)
    {
        GameObject item = Instantiate(itemTemplate, content).gameObject;
        item.name = "item";
        item.transform.Find("Text").GetComponent<Text>().text = detail;
        item.SetActive(true);
        UpdateScroll();
        return item;
    }
    protected void UpdateItem(GameObject item, string detail)
    {
        item.name = "item";
        item.transform.Find("Text").GetComponent<Text>().text = detail;
        item.SetActive(true);
        UpdateScroll();
    }
    protected void RemoveItem(int index)
    {
        if (content.childCount < index)
        {
            Destroy(content.GetChild(index).gameObject);
            UpdateScroll();
        }
    }
    protected void ClearItems()
    {
        for (int i = 1; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        UpdateScroll();
    }
    void UpdateScroll()
    {
        scrollRect.scrollSensitivity = (content.childCount - 1) * 3;
    }

}
