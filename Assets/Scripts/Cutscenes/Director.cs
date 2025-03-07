using System;
using System.Collections;
using Cinemachine;
using Dialogs;
using Player;
using Services;
using UnityEngine;

namespace Cutscenes
{
    public class Director: MonoBehaviour
    {
        [SerializeField] protected CinemachineVirtualCamera _virtualCamera;
        
        protected static readonly int Close = Animator.StringToHash("Close");
        protected static readonly int StartClosing = Animator.StringToHash("StartClosing");
        
        protected static readonly int StartTransform = Animator.StringToHash("TransformStart");
        protected static readonly int StopTransform = Animator.StringToHash("TransformEnd");
        
        protected Color _akariColor = new Color(1, .227f, .035f); // FF3A09
        protected Color _tomoyaColor = new Color(0.388f, 0.612f, .906f); // 639CE7

        protected IEnumerator ZoomCamera(float from, float to, float durationSeconds)
        {
            float elapsedTime = 0f;
            _virtualCamera.m_Lens.OrthographicSize = from;
        
            // fade
            while (Math.Abs(_virtualCamera.m_Lens.OrthographicSize - to) > CAMERA_ORTHO_TOLERANCE)
            {
                _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(from, to, elapsedTime / durationSeconds);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        private const double CAMERA_ORTHO_TOLERANCE = 0.01;

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

        protected void SnapCharacterTo(GameObject obj, Transform loc)
        {
            Vector3 newPos = loc.position;
            newPos.z = obj.transform.position.z;
            obj.transform.position = newPos;
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