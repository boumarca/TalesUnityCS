using System.Linq;
using Framework.Common;
using Game.DialogueSystem.Data;
using Game.Maps.Actors;
using Jrpg.Dialogues;
using UnityEngine;

namespace Jrpg.Maps.LocalMap
{
    public class MapNpc : MonoBehaviour, IMapInteractable
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private MapActor _actor;
        [SerializeField] private ConversationHandler _dialogue;
        [SerializeField] private GameObject[] _sceneInteractions;

        [Header("Game Data")]
        [SerializeField] private bool _keepOrientation;
        #endregion

        #region Private Fields
        private Direction _previousDirection;
        #endregion

        #region MonoBehaviour Methods
        private void OnEnable()
        {
            _dialogue.OnDialogueEndedEvent += HandleOnDialogueEnded;
        }

        private void OnDisable()
        {
            _dialogue.OnDialogueEndedEvent -= HandleOnDialogueEnded;
        }
        #endregion

        #region IMapInteractable Implementation
        public void Interact(MapPlayer player)
        {
            if (!_keepOrientation)
                FacePlayer(player);

            if (_sceneInteractions.Any(obj => obj.activeSelf))
                return;

            _dialogue.StartDialogue();
        }

        public void RestoreOrientation()
        {
            if (!_keepOrientation)
                _actor.ChangeOrientation(_previousDirection);
        }
        #endregion

        #region Private Methods
        private void FacePlayer(MapPlayer player)
        {
            _previousDirection = _actor.CurrentDirection;
            _actor.ChangeOrientation(player.CurrentDirection.GetOppositeDirection());
        }

        //TODO: Add as a listener instead of Unity Event in the inspector?
        private void HandleOnDialogueEnded(object sender, DialogueEventArgs eventArgs)
        {
            RestoreOrientation();
        }
        #endregion
    }
}
