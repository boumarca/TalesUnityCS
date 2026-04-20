using Game.RpgSystem.Data;
using UnityEngine;
using UnityEngine.Localization;

namespace Game.CraftingSystem.Data
{
    [CreateAssetMenu(fileName = "NewCraftingSkillData", menuName = "Game Data/Crafting/CraftingSkillData")]
    public class CraftingSkillData : ScriptableObject
    {
        #region Serialized Properties
        [field: SerializeField] public CraftingSkillType SkillType { get; private set; }
        [field: SerializeField] public LocalizedString Name { get; private set; }
        [field: SerializeField] public LocalizedString ProcMessage { get; private set; }
        [field: SerializeField] public float ProcChance { get; private set; }
        [field: SerializeField] public int EffectValue { get; private set; }
        [field: SerializeField] public ItemEffectData AddedEffect { get; private set; }
        #endregion

        #region Public Methods
        public bool Proc()
        {
            bool hasProc = Random.value < ProcChance;
            Debug.Log($"Skill {Name.GetLocalizedString()} has {(hasProc ? "" : "not")} proc");
            return hasProc;
        }
        #endregion
    }
}
