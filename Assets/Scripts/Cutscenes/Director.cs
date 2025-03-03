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

        protected IEnumerator FadeToColorCoroutine(Color color, float time)
        {
            Color startColor = new Color(color.r, color.b, color.g, 0);
            Color endColor = new Color(color.r, color.b, color.g, 1);
            yield return ServiceLocator.Instance.MenuManager.FadeToColorCoroutine(startColor, endColor, time);
        }
        
        protected IEnumerator FadeFromColorCoroutine(Color color, float time)
        {
            Color startColor = new Color(color.r, color.b, color.g, 1);
            Color endColor = new Color(color.r, color.b, color.g, 0);
            yield return ServiceLocator.Instance.MenuManager.FadeToColorCoroutine(startColor, endColor, time);
        }
    }
}