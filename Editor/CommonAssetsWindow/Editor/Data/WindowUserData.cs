namespace CommonAssetsWindow.Editor.Data
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public class WindowUserData
    {
        #region Properties
        public string Uuid => uuid;
        public int HistoryCapacity => historyCapacity;
        public int MinInteractionsToShow => minInteractionsToShow;
        public bool AllowDirectories => allowDirectories;
        public string UnityId => unityId;
        #endregion

        #region Private Variables
        [SerializeField, ReadOnly, BoxGroup("User")] private string uuid;
        [SerializeField, ReadOnly, BoxGroup("User")]
        private string unityId;
        [SerializeField, BoxGroup("Settings"), LabelWidth(100)] private int historyCapacity = 16;
        [SerializeField, BoxGroup("Settings"), LabelWidth(150)] private int minInteractionsToShow = 4;
        [SerializeField, BoxGroup("Settings"), LabelWidth(100)] private bool allowDirectories;
        #endregion

        #region Public API
        public void Initialize()
        {
            uuid = SystemInfo.deviceUniqueIdentifier;
            unityId = CloudProjectSettings.userId;
        }

        #endregion
    }
}