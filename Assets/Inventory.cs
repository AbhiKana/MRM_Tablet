using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class Inventory : MonoBehaviour
{
    public ShowMarbleDetails[] data_1;
    public ShowMarbleDetails[] data_2;
    public Dictionary<ShowMarbleDetails, ShowMarbleDetails> shapesObjects;
    private void Start()
    {
        //InstantiateUIElementNew();
    }
    public void InstantiateUIElementNew()
    {
        shapesObjects = new Dictionary<ShowMarbleDetails, ShowMarbleDetails>();
        for (int i = 0; i < data_1.Length; i++)
        {
            shapesObjects.Add(data_1[i], data_2[i]);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Inventory))]
public class InventoryEdtior : Editor
{
    private Dictionary<ShowMarbleDetails, ShowMarbleDetails> inventoryDic = new Dictionary<ShowMarbleDetails, ShowMarbleDetails>();
    bool showBackground;
    List<bool> showElement = new List<bool>();
    public override void OnInspectorGUI()
    {
        Inventory inventory = (Inventory)target;
        GetReferenceImage(inventory);

        EditorGUILayout.Space(10);
        GetScriptableArray();

        EditorGUILayout.Space(10);
        GetDictonaryValues(inventory);
    }

    void GetReferenceImage(Inventory inventory)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
    }
    void GetScriptableArray()
    {
        EditorGUILayout.LabelField("Data & Reference Holder");
        var array = new SerializedObject(target);
        var property = array.FindProperty("dataHolder");

        array.UpdateIfRequiredOrScript();
        EditorGUILayout.PropertyField(property, true);
        array.ApplyModifiedProperties();
    }
    void GetDictonaryValues(Inventory inventory)
    {
        List<DictonaryData> d = new List<DictonaryData>();
        inventoryDic = inventory.shapesObjects;
        if (inventoryDic != null)
        {
            foreach (var item in inventoryDic)
            {
                DictonaryData dd = new DictonaryData();
                dd.showMarbleDetails_1 = item.Key;
                dd.showMarbleDetails_2 = item.Value;
                d.Add(dd);

                bool localBool = false;
                showElement.Add(localBool);
            }

            showBackground = EditorGUILayout.Foldout(showBackground, "Dictonary Elements", true);
            if (showBackground)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < d.Count; i++)
                {
                    Debug.Log(d[i]);
                    showElement[i] = EditorGUILayout.BeginFoldoutHeaderGroup(showElement[i], "Dictonary values: " + i);
                    if (showElement[i])
                    {
                        //d[i].id = EditorGUILayout.ObjectField("Element " + 0, d[i].id, typeof(int), true) as int;
                        d[i].showMarbleDetails_1 = EditorGUILayout.ObjectField("Element " + 0, d[i].showMarbleDetails_1, typeof(ShowMarbleDetails), true) as ShowMarbleDetails;
                        d[i].showMarbleDetails_2 = EditorGUILayout.ObjectField("Element " + 1, d[i].showMarbleDetails_2, typeof(ShowMarbleDetails), true) as ShowMarbleDetails;
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUILayout.LabelField("Dictonary Values");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(null, typeof(Sprite), true);
            EditorGUILayout.ObjectField(null, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif

[System.Serializable]
public class DictonaryData
{
    public ShowMarbleDetails showMarbleDetails_1;
    public ShowMarbleDetails showMarbleDetails_2;
}