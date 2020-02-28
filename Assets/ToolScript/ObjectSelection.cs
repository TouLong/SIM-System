using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectSelection : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform frame;
    Vector3 startPoint, endPoint;
    static public Action<GameObject> onSelect;
    static public Action onDeselect;
    static public List<GameObject> selectebleObjects;
    void Start()
    {
        frame.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !MouseRay.IsOverUI)
        {
            startPoint = Input.mousePosition;
            if (MouseRay.Hit(out RaycastHit hit))
            {
                onDeselect?.Invoke();
                onSelect?.Invoke(hit.transform.gameObject);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            frame.gameObject.SetActive(false);
        }
        if (Input.GetMouseButton(0) && !MouseRay.IsOverUI)
        {
            frame.gameObject.SetActive(true);
            endPoint = Input.mousePosition;
            Vector2 size = new Vector2(Mathf.Abs(startPoint.x - endPoint.x), Mathf.Abs(startPoint.y - endPoint.y));
            frame.position = (startPoint + endPoint) / 2f;
            frame.sizeDelta = canvas.transform.InverseTransformVector(size.x, size.y, 0);
            Rect selectRect = new Rect(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y, endPoint.y), size.x, size.y);
            foreach (GameObject selectebleObject in selectebleObjects)
            {
                if (selectRect.Contains(Camera.main.WorldToScreenPoint(selectebleObject.transform.position)))
                {
                    onSelect?.Invoke(selectebleObject);
                }
            }
        }
    }
}
