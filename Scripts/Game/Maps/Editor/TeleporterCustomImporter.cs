using System.Collections.Generic;
using System.Linq;
using Framework.Assertions;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Maps.Editor
{
    [AutoCustomTmxImporter()]
    public class TeleporterCustomImporter : CustomTmxImporter
    {
        #region Constants
        private const string TeleporterAssetPath = "Assets/Prefabs/Maps/Teleporter.prefab";
        #endregion

        #region Private Fields
        private TmxAssetImportedArgs _importedArgs;
        #endregion

        #region CustomTmxImporter Implementation
        public override void TmxAssetImported(TmxAssetImportedArgs args)
        {
            _importedArgs = args;
            InstantiateTeleporters();
        }
        #endregion

        #region Private Methods
        private void InstantiateTeleporters()
        {
            Teleporter teleporterPrefab = AssetDatabase.LoadAssetAtPath<Teleporter>(TeleporterAssetPath);
            AssertWrapper.IsNotNull(teleporterPrefab, $"Cannot find Teleporter prefab at path {TeleporterAssetPath}. Please modify the path in the importer script.");

            IEnumerable<SuperObject> teleporters = _importedArgs.ImportedSuperMap.GetComponentsInChildren<SuperObject>().Where(o => o.m_Type == "Teleporter");

            foreach (SuperObject superObject in teleporters)
            {
                InstantiateTeleporter(superObject, teleporterPrefab.gameObject);
                Object.DestroyImmediate(superObject.gameObject);
            }
        }

        private static void InstantiateTeleporter(SuperObject superObject, GameObject prefab)
        {
            GameObject teleporterObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            teleporterObject.name = superObject.name;
            teleporterObject.transform.SetParent(superObject.transform.parent);
            teleporterObject.transform.SetPositionAndRotation(superObject.transform.position, superObject.transform.rotation);

            float ppu = ST2USettings.instance.m_DefaultPixelsPerUnit;
            float width = superObject.m_Width / ppu;
            float height = superObject.m_Height / ppu;
            TriggerZone triggerZone = teleporterObject.GetComponent<TriggerZone>();
            triggerZone.ResizeCollider(new Vector3(width, height, 1));

            Teleporter teleporterComponent = teleporterObject.GetComponent<Teleporter>();
            triggerZone.RegisterPersistentEvent(teleporterComponent.Teleport);

            if (superObject.TryGetComponent(out SuperCustomProperties customProperties))
            {
                foreach (CustomProperty property in customProperties.m_Properties)
                {
                    //null is passed because I assume I don't need the dictionary for these properties.
                    teleporterObject.BroadcastProperty(property, null, error => Debug.LogError(error));
                }
            }
        }
        #endregion
    }
}
