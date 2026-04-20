using System.Collections.Generic;
using Game.SaveSystem;

namespace Jrpg.Save
{
    public class RpgSaveMetadata : SaveMetadataBase
    {
        #region Public Properties
        public long TotalPlayTimeTicks { get; set; }
        public string CurrentLocationTable { get; set; }
        public long CurrentLocationKey { get; set; }
        public int CurrentChapter { get; set; }
        public bool IsClearData { get; set; }
        public List<PartyMemberInfo> ActivePartyInfos { get; set; }
        public string Screenshot { get; set; }
        #endregion

        #region Constructors
        public RpgSaveMetadata(int slotId) : base(slotId)
        {
        }
        #endregion
    }
}
