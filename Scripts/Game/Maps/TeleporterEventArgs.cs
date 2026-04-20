using System;
using Game.Maps.Data;

namespace Game.Maps
{
    public class TeleporterEventArgs : EventArgs
    {
        #region Public Properties
        public DestinationInfo Destination { get; private set; }
        #endregion

        #region Constructor
        public TeleporterEventArgs(DestinationInfo destination)
        {
            Destination = destination;
        }
        #endregion
    }
}
