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
    Dictionary<Res, bool> propShow = new Dictionary<Res, bool>();
    List<string> animNames;
    List<string> typeNames;
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
        animNames = typeof(UnitAnim).GetFields().Select(x => (string)x.GetValue(null)).ToList();
        if (buildCostFields == null)
            buildCostFields = typeof(BuildCost).GetFields();
        resList = resList.Distinct().ToList();
        for (int i = 0; i < resList.Count; ++i)
        {
            typeNames.Add(resList[i].GetType().ToString());
            Res res = resList[i];
            if (!propShow.ContainsKey(res))
            {
                propShow.Add(res, true);
            }
        }
        UIHelper.Horizontal(() =>
        {
            UIHelper.Button("++", () => ShowAll(true));
            UIHelper.Button("--", () => ShowAll(false));
        });
        UIHelper.Line();
        for (int i = 0; i < resList.Count; ++i)
        {
            GUILayout.BeginVertical("Box");
            Res res = resList[i];
            Type type = res.GetType();
            ResProp prop = GetResProp(res);
            if (prop.buildCost == null)
                prop.buildCost = new BuildCost();
            BuildCost buildCost = prop.buildCost;
            string name = res.GetType().ToString();
            propShow[res] = EditorGUILayout.Foldout(propShow[res], name);
            if (propShow[res])
            {
                UIHelper.Horizontal(() =>
                {
                    prop.zhTW = EditorGUILayout.TextField("中文", prop.zhTW);
                    prop.interact = EditorGUILayout.FloatField("互動範圍", prop.interact);
                });
                UIHelper.Horizontal(() =>
                {
                    prop.portable = EditorGUILayout.Toggle("可攜帶", prop.portable);
                    prop.buildable = EditorGUILayout.Toggle("可建造", prop.buildable);
                    prop.storable = EditorGUILayout.Toggle("可儲藏", prop.storable);
                });
                if (prop.portable)
                {
                    prop.carryAnim = animNames[EditorGUILayout.Popup("搬運動畫", Mathf.Max(0, animNames.IndexOf(prop.carryAnim)), animNames.ToArray())];
                    prop.placeAnim = animNames[EditorGUILayout.Popup("放置動畫", Mathf.Max(0, animNames.IndexOf(prop.placeAnim)), animNames.ToArray())];
                    prop.pickupAnim = animNames[EditorGUILayout.Popup("拿起動畫.", Mathf.Max(0, animNames.IndexOf(prop.pickupAnim)), animNames.ToArray())];
                }
                if (prop.buildable)
                {
                    //foreach (FieldInfo field in buildCostFields)
                    //{
                    //    field.SetValue(buildCost, EditorGUILayout.IntField(field.Name, (int)field.GetValue(buildCost)));
                    //}
                }
                if (prop.storable)
                {
                    int index = 0;
                    if (prop.storageBy != null)
                        index = EditorGUILayout.Popup("儲藏", Mathf.Max(0, typeNames.IndexOf(prop.storageBy.ToString())), typeNames.ToArray());
                    prop.storageBy = Type.GetType(typeNames[index]);
                }
                else
                {
                    prop.storageBy = null;
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
            propShow[res] = show;
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
}
