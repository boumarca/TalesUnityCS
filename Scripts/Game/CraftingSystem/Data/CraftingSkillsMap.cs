using System.Collections.Generic;
using Game.RpgSystem.Data;
using SerializedTuples;
using SerializedTuples.Runtime;
using UnityEngine;

namespace Game.CraftingSystem.Data
{
    /// <summary>
    /// TODO: Probably a placeholder data structure. Will change when I implement the character progression feature.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCraftingEffectsMap", menuName = "Game Data/CraftingEffectsMap")]
    public class CraftingSkillsMap : ScriptableObject
    {
        #region Serialized Fields
        [SerializedTupleLabels("Actor", "Crafting Skills")]
        [SerializeField] private SerializedTuple<ActorId, List<CraftingSkillData>>[] _craftingSkills;
        #endregion

        #region Private Fields
        private Dictionary<ActorId, List<CraftingSkillData>> _craftingSkillsMap;
        #endregion

        #region Public Methods
        public IReadOnlyCollection<CraftingSkillData> GetActorSkills(ActorId actorId)
        {
            _craftingSkillsMap ??= _craftingSkills.ToDictionary();
            return _craftingSkillsMap[actorId];
        }
        #endregion
    }
}
