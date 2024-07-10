using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FindMissingScripts
{
    private const string MenuRoot = "Custom/Find Missing Scripts/";
    private const string MenuSearchInLoadedScenes = "Search in loaded scenes";
    private const string MenuSearchInBuildSettingsScenes = "Search in build settings scenes";
    private const string MenuSearchInBuildSettingsScenesIncludeDisabled = "Search in build settings scenes (include disabled)";
    private const string MenuSearchInAllScenes = "Search in all scenes";
    private const string MenuSearchInAllPrefabs = "Search in all prefabs";

    private static readonly Regex scriptGuidRegex = new Regex(@"m_Script: \{fileID: [0-9]+, guid: ([0-9a-f]{32}), type: 3\}");

    [MenuItem(MenuRoot + MenuSearchInLoadedScenes)]
    public static void FindMissingScriptsInLoadedScenes()
    {
        BeginSearch(MenuSearchInLoadedScenes);

        for (int i = 0, count = SceneManager.loadedSceneCount; i < count; i++)
        {
            SearchInScene(SceneManager.GetSceneAt(i).path, i, count);
        }

        EndSearch();
    }

    [MenuItem(MenuRoot + MenuSearchInBuildSettingsScenes)]
    public static void FindMissingScriptsInBuildSettingsScenes()
    {
        BeginSearch(MenuSearchInBuildSettingsScenes);

        EditorBuildSettingsScene[] editorBuildSettingsScenes = EditorBuildSettings.scenes;
        for (int i = 0, count = editorBuildSettingsScenes.Length; i < count; i++)
        {
            if (editorBuildSettingsScenes[i].enabled)
            {
                SearchInScene(editorBuildSettingsScenes[i].path, i, count);
            }
        }

        EndSearch();
    }

    [MenuItem(MenuRoot + MenuSearchInBuildSettingsScenesIncludeDisabled)]
    public static void FindMissingScriptsInBuildSettingsScenesIncludeDisable()
    {
        BeginSearch(MenuSearchInBuildSettingsScenesIncludeDisabled);

        EditorBuildSettingsScene[] editorBuildSettingsScenes = EditorBuildSettings.scenes;
        for (int i = 0, count = editorBuildSettingsScenes.Length; i < count; i++)
        {
            SearchInScene(editorBuildSettingsScenes[i].path, i, count);
        }

        EndSearch();
    }

    [MenuItem(MenuRoot + MenuSearchInAllScenes)]
    public static void FindMissingScriptsInProjectAllScenes()
    {
        BeginSearch(MenuSearchInAllScenes);

        string[] paths = AssetDatabase.FindAssets("t:Scene").Select(AssetDatabase.GUIDToAssetPath).ToArray();
        for (int i = 0, count = paths.Length; i < count; i++)
        {
            SearchInScene(paths[i], i, count);
        }

        EndSearch();
    }

    [MenuItem(MenuRoot + MenuSearchInAllPrefabs)]
    public static void FindMissingScriptsInAllPrefabs()
    {
        BeginSearch(MenuSearchInAllPrefabs);

        string[] allScriptGuids = AssetDatabase.FindAssets("t:Script");
        HashSet<string> missingScriptPrefabs = new HashSet<string>();
        foreach (HashSet<string> prefabPaths in FindAllPrefabScriptRefs()
            .Where(kv => !allScriptGuids.Contains(kv.Key))
            .Select(kv => kv.Value))
        {
            foreach (string prefabPath in prefabPaths)
            {
                if (!missingScriptPrefabs.Contains(prefabPath))
                {
                    missingScriptPrefabs.Add(prefabPath);
                }
            }
        }

        int notMissingCount = 0;
        missingScriptPrefabs.ForEachIndex((path, index, count) =>
        {
            EditorUtility.DisplayProgressBar("Check missing script prefabs", path, (float)index / count);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!FindMissingScriptsInGo(prefab, needSceneRemove: false))
            {
                notMissingCount++;
            }
        });

        Debug.Log($"Missing script prefabs count: {missingScriptPrefabs.Count - notMissingCount}");
        EndSearch();
    }

    private static void BeginSearch(string menuItem)
    {
        Debug.Log($"Begin {menuItem.ToLower()}");
    }

    private static void EndSearch()
    {
        EditorUtility.ClearProgressBar();
        Debug.Log("Search end");
    }

    private static void SearchInScene(string sceneAssetPath, int index, int count)
    {
        Scene scene = SceneManager.GetSceneByPath(sceneAssetPath);
        bool needRemove = false;
        if (!scene.IsValid())
        {
            needRemove = true;
            try
            {
                scene = EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Additive);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
        }

        if (!scene.IsValid())
        {
            return;
        }

        GameObject[] gameObjects = scene.GetRootGameObjects();
        string title = $"Check missing scripts ({index}/{count})";
        gameObjects.ForEachIndex((go, goIndex, goCount) =>
        {
            EditorUtility.DisplayProgressBar(title, go.ToString(), (float)goIndex / goCount);
            FindMissingScriptsInGo(go, needRemove);
        });

        if (needRemove)
        {
            EditorSceneManager.CloseScene(scene, removeScene: true);
        }
    }

    private static Dictionary<string, HashSet<string>> FindAllPrefabScriptRefs()
    {
        EditorUtility.DisplayProgressBar("Scanning prefabs", "", 0);

        string[] allPrefabPaths = Directory.GetFiles("Assets/", "*.prefab", SearchOption.AllDirectories);
        Dictionary<string, HashSet<string>> references = new Dictionary<string, HashSet<string>>();

        for (int i = 0, length = allPrefabPaths.Length; i < length; ++i)
        {
            string prefabPath = allPrefabPaths[i];
            EditorUtility.DisplayProgressBar("Scanning prefabs", prefabPath, (float)i / length);
            foreach (string line in File.ReadAllLines(prefabPath))
            {
                string guid = GetScriptGuid(line);
                if (guid == null)
                {
                    continue;
                }

                if (!references.TryGetValue(guid, out HashSet<string> prefabPaths))
                {
                    prefabPaths = new HashSet<string>();
                    references.Add(guid, prefabPaths);
                }

                if (prefabPaths != null && !prefabPaths.Contains(prefabPath))
                {
                    prefabPaths.Add(prefabPath);
                }
            }
        }

        EditorUtility.ClearProgressBar();
        return references;
    }

    private static bool FindMissingScriptsInGo(GameObject go, bool needSceneRemove)
    {
        Transform transform = go.transform;
        bool missing = false;
        foreach (Component component in go.GetComponents<Component>())
        {
            // Missing components will be null, we can't find their type, etc.
            if (component)
            {
                continue;
            }

            missing = true;
            string assetPath = AssetDatabase.GetAssetPath(go);
            if (!string.IsNullOrEmpty(assetPath))
            {
                Debug.Log($"Missing script: {transform.GetTransformPath()}-->{assetPath}", transform.root.gameObject);
                continue;
            }

            if (go.scene.IsValid())
            {
                if (needSceneRemove)
                {
                    Debug.Log($"Missing script: {transform.GetTransformPath()}-->{go.scene.path}", AssetDatabase.LoadAssetAtPath<SceneAsset>(go.scene.path));
                    continue;
                }

                Debug.Log($"Missing script: {transform.GetTransformPath()}", go);
                continue;
            }

            Debug.Log($"Missing script: {transform.GetTransformPath()}", go);
        }

        for (int i = 0, childCount = transform.childCount; i < childCount; i++)
        {
            missing |= FindMissingScriptsInGo(transform.GetChild(i).gameObject, needSceneRemove);
        }

        return missing;
    }

    private static string GetScriptGuid(string line)
    {
        Match match = scriptGuidRegex.Match(line);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        if (line.Contains("m_Script: {fileID: 0}"))
        {
            return "0";
        }

        return null;
    }

    private static string GetTransformPath(this Transform transform)
    {
        StringBuilder sb = new StringBuilder(transform.name);
        while (transform.parent)
        {
            transform = transform.parent;
            sb.Insert(0, '/');
            sb.Insert(0, transform.name);
        }
        return sb.ToString();
    }

    private static void ForEachIndex<T>(this ICollection<T> source, Action<T, int, int> action)
    {
        int index = 0;
        int count = source.Count;
        foreach (T element in source)
        {
            action(element, index, count);
            index += 1;
        }
    }
}
