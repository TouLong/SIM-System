using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class GameHelper : EditorWindow
{
    [MenuItem("Window/Game Helper")]
    public static void ShowWindow()
    {
        GetWindow<GameHelper>("Game Helper");
    }
    Vector2 scrollPos;
    public GameObject template;
    public GameObject[] buildCharacters;
    public MeshFilter[] rotateMeshFilters;
    static public bool snapObject;
    void OnGUI()
    {
        minSize = new Vector2(300, 100);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        SerializedObject serializedObject = new SerializedObject(this);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("buildCharacters"), true);
        template = EditorGUILayout.ObjectField("模板", template, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("建造角色"))
        {
            foreach (GameObject character in buildCharacters)
                BuildCharacter(character, template);
        }
        GUILayout.Label("建造完成後還需綁定Animator及Animation");
        HelperWindow.GUILine();
        snapObject = EditorGUILayout.Toggle("貼地快捷鍵: \"`\" ", snapObject);
        if (GUILayout.Button("落地"))
        {
            Land(Selection.gameObjects);
        }
        if (GUILayout.Button("以取底點落地"))
        {
            LandForButtom(Selection.gameObjects);
        }
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndScrollView();
    }

    public void BuildCharacter(GameObject gameObject, GameObject template)
    {
        PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        Transform bone = gameObject.transform.Find("Armature");
        SkinnedMeshRenderer skinnedMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        Transform skinBone = skinnedMesh.transform;
        skinBone.name = bone.name;
        skinnedMesh.updateWhenOffscreen = true;
        for (int i = 0; i < bone.childCount; i++)
        {
            bone.GetChild(i).SetParent(skinBone);
        }
        DestroyImmediate(bone.gameObject);

        if (template == null) return;
        foreach (Component component in template.GetComponents<Component>())
        {
            if (component.GetType() == typeof(Transform)) continue;
            HelperWindow.CopyComponent(component, gameObject);
        }
    }

    public static void Land(GameObject[] objs)
    {
        foreach (GameObject obj in objs)
        {
            Land(obj.transform);
        }
    }
    public static void LandForButtom(GameObject[] objs)
    {
        foreach (GameObject obj in objs)
        {
            LandForButtom(obj.transform);
        }
    }
    public static void LandForButtom(Transform obj)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        Quaternion rotation = obj.transform.rotation;
        Vector3 position = obj.transform.position;
        List<Vector3> vertices = mesh.vertices.ToList();
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = rotation * vertices[i] + position;
        }
        Vector3 origin = vertices.OrderBy(a => a.y).First();
        float offset = Mathf.Abs(obj.position.y - origin.y);
        origin.y = 1000;
        RaycastHit[] downHits = Physics.RaycastAll(origin, Vector3.down);
        for (int i = downHits.Length - 1; i >= 0; i--)
        {
            if (downHits[i].transform != obj.transform)
            {
                origin.y = downHits[i].point.y + offset;
                break;
            }
        }
        obj.transform.position = new Vector3(position.x, origin.y, position.z);
    }
    public static void Land(Transform obj)
    {
        Vector3 origin = obj.transform.position;
        origin.y = 1000;
        RaycastHit[] downHits = Physics.RaycastAll(origin, Vector3.down);
        origin.y = -1000;
        RaycastHit[] upHits = Physics.RaycastAll(origin, Vector3.up);
        for (int i = downHits.Length - 1; i >= 0; i--)
        {
            if (downHits[i].transform != obj.transform)
            {
                origin.y = downHits[i].point.y;
                break;
            }
        }
        for (int i = 0; i < upHits.Length; i++)
        {
            if (upHits[i].transform == obj.transform)
            {
                origin.y -= upHits[i].point.y - obj.transform.position.y;
                break;
            }
        }
        obj.transform.position = origin;
    }
}
[InitializeOnLoad]
public static class EditorHotkeysTracker
{
    static EditorHotkeysTracker()
    {
        SceneView.duringSceneGui += view =>
        {
            if (GameHelper.snapObject)
            {
                if (Event.current.keyCode == KeyCode.BackQuote)
                {
                    GameHelper.Land(Selection.gameObjects);
                }
            }
        };
    }
}

