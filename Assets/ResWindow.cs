using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
#if UNITY_EDITOR
public class ResWindow : EditorWindow
{
    Vector2 scrollPos;
    public List<Res> resList = new List<Res>();
    List<ResProp> propList = new List<ResProp>();
    readonly Dictionary<Res, UIProp> uiProps = new Dictionary<Res, UIProp>();
    List<string> animNames;
    List<string> resTypes;
    class UIProp
    {
        public bool showGourp = true;
        public int selectBuildCost;
        public int selectMaterial;
    }

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
        resList = resList.Distinct().ToList();
        resTypes = resList.Select(x => x.GetType().ToString()).ToList();
        propList = resList.Select(x => GetResProp(x)).ToList();
        Res.SetResProp(propList);

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
            ResProp prop = propList[i];
            if (!uiProps.ContainsKey(res))
            {
                uiProps.Add(res, new UIProp());
            }
            if (prop.buildCostMeta == null)
                prop.buildCostMeta = new List<StringInt>();
            List<StringInt> buildCostMate = prop.buildCostMeta;
            if (prop.materialMeta == null)
                prop.materialMeta = new List<StringInt>();
            List<StringInt> materialMeta = prop.materialMeta;
            string name = res.GetType().ToString();
            UIProp uiProp = uiProps[res];
            uiProp.showGourp = EditorGUILayout.Foldout(uiProp.showGourp, name);
            if (uiProp.showGourp)
            {
                UIHelper.Horizontal(() =>
                {
                    prop.zhTW = EditorGUILayout.TextField("中文", prop.zhTW);
                    prop.interact = EditorGUILayout.FloatField("互動範圍", prop.interact);
                    prop.isBuildRes = EditorGUILayout.Toggle("建造材料", prop.isBuildRes);
                }, 60);
                UIHelper.Width(50, 10);
                prop.portable = EditorGUILayout.Toggle("可攜帶", prop.portable);
                UIHelper.Width(0, 0);
                if (prop.portable)
                {
                    UIHelper.Horizontal(() =>
                    {
                        prop.carryAnim = animNames[EditorGUILayout.Popup("搬運動畫", Mathf.Max(0, animNames.IndexOf(prop.carryAnim)), animNames.ToArray())];
                        prop.placeAnim = animNames[EditorGUILayout.Popup("放置動畫", Mathf.Max(0, animNames.IndexOf(prop.placeAnim)), animNames.ToArray())];
                        prop.pickupAnim = animNames[EditorGUILayout.Popup("拿起動畫", Mathf.Max(0, animNames.IndexOf(prop.pickupAnim)), animNames.ToArray())];
                    }, 50, 10);
                }
                UIHelper.Width(50, 10);
                prop.buildable = EditorGUILayout.Toggle("可建造", prop.buildable);
                if (prop.buildable)
                {
                    uiProp.selectBuildCost = BuildCostLayout(buildCostMate, uiProp.selectBuildCost);
                }
                prop.storable = EditorGUILayout.Toggle("可儲藏", prop.storable);
                if (prop.storable)
                {
                    prop.storageByMate = resTypes[EditorGUILayout.Popup("儲藏於", Mathf.Max(0, resTypes.IndexOf(prop.storageByMate)), resTypes.ToArray())];
                }
                prop.isWorkShop = EditorGUILayout.Toggle("加工站", prop.isWorkShop);
                if (prop.isWorkShop)
                {
                    uiProp.selectMaterial = BuildCostLayout(materialMeta, uiProp.selectMaterial);
                }
                UIHelper.Width(0, 0);
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
            uiProps[res].showGourp = show;
        }
    }

    static ResProp GetResProp(Res res)
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

    static int BuildCostLayout(List<StringInt> meta, int currentSelect)
    {
        List<string> buildResName = meta.Select(y => y.key).ToList();
        List<Type> buildRes = BuildCost.AllTypes.Where(x => !buildResName.Contains(x.Name)).ToList();
        int select = 0;
        UIHelper.Horizontal(() =>
        {
            select = EditorGUILayout.Popup("材料", currentSelect, buildRes.Select(x => x.Name).ToArray(), GUILayout.MaxWidth(200));
            UIHelper.Button("+", () =>
            {
                if (buildRes.Any())
                    meta.Add(new StringInt(buildRes[currentSelect], 1));
            });
            UIHelper.Button("-", () =>
            {
                if (meta.Any())
                    meta.RemoveAt(meta.Count - 1);
            });
        }, 40, 20);
        EditorGUIUtility.labelWidth = 40;
        EditorGUIUtility.fieldWidth = 20;
        foreach (StringInt stringInt in meta)
        {
            stringInt.value = EditorGUILayout.IntField(stringInt.key, stringInt.value);
        }
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        return select;
    }
}
#endif