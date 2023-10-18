#if UNITY_EDITOR && UNITY_INPUT_SYSTEM_PROJECT_WIDE_ACTIONS

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Editor
{
    internal static class ProjectWideActionsAsset
    {
        private const string kDefaultAssetPath = "Packages/com.unity.inputsystem/InputSystem/Editor/ProjectWideActions/ProjectWideActionsTemplate.json";
        private const string kAssetPath = "ProjectSettings/InputManager.asset";
        private const string kAssetName = InputSystem.kProjectWideActionsAssetName;

#if UNITY_INCLUDE_TESTS
        private static string s_DefaultAssetPath = kDefaultAssetPath;
        private static string s_AssetPath = kAssetPath;

        internal static void SetAssetPaths(string defaultAssetPathOverride, string assetPathOverride)
        {
            s_DefaultAssetPath = defaultAssetPathOverride;
            s_AssetPath = assetPathOverride;
        }

        internal static void Reset()
        {
            s_DefaultAssetPath = kDefaultAssetPath;
            s_AssetPath = kAssetPath;
        }

        internal static string defaultAssetPath => s_DefaultAssetPath;
        internal static string assetPath => s_AssetPath;
#else
        internal static string defaultAssetPath => kDefaultAssetPath;
        internal static string assetPath => kAssetPath;
#endif

        [InitializeOnLoadMethod]
        internal static void InstallProjectWideActions()
        {
            GetOrCreate();
        }

        internal static bool IsProjectWideActionsAsset(Object obj)
        {
            return IsProjectWideActionsAsset(obj as InputActionAsset);
        }

        internal static bool IsProjectWideActionsAsset(InputActionAsset asset)
        {
            if (ReferenceEquals(asset, null))
                return false;
            return kAssetName.Equals(asset.name);
        }

        internal static InputActionAsset GetOrCreate()
        {
            var objects = AssetDatabase.LoadAllAssetsAtPath(s_AssetPath);
            var inputActionsAsset = (InputActionAsset)objects?.FirstOrDefault(IsProjectWideActionsAsset);
            if (ReferenceEquals(inputActionsAsset, null))
                return CreateNewActionAsset();
            return inputActionsAsset;
        }

        private static InputActionAsset CreateNewActionAsset()
        {
            // Read JSON file content representing a serialized version of the InputActionAsset
            var json = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, s_DefaultAssetPath));

            var asset = ScriptableObject.CreateInstance<InputActionAsset>();
            asset.LoadFromJson(json);
            asset.name = kAssetName;
            asset.m_ValidateReferencesInEditMode = true;

            AssetDatabase.AddObjectToAsset(asset, s_AssetPath);

            // Make sure all the elements in the asset have GUIDs and that they are indeed unique.
            var maps = asset.actionMaps;
            foreach (var map in maps)
            {
                // Make sure action map has GUID.
                if (string.IsNullOrEmpty(map.m_Id) || asset.actionMaps.Count(x => x.m_Id == map.m_Id) > 1)
                    map.GenerateId();

                // Make sure all actions have GUIDs.
                foreach (var action in map.actions)
                {
                    var actionId = action.m_Id;
                    if (string.IsNullOrEmpty(actionId) || asset.actionMaps.Sum(m => m.actions.Count(a => a.m_Id == actionId)) > 1)
                        action.GenerateId();
                }

                // Make sure all bindings have GUIDs.
                for (var i = 0; i < map.m_Bindings.LengthSafe(); ++i)
                {
                    var bindingId = map.m_Bindings[i].m_Id;
                    if (string.IsNullOrEmpty(bindingId) || asset.actionMaps.Sum(m => m.bindings.Count(b => b.m_Id == bindingId)) > 1)
                        map.m_Bindings[i].GenerateId();
                }
            }

            // Create sub-asset for each action. This is so that users can select individual input actions from the asset when they're
            // trying to assign to a field that accepts only one action.
            var index = 0;
            var inputActionReferences = new InputActionReference[asset.Count()];
            foreach (var map in maps)
            {
                foreach (var action in map.actions)
                {
                    var actionReference = InputActionReference.Create(action);
                    AssetDatabase.AddObjectToAsset(actionReference, asset);

                    // Keep track of associated references to avoid creating new ones when queried by e.g. pickers
                    // (not a big deal) but also to provide object persistence. If we would not create these we could
                    // instead let all references serialize at user-side, but that would also require us to anyway
                    // keep a registry of them in InputActionAsset in order to validate them.
                    //asset.m_References.Add(actionReference);
                    inputActionReferences[index++] = actionReference;
                }
            }

            asset.m_References = inputActionReferences;

            AssetDatabase.SaveAssets();

            return asset;
        }
    }
}
#endif
