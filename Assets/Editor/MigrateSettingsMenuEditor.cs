using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using SG;
using UnityEditor.SceneManagement;

public class MigrateSettingsMenuEditor : EditorWindow
{
    [MenuItem("Tools/Migrate Settings Menu")]
    public static void Migrate()
    {
        // 1. Open scene Scene_Main_Menu_01.unity
        string scenePath = "Assets/_Project/Scenes/Scene_Main_Menu_01.unity";
        Debug.Log("Opening scene: " + scenePath);
        var scene = EditorSceneManager.OpenScene(scenePath);

        // 2. Find Title Screen Settings Menu in the scene (including inactive objects)
        GameObject settingsMenuObj = null;
        var rootObjects = scene.GetRootGameObjects();
        foreach (var root in rootObjects)
        {
            settingsMenuObj = FindChildRecursive(root.transform, "Title Screen Settings Menu");
            if (settingsMenuObj != null)
                break;
        }

        if (settingsMenuObj == null)
        {
            Debug.LogError("Could not find 'Title Screen Settings Menu' in the scene!");
            return;
        }

        // 3. Make sure it is active, so we can save it properly
        bool wasActive = settingsMenuObj.activeSelf;
        settingsMenuObj.SetActive(true);

        // 4. Save as a prefab
        string prefabPath = "Assets/_Project/Prefabs/UI/Settings Menu.prefab";
        System.IO.Directory.CreateDirectory("Assets/_Project/Prefabs/UI");
        
        GameObject settingsPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(settingsMenuObj, prefabPath, InteractionMode.UserAction);
        if (settingsPrefab == null)
        {
            Debug.LogError("Failed to save settings menu prefab!");
            return;
        }
        settingsMenuObj.SetActive(wasActive);
        Debug.Log("Successfully created prefab: " + prefabPath);

        // 5. Load the Player UI Manager prefab
        string uiManagerPrefabPath = "Assets/_Project/Prefabs/UI/Player UI Manager.prefab";
        Debug.Log("Loading Player UI Manager contents: " + uiManagerPrefabPath);
        GameObject uiManagerRoot = PrefabUtility.LoadPrefabContents(uiManagerPrefabPath);
        if (uiManagerRoot == null)
        {
            Debug.LogError("Failed to load Player UI Manager prefab!");
            return;
        }

        // 6. Check if Settings Menu already exists inside Player UI Manager, delete if so
        Transform oldSettings = uiManagerRoot.transform.Find("Settings Menu");
        if (oldSettings != null)
        {
            Debug.Log("Found existing 'Settings Menu' child, deleting it first...");
            DestroyImmediate(oldSettings.gameObject);
        }

        // 7. Instantiate the Settings Menu prefab as a child of the Canvas in Player UI Manager
        Canvas canvas = uiManagerRoot.GetComponentInChildren<Canvas>(true);
        if (canvas == null)
        {
            Debug.LogError("Could not find Canvas inside Player UI Manager!");
            PrefabUtility.UnloadPrefabContents(uiManagerRoot);
            return;
        }

        GameObject settingsMenuInstance = PrefabUtility.InstantiatePrefab(settingsPrefab, canvas.transform) as GameObject;
        if (settingsMenuInstance == null)
        {
            Debug.LogError("Failed to instantiate settings menu prefab inside Player UI Manager canvas!");
            PrefabUtility.UnloadPrefabContents(uiManagerRoot);
            return;
        }
        settingsMenuInstance.name = "Settings Menu";
        settingsMenuInstance.SetActive(false);

        // 8. Bind the variables inside PlayerUIManager
        PlayerUIManager uiManagerScript = uiManagerRoot.GetComponent<PlayerUIManager>();
        if (uiManagerScript != null)
        {
            uiManagerScript.settingsMenu = settingsMenuInstance;

            // Find BGM Volume Slider
            Slider[] sliders = settingsMenuInstance.GetComponentsInChildren<Slider>(true);
            foreach (var s in sliders)
            {
                if (s.gameObject.name.Contains("BGM") || s.gameObject.name.ToLower().Contains("bgm"))
                {
                    uiManagerScript.bgmVolumeSlider = s;
                    Debug.Log("Assigned BGM Slider reference: " + s.gameObject.name);
                }
                else if (s.gameObject.name.Contains("SFX") || s.gameObject.name.ToLower().Contains("sfx"))
                {
                    uiManagerScript.sfxVolumeSlider = s;
                    Debug.Log("Assigned SFX Slider reference: " + s.gameObject.name);
                }
            }

            // Find Back Button
            Button[] buttons = settingsMenuInstance.GetComponentsInChildren<Button>(true);
            foreach (var b in buttons)
            {
                if (b.gameObject.name.Contains("Back") || b.gameObject.name.Contains("Return"))
                {
                    uiManagerScript.settingsReturnButton = b;
                    Debug.Log("Assigned Settings Return Button reference: " + b.gameObject.name);
                }
            }

            EditorUtility.SetDirty(uiManagerScript);
        }
        else
        {
            Debug.LogError("Could not find PlayerUIManager script on prefab root!");
        }

        // 9. Save Player UI Manager prefab back
        PrefabUtility.SaveAsPrefabAsset(uiManagerRoot, uiManagerPrefabPath);
        PrefabUtility.UnloadPrefabContents(uiManagerRoot);
        Debug.Log("Successfully updated Player UI Manager prefab!");

        // 10. Update TitleScreenManager in the scene to point to the new prefab instance
#if UNITY_2023_1_OR_NEWER
        TitleScreenManager titleScreenManager = Object.FindAnyObjectByType<TitleScreenManager>();
        PlayerUIManager sceneUIManager = Object.FindAnyObjectByType<PlayerUIManager>();
#else
        TitleScreenManager titleScreenManager = Object.FindObjectOfType<TitleScreenManager>();
        PlayerUIManager sceneUIManager = Object.FindObjectOfType<PlayerUIManager>();
#endif

        if (titleScreenManager != null)
        {
            if (sceneUIManager != null && sceneUIManager.settingsMenu != null)
            {
                SerializedObject so = new SerializedObject(titleScreenManager);
                so.FindProperty("titleScreenSettingsMenu").objectReferenceValue = sceneUIManager.settingsMenu;
                so.FindProperty("bgmVolumeSlider").objectReferenceValue = sceneUIManager.bgmVolumeSlider;
                so.FindProperty("sfxVolumeSlider").objectReferenceValue = sceneUIManager.sfxVolumeSlider;
                so.FindProperty("settingsReturnButton").objectReferenceValue = sceneUIManager.settingsReturnButton;
                so.ApplyModifiedProperties();
                
                Debug.Log("Successfully updated TitleScreenManager references in Scene.");
                
                // Now we can deactivate or delete the old scene-based Settings Menu!
                Debug.Log("Deleting old scene-based Settings Menu GameObject...");
                DestroyImmediate(settingsMenuObj);

                // Save scene
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log("Scene saved successfully!");
            }
            else
            {
                Debug.LogError("Could not find PlayerUIManager instance or settingsMenu in the Scene!");
            }
        }
        else
        {
            Debug.LogError("Could not find TitleScreenManager in the Scene!");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("MIGRATION COMPLETED SUCCESSFULLY!");
    }

    private static GameObject FindChildRecursive(Transform parent, string name)
    {
        if (parent.gameObject.name == name)
            return parent.gameObject;

        for (int i = 0; i < parent.childCount; i++)
        {
            var result = FindChildRecursive(parent.GetChild(i), name);
            if (result != null)
                return result;
        }
        return null;
    }
}
