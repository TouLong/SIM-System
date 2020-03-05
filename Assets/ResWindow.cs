using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
public class ResWindow : EditorWindow
{
    Vector2 scrollPos;
    public List<Res> resList = new List<Res>();
    Dictionary<Res, bool> resDict = new Dictionary<Res, bool>();
    List<string> animNames;
    FieldInfo[] buildCostFields;
    [MenuItem("Window/Res Window")]
    public static void ShowWindow()
    {
        GetWindow<ResWindow>("Res Window");
    }
    void OnGUI()
    {
        minSize = new Vector2(450, 100);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        SerializedObject serializedObject = new SerializedObject(this);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("resList"), true);
        serializedObject.ApplyModifiedProperties();
        if (Event.current.type == EventType.DragPerform)
        {
            EditorGUILayout.EndScrollView();
            return;
        }
        //if (animNames == null)
        animNames = typeof(UnitAnim).GetFields().Select(x => (string)x.GetValue(null)).ToList();
        if (buildCostFields == null)
            buildCostFields = typeof(BuildCost).GetFields();
        resList = resList.Distinct().ToList();
        for (int i = 0; i < resList.Count; ++i)
        {
            Res res = resList[i];
            if (!resDict.ContainsKey(res))
            {
                resDict.Add(res, true);
            }
        }
        if (Event.current.type != EventType.DragPerform)
            GUILayout.BeginHorizontal();
        if (GUILayout.Button("++")) ShowAll(true);
        if (GUILayout.Button("--")) ShowAll(false);
        GUILayout.EndHorizontal();
        UIHelper.Line();
        for (int i = 0; i < resList.Count; ++i)
        {
            GUILayout.BeginVertical("Box");
            Res res = resList[i];
            if (res.prop == null)
                res.prop = GetResProp(res);
            ResProp prop = res.prop;
            if (prop.buildCost == null)
                prop.buildCost = new BuildCost();
            BuildCost buildCost = prop.buildCost;
            string name = res.GetType().ToString();
            resDict[res] = EditorGUILayout.Foldout(resDict[res], name);
            if (resDict[res])
            {
                prop.zhTW = EditorGUILayout.TextField("中文", prop.zhTW);
                prop.interact = EditorGUILayout.FloatField("互動範圍", prop.interact);
                prop.portable = EditorGUILayout.Toggle("可攜帶", prop.portable);
                if (prop.portable)
                {
                    prop.carryAnim = animNames[EditorGUILayout.Popup("搬運動畫", Mathf.Max(0, animNames.IndexOf(prop.carryAnim)), animNames.ToArray())];
                    prop.placeAnim = animNames[EditorGUILayout.Popup("放置動畫", Mathf.Max(0, animNames.IndexOf(prop.placeAnim)), animNames.ToArray())];
                    prop.pickupAnim = animNames[EditorGUILayout.Popup("拿起動畫.", Mathf.Max(0, animNames.IndexOf(prop.pickupAnim)), animNames.ToArray())];
                }
                prop.buildable = EditorGUILayout.Toggle("可建造", prop.buildable);
                if (prop.buildable)
                {
                    foreach (FieldInfo field in buildCostFields)
                    {
                        field.SetValue(buildCost, EditorGUILayout.IntField(field.Name, (int)field.GetValue(buildCost)));
                    }
                }
            }
            //EditorUtility.SetDirty(res);
            EditorUtility.SetDirty(prop);
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

    }

    public void ShowAll(bool show)
    {
        for (int i = 0; i < resList.Count; ++i)
        {
            Res res = resList[i];
            resDict[res] = show;
        }
    }

    static public ResProp GetResProp(Res res)
    {
        ResProp prop;
        string localPath = "Assets/Models/Prefab/" + res.GetType().ToString() + ".asset";
        if (File.Exists(localPath))
        {
            prop = AssetDatabase.LoadAssetAtPath(localPath, typeof(ResProp)) as ResProp;
        }
        else
        {
            prop = CreateInstance<ResProp>();
            AssetDatabase.CreateAsset(prop, localPath);
            AssetDatabase.SaveAssets();
        }
        return prop;
    }
    static public void ListField(string label, int count, Action add, Action remove)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label + ": " + count.ToString());
        if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
            add();
        if (GUILayout.Button("-", GUILayout.MaxWidth(20)) && count > 0)
            remove();
        GUILayout.EndHorizontal();
    }
}
