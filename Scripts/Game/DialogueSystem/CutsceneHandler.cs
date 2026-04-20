using Game.DialogueSystem.Data;
using UnityEngine;

namespace Game.DialogueSystem
{
    public class CutsceneHandler : DialogueHandler
    {
        #region Serialized Fields
        [Header("Game Data")]
        [SerializeField] private CutsceneSequenceData _cutscene;
        #endregion

        #region Override Methods

        protected override IDialogueSequence GetDialogueSequence() => _cutscene;
        #endregion
    }
}
