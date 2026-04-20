using System;

namespace Game.SaveSystem
{
    public class SaveMetadataBase
    {
        #region Public Properties
        public int SlotId { get; set; }
        public DateTime LastSaveDate { get; set; }
        #endregion

        #region Constructors
        public SaveMetadataBase(int slotId)
        {
            SlotId = slotId;
            LastSaveDate = DateTime.UtcNow;
        }
        #endregion
    }
}
