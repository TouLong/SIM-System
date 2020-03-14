using UnityEngine;
using UnityEngine.EventSystems;

public static class MouseRay
{
    static readonly Camera cam = Camera.main;

    static public Vector3 Point()
    {
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
        return hit.point;
    }

    static public Vector3 SnapPosition(float unit)
    {
        Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
        return new Vector3(unit * Mathf.Round(hit.point.x / unit),
                           hit.point.y,
                           unit * Mathf.Round(hit.point.z / unit));
    }

    static public GameObject HitObject()
    {
        Hit(out RaycastHit hit);
        return hit.transform.gameObject;
    }
    static public bool Hit(out RaycastHit hit)
    {
        return Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);
    }

    static public bool IsOverUI => EventSystem.current.IsPointerOverGameObject();
}

