namespace CommonAssetsWindow.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;

    [Serializable]
    public class AssetEntryData
    {
        public string ObjectGUID;
        public int UsageCounter;

        public AssetEntryData()
        {
        }

        public AssetEntryData(KeyValuePair<UnityEngine.Object, int> keyValuePair)
        {
            ObjectGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(keyValuePair.Key));
            UsageCounter = keyValuePair.Value;
        }

        public static KeyValuePair<UnityEngine.Object, int> FromAssetEntryData(AssetEntryData assetEntryData)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetEntryData.ObjectGUID);
            var unityObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                
            return new KeyValuePair<UnityEngine.Object, int>(unityObject, assetEntryData.UsageCounter);
        }
    }
}