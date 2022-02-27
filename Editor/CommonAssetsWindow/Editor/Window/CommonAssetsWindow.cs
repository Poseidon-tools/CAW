namespace CommonAssetsWindow.Editor.Window
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Serialization;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class CommonAssetsWindow : OdinEditorWindow
    {
        [MenuItem("Tools/CommonAssets")]
        private static void OpenWindow()
        {
            GetWindow<CommonAssetsWindow>().Show();
        }

        #region Content

        [SerializeField] private WindowUserData userData = new WindowUserData();
        [OdinSerialize, NonReorderable, InlineEditor(InlineEditorModes.FullEditor),
         BoxGroup("Common Assets"), OnValueChanged(nameof(OnCollectionChangedHandler))]
        private List<Object> commonItems = new List<Object>();
        [Button("Clear")]
        private void ClearMemory()
        {
            commonItems.Clear();
            interactionCounter.Clear();
        }

        #endregion
        
        #region Internal
        private Dictionary<Object, int> interactionCounter = new Dictionary<Object, int>();
        private WindowDataManager windowDataManager;
        #endregion

        #region Selection

        private void OnSelectionChanged()
        {
            var selected = Selection.activeObject;
            if (!CheckIfSelectionAllowed(selected))
            {
                return;
            }
            if (interactionCounter.ContainsKey(selected))
            {
                interactionCounter[selected]++;
            }
            else
            {
                interactionCounter.Add(selected, 1);
            }
            RebuildStack();
        }

        private void RebuildStack()
        {
            var unorderedList = interactionCounter.ToList();

            unorderedList.Sort(OccuranceComparison);
            var index = 0;
            commonItems.Clear();
            foreach (var keyValuePair in unorderedList)
            {
                if (index >= userData.HistoryCapacity || keyValuePair.Value < userData.MinInteractionsToShow)
                {
                    return;
                }
                commonItems.Add(keyValuePair.Key);
                index++;
            }
            windowDataManager.SerializeWindowData(userData, interactionCounter);
        }

        private int OccuranceComparison(KeyValuePair<Object, int> x, KeyValuePair<Object, int> y)
        {
            return x.Value > y.Value ? -1 : x.Value < y.Value ? 1 : 0;
        }

        private bool CheckIfSelectionAllowed(Object selection)
        {
            if (selection == null) return false;
            if (!userData.AllowDirectories)
            {
                return !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(selection));
            }

            return true;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            userData.Initialize();
            Selection.selectionChanged += OnSelectionChanged;
            windowDataManager = new WindowDataManager();
            DeserializeOnInit();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            windowDataManager.SerializeWindowData(userData, interactionCounter);
        }

        private void DeserializeOnInit()
        {
            var dataPack = windowDataManager.DeserializeForUser();
            if (dataPack == null)
            {
                Debug.Log("data pack null");
                RebuildStack();
                return;
            }
            userData = dataPack.UserData;
            commonItems.Clear();
            interactionCounter.Clear();
            foreach (var assetEntryData in dataPack.Data)
            {
                var kvp = AssetEntryData.FromAssetEntryData(assetEntryData);
                interactionCounter.Add(kvp.Key, kvp.Value);
            }
            RebuildStack();
        }
        #endregion

        #region Helpers
        private void OnCollectionChangedHandler()
        {
            var keysArray = interactionCounter.Keys.ToArray();
            foreach (var key in keysArray)
            {
                if (!commonItems.Contains(key) && interactionCounter[key] >= userData.MinInteractionsToShow)
                {
                    interactionCounter.Remove(key);
                }
            }
        }
        #endregion
    }
}