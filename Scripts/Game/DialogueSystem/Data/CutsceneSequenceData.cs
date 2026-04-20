using Framework.Assertions;
using UnityEngine;

namespace Game.DialogueSystem.Data
{
    //TODO: Make an X-node editor
    [CreateAssetMenu(fileName = "Cutscene_0000", menuName = "Game Data/CutsceneSequenceData")]
    public class CutsceneSequenceData : ScriptableObject, IDialogueSequence
    {
        #region Serialized Fields
        [SerializeReference, SubclassSelector] private IDialogueStep[] _steps;
        #endregion

        #region Public Properties
        public int StepCount => _steps.Length;
        #endregion

        #region Public Methods
        //TODO: Maybe use an enumerator?
        public IDialogueStep GetStep(int index)
        {
            AssertWrapper.IsIndexInRange(index, _steps, $"{nameof(index)} should never be out of range of {nameof(_steps)}.");
            return _steps[index];
        }
        #endregion
    }
}
