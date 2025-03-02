using System.Collections;
using Dialogs;
using Player;
using Services;
using UnityEngine;

namespace Cutscenes
{
    public class Director: MonoBehaviour
    {
        protected IEnumerator MoveCharacterTo(PlayerMover mover, Transform loc)
        {
            bool reached = false;
            mover.MoveTo(loc, () =>
            {
                reached = true;
            });

            yield return new WaitUntil(() => reached);
        }

        protected void StartConversation(ConversationDefinition conv)
        {
            ServiceLocator.Instance.DialogManager.StartConversation(conv, null);
        }

        protected void SnapCharacterTo(PlayerMover mover, Transform loc)
        {
            Vector3 newPos = loc.position;
            newPos.z = mover.transform.position.z;
            mover.transform.position = newPos;
        }
    }
}