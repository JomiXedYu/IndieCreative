﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using JxUnity.Resources;

public class ResourceBuilder : Editor
{
    [MenuItem("Assets/ResourcePackage/Set Selected Name")]
    [MenuItem("ResourcePackage/Set Selected Name", false, 0)]
    public static void SetSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            SetName(AssetDatabase.GUIDToAssetPath(item));
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ResourcePackage/Set Selected SubNames")]
    [MenuItem("ResourcePackage/Set Selected SubNames", false, 5)]
    public static void SetSelectSubNames()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            SetSubNames(AssetDatabase.GUIDToAssetPath(item));
        }
        AssetDatabase.Refresh();
    }

    internal static void SetName(string rootName)
    {
        ResourceBuilderUtility.SetNameAndRemoveSub(rootName);
    }

    internal static void SetSubNames(string rootName)
    {
        ResourceBuilderUtility.ResetSubNames(rootName);
    }

    [MenuItem("Assets/ResourcePackage/Remove Selected Name")]
    [MenuItem("ResourcePackage/Remove Selected Name", false, 10)]
    public static void RemoveSelectName()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            string rootName = AssetDatabase.GUIDToAssetPath(item);
            ResourceBuilderUtility.RemoveName(rootName);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ResourcePackage/Remove Selected SubNames")]
    [MenuItem("ResourcePackage/Remove Selected SubNames", false, 15)]
    public static void RemoveSelectSubNames()
    {
        foreach (string item in Selection.assetGUIDs)
        {
            string rootFullName = AssetDatabase.GUIDToAssetPath(item);
            if (AssetDatabase.IsValidFolder(rootFullName))
            {
                ResourceBuilderUtility.RemoveAllInSub(rootFullName);
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("ResourcePackage/Remove All Names", false, 100)]
    public static void RemoveAllNames()
    {
        if (!EditorUtility.DisplayDialog("warn", "remove all ab names", "yes", "no"))
        {
            return;
        }

        ResourceBuilderUtility.RemoveAllNames();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("msg", "done", "ok");
    }

    [MenuItem("ResourcePackage/Remove Unused Names", false, 105)]
    public static void RemoveUnusedNames()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
    }

    [MenuItem("ResourcePackage/Check Valid", false, 105)]
    public static void CheckValid()
    {
        //AssetDatabase.IsValidFolder();

        //foreach (var item in AssetDatabase.GetAssetPathsFromAssetBundle("test_folder.pck"))
        //{
        //    Debug.Log(item);

        //}
        Debug.Log
            (
            AssetNameUtility.UnformatBundleName("Resour/abc.pck")
            );
    }

    /// <summary>
    /// 生成映射表
    /// </summary>
    [MenuItem("ResourcePackage/Generate Resource Mapping", false, 200)]
    public static void GenerateResourceMapping()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        StringBuilder sb = new StringBuilder(1024 * 64);

        var abList = ResourceBuilderUtility.GetUsedAssetBundleNames();
        int assetCount = 0;
        foreach (string abName in abList)
        {
            foreach (string assetROOT in AssetDatabase.GetAssetPathsFromAssetBundle(abName))
            {
                ++assetCount;
                sb.Append(AssetNameUtility.ROOTToASSET(assetROOT));
                sb.Append(':');
                sb.Append(Path.GetFileNameWithoutExtension(assetROOT));
                sb.Append(':');
                sb.Append(abName);
                sb.Append('\n');
            }
        }

        if (!Directory.Exists($"Assets/{AssetConfig.ResourceFolderName}"))
        {
            Directory.CreateDirectory($"Assets/{AssetConfig.ResourceFolderName}");
        }

        string fileROOT = $"Assets/{AssetConfig.ResourceFolderName}/{AssetConfig.MapFilename}";
        File.WriteAllText(fileROOT, sb.ToString());

        AssetDatabase.Refresh();
        SetName(fileROOT);
        AssetDatabase.Refresh();
        stopwatch.Stop();
        Debug.Log($"Resource Mapping Generated! count: {assetCount}, ms: {stopwatch.ElapsedMilliseconds}");
    }


    [MenuItem("ResourcePackage/Generate Local ResObjects", false, 205)]
    private static void GenerateResObjects()
    {
        if (AssetManager.AssetLoadMode != AssetLoadMode.Local)
        {
            Debug.Log("LoadMode is not Local");
            return;
        }
        GenerateResourceMapping();

        const string resdir = "Assets/Resources/LocalResPck";

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        foreach (string abName in ResourceBuilderUtility.GetUsedAssetBundleNames())
        {
            AssetLocalMap ser = ScriptableObject.CreateInstance<AssetLocalMap>();
            foreach (string assetROOT in AssetDatabase.GetAssetPathsFromAssetBundle(abName))
            {
                var assetObjects = AssetDatabase.LoadAllAssetsAtPath(assetROOT);
                foreach (UnityEngine.Object item in assetObjects)
                {
                    ser.Add(item.name, item);
                }
            }

            string assetsoPath = resdir + "/" + AssetNameUtility.UnformatBundleName(abName) + ".asset";
            string assetsoDir = Path.GetDirectoryName(assetsoPath);
            if (!Directory.Exists(assetsoDir))
            {
                Directory.CreateDirectory(assetsoDir);
            }

            AssetDatabase.CreateAsset(ser, assetsoPath);
        }

        AssetDatabase.Refresh();

        stopwatch.Stop();

        Debug.Log("Local ResObjects Generated! ms: " + stopwatch.ElapsedMilliseconds.ToString());
    }


    [MenuItem("ResourcePackage/Build ResourcePackage", false, 210)]
    public static void BuildResourcePackage()
    {
        ResourcePackageBuilderWindow.ShowWindow();
    }

}
