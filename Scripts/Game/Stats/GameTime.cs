using System;

namespace Game.Stats
{
    public class GameTime
    {
        #region Private Fields
        private long _totalGameTicks;
        private long _lastSaveTimestamp;
        #endregion

        #region Public Properties
        public TimeSpan TotalGameTime
        {
            get
            {
                RefreshTime();
                return new TimeSpan(_totalGameTicks);
            }
        }
        #endregion

        #region Constructors
        public GameTime()
        {
            _lastSaveTimestamp = DateTime.UtcNow.Ticks;
        }

        public GameTime(long ticks) : this() 
        {
            _totalGameTicks = ticks;
        }
        #endregion

        #region Public Methods
        public void PauseTime()
        {
            RefreshTime();
        }

        public void ResumeTime()
        {
            _lastSaveTimestamp = DateTime.UtcNow.Ticks;
        }
        #endregion

        #region Private Methods
        private void RefreshTime()
        {
            long newTimeStamp = DateTime.UtcNow.Ticks;
            long deltaTime = newTimeStamp - _lastSaveTimestamp;
            _lastSaveTimestamp = newTimeStamp;
            _totalGameTicks += deltaTime;
        }
        #endregion
    }
}
