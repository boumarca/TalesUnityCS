using Framework.Inputs;
using Game.DialogueSystem;
using Game.DialogueSystem.Data;
using Game.UI;
using UnityEngine.InputSystem;

namespace Jrpg.Dialogues
{
    public class DialogueInputController : InputController
    {
        #region Input Controller Implementation
        public override void RegisterCallbacks()
        {
            GameInputs InputActions = InputManager.Instance.InputActions;
            InputActions.Interactions.AdvanceText.performed += OnAdvanceTextPerformed;

            DialogueManager.Instance.OnDialogueStartedEvent += HandleOnDialogueStarted;
            DialogueManager.Instance.OnDialogueEndedEvent += HandleOnDialogueEnded;
        }

        public override void UnregisterCallbacks()
        {
            if (InputManager.Instance != null)
            {
                GameInputs InputActions = InputManager.Instance.InputActions;
                InputActions.Interactions.AdvanceText.performed -= OnAdvanceTextPerformed;
            }

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueStartedEvent -= HandleOnDialogueStarted;
                DialogueManager.Instance.OnDialogueEndedEvent -= HandleOnDialogueEnded;
            }
        }

        public override InputActionMap ActionMap => InputManager.Instance.InputActions.Interactions;
        #endregion

        #region Private Methods
        private void HandleOnDialogueStarted(object sender, DialogueEventArgs args)
        {
            EnableInputMaps();
        }

        private void HandleOnDialogueEnded(object sender, DialogueEventArgs args)
        {
            DisableInputMaps();
        }

        private void EnableInputMaps()
        {
            InputManager.Instance.DisableCurrentInputMap();
            InputManager.Instance.EnableInputMap(ActionMap);
        }

        private void DisableInputMaps()
        {
            InputManager.Instance.EnableCurrentInputMap();
            InputManager.Instance.DisableInputMap(ActionMap);
        }
        #endregion

        #region Input Callbacks
        private void OnAdvanceTextPerformed(InputAction.CallbackContext context)
        {
            UIManager.Instance.HideTextbox();
        }
        #endregion
    }
}
