using System;
using Framework.Assertions;
using Framework.Extensions;
using UnityEngine;

namespace Game.DialogueSystem.Data
{
    [Serializable]
    public class ConversationSequence : IDialogueSequence
    {
        #region Serialized Fields
        [SerializeReference, SubclassSelector] private IDialogueStep[] _steps;
        #endregion

        #region Override Methods
        public int StepCount => _steps.Length;

        public IDialogueStep GetStep(int index)
        {
            AssertWrapper.IsIndexInRange(index, _steps, $"{nameof(index)} should never be out of range of {nameof(_steps)}.");
            return _steps[index];
        }
        #endregion
    }
}
