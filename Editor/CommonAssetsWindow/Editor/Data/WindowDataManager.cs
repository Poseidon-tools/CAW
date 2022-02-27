namespace CommonAssetsWindow.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class WindowDataManager
    {
        #region Internal
        private string DATA_KEY = "CAW_USER_DATA_{ID}";
        #endregion
        #region Data Wrapper
        [Serializable]
        public class DataWrapper
        {
            public AssetEntryData[] Data;
            public WindowUserData UserData;
            public DataWrapper(AssetEntryData[] data, WindowUserData userData)
            {
                Data = data;
                UserData = userData;
            }
        }
        #endregion
        #region Public API
        public void SerializeWindowData(WindowUserData userData, Dictionary<Object, int> assetsDict)
        {
            var dataArray = assetsDict.Select(kvp => new AssetEntryData(kvp)).ToArray();
            var dataWrapper = new DataWrapper(dataArray, userData);
            var json = JsonUtility.ToJson(dataWrapper);
            if (string.IsNullOrEmpty(CloudProjectSettings.userId) || CloudProjectSettings.userName == "anonymous")
            {
                EditorPrefs.SetString(DATA_KEY.Replace("{ID}", userData.Uuid), json);
                return;
            }
            EditorPrefs.SetString(DATA_KEY.Replace("{ID}", userData.UnityId), json);
        }

        /// <summary>
        /// Deserialize data for user.
        /// </summary>
        /// <param name="userId">Can be device UUID obtained by SystemInfo.deviceUniqueIdentifier or Unity user ID</param>
        /// <returns></returns>
        public DataWrapper DeserializeForUser()
        {
            string json = string.Empty;
            if (string.IsNullOrEmpty(CloudProjectSettings.userId) || CloudProjectSettings.userName == "anonymous")
            {
                json = EditorPrefs.GetString(DATA_KEY.Replace("{ID}", SystemInfo.deviceUniqueIdentifier));
            }
            else
            {
                json = EditorPrefs.GetString(DATA_KEY.Replace("{ID}", CloudProjectSettings.userId));
            }
            var result = JsonUtility.FromJson<DataWrapper>(json);
            return result;
        }
        #endregion
    }
}