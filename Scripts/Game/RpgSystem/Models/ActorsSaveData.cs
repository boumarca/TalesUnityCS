using System;
using System.Collections;
using System.Collections.Generic;
using Game.RpgSystem.Models;
using Game.SaveSystem;
using UnityEngine;

namespace Game.RpgSystem.Models
{
    [Serializable]
    public class ActorsSaveData : SaveDataBase
    {
        public ICollection<ActorSaveData> Actors { get; } = new List<ActorSaveData>();
    }
}
