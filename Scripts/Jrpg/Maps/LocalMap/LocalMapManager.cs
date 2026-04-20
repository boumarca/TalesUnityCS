using System;
using System.Threading.Tasks;
using Framework.Extensions;
using Game.Cameras;
using Game.Maps;
using Game.Maps.Data;
using Game.Maps.LocalMap;
using Game.QuestSystem;
using Game.QuestSystem.Models;

namespace Jrpg.Maps.LocalMap
{
    public class LocalMapManager : MapManager
    {
        #region Private Fields
        private LocalMapRoot _currentMapRoot;
        private bool _isTeleporting;
        #endregion

        #region MonoBehaviour Methods
        private void OnDisable()
        {
            QuestManager questManager = QuestManager.Instance;
            if (questManager != null)
            {
                questManager.OnQuestStateChangedEvent -= HandleQuestStateChanged;
                questManager.OnQuestStepUpdatedEvent -= HandleQuestStateChanged;
            }
        }
        #endregion

        #region MapManager Implementation
        public override bool AllowSave => false;

        public override async Task InitializeMap(DestinationInfo destinationInfo)
        {
            await base.InitializeMap(destinationInfo);
            _currentMapRoot = CurrentMap as LocalMapRoot;
            if (Camera is LocalMapCamera localMapCamera)
            {
                SubMap map = CurrentMap.CurrentSubMap;
                localMapCamera.ChangeBounds(map.Position, map.Width, map.Height);
            }

            QuestManager.Instance.BroadcastQuestsStatus();
            RefreshMapState();
            QuestManager.Instance.OnQuestStateChangedEvent += HandleQuestStateChanged;
            QuestManager.Instance.OnQuestStepUpdatedEvent += HandleQuestStateChanged;
            RegisterQuestTargetEvents();
        }

        protected override void HandleTeleporterEvent(object sender, TeleporterEventArgs eventArgs)
        {
            if (_isTeleporting)
                return;

            DestinationInfo destinationInfo = eventArgs.Destination;
            if (destinationInfo.MapId != OverworldMapId)
                TeleportPlayerToMap(destinationInfo);
            else
                TeleportPlayerToOverworld(destinationInfo);
        }
        #endregion

        #region Private Methods
        private async Task TeleportAsync(DestinationInfo destinationInfo)
        {
            _isTeleporting = true;
            Player.DisableCollisions();
            PlayerController.DisableInputs();
            await ScreenCamera.Instance.FadeOut();

            await LoadNewScene(destinationInfo.MapId);

            _currentMapRoot = CurrentMap as LocalMapRoot;

            TeleportPlayer(destinationInfo);

            if (Camera is LocalMapCamera localMapCamera)
            {
                SubMap map = _currentMapRoot.CurrentSubMap;
                localMapCamera.ChangeBounds(map.Position, map.Width, map.Height);
            }

            QuestManager.Instance.BroadcastQuestsStatus();
            RefreshMapState();
            RegisterQuestTargetEvents();
            await ScreenCamera.Instance.FadeIn();
            PlayerController.EnableInputs();
            Player.EnableCollisions();
            _isTeleporting = false;
        }

        private void TeleportPlayerToMap(DestinationInfo destinationInfo)
        {
            TeleportAsync(destinationInfo).FireAndForget();
        }

        private void TeleportPlayerToOverworld(DestinationInfo destinationInfo)
        {
            //TODO: Use the LoadMap function.
            MapStateManager.Instance.LoadWorldMap(destinationInfo);
        }

        //TODO: Rework this event to avoid being sent for each quest.
        private void HandleQuestStateChanged(object sender, QuestEventArgs eventArgs)
        {
            RefreshMapState();
        }

        private void RefreshMapState()
        {
            _currentMapRoot.RefreshMapState();
        }

        private void RegisterQuestTargetEvents()
        {
            QuestTarget[] questTargets = _currentMapRoot.GetComponentsInChildren<QuestTarget>(true);
            foreach (QuestTarget target in questTargets)
            {
                target.OnTargetInteractedEvent -= HandleOnTargetInteracted;
                target.OnTargetInteractedEvent += HandleOnTargetInteracted;
            }
        }

        private void HandleOnTargetInteracted(object sender, EventArgs eventArgs)
        {
            QuestManager.Instance.NotifyQuestSteps(sender);
        }
        #endregion
    }
}
