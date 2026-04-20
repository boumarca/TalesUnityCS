using System;
using System.Collections.Generic;
using Framework.Utils;
using Game.QuestSystem;
using Game.RpgSystem;
using Game.RpgSystem.Models;
using Game.SaveSystem;
using Game.Stats;
using Jrpg.Maps;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Jrpg.Save
{
    public class RpgMetadataProvider : SaveMetadataProvider
    {
        #region SaveMetadataProvider Implementation
        public override IEnumerable<Type> MetadataTypes => new[]
        {
            typeof(RpgSaveMetadata),
            typeof(TableReference),
            typeof(TableEntryReference),
            typeof(PartyMemberInfo)
        };

        public override SaveMetadataBase CreateMetadata(int slotId)
        {
            LocalizedString mapName = MapStateManager.Instance.CurrentMap.MapName;
            RpgSaveMetadata saveData = new(slotId)
            {
                LastSaveDate = DateTime.UtcNow,
                TotalPlayTimeTicks = GameStatsManager.Instance.TotalGameTime.Ticks,
                CurrentLocationTable = mapName.TableReference.TableCollectionName,
                CurrentLocationKey = mapName.TableEntryReference.KeyId,
                CurrentChapter = QuestManager.Instance.CurrentChapter,
                IsClearData = false, //TODO: Handle clear data
                ActivePartyInfos = GetActivePartyInfo(),
                Screenshot = GetScreenshotAsBase64()
            };
            return saveData;
        }
        #endregion

        #region Private Methods
        private List<PartyMemberInfo> GetActivePartyInfo()
        {
            IReadOnlyList<RpgActor> activeParty = PartyManager.Instance.CurrentParty.ActiveMembers;
            List<PartyMemberInfo> partyInfo = new();
            foreach (RpgActor actor in activeParty)
            {
                PartyMemberInfo info = new ()
                {
                    ActorId = actor.Id.Id,
                    Level = actor.Level
                };
                partyInfo.Add(info);
            }
            return partyInfo;
        }

        private string GetScreenshotAsBase64()
        {
            byte[] bytes = ScreenshotCapture.CaptureToBytes("MapCamera", 82, 46); //TODO: Move to constants
            return Convert.ToBase64String(bytes);
        }
        #endregion
    }
}
