using Game.DialogueSystem.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Game.DialogueSystem
{
    public abstract class DialogueHandler : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Events")]
        [SerializeField] private UnityEvent _onDialogueEndedUnityEvent;
        #endregion

        #region Abstract Methods
        protected abstract IDialogueSequence GetDialogueSequence();
        #endregion

        #region Public Methods
        //Called from Unity events
        public void StartDialogue()
        {
            DialogueManager.Instance.OnDialogueEndedEvent += HandleOnDialogueEnded;
            DialogueManager.Instance.StartDialogue(GetDialogueSequence());
        }
        #endregion

        #region Private Methods
        private void HandleOnDialogueEnded(object sender, DialogueEventArgs eventArgs)
        {
            DialogueManager.Instance.OnDialogueEndedEvent -= HandleOnDialogueEnded;
            _onDialogueEndedUnityEvent.Invoke();
        }
        #endregion
    }
}
