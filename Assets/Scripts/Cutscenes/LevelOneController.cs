using System.Collections;
using Cinemachine;
using Dialogs;
using Player;
using Services;
using UnityEngine;

namespace Cutscenes
{
    public class LevelOneController: Director
    {
        [SerializeField] private PlayerInputController _tomoyaNormalInput;
        [SerializeField] private PlayerInputController _tomoyaMagicalInput;
        [SerializeField] private Animator _tomoyaNormalAnimator;
        [SerializeField] private Animator _tomoyaMagicalAnimator;

        [SerializeField] private ConversationDefinition _convinceConversation1;
        [SerializeField] private ConversationDefinition _convinceConversation2;

        [SerializeField] private Transform _transformationLocation;
        
        [SerializeField] private ButterflyController _butterflyController;
        [SerializeField] private Transform _butterflyLocation1;
        [SerializeField] private Transform _butterflyLocation2;

        [SerializeField] private QuipDefinition _afterTF1;
        
        [SerializeField] private ParticleSystem _tomoyaParticleEmitter;

        private static readonly int StartTransform = Animator.StringToHash("TransformStart");
        private static readonly int StopTransform = Animator.StringToHash("TransformEnd");

        private void Start()
        {
            ServiceLocator.Instance.GameManager.OnPlayerSpawn += (controller) =>
            {
                _tomoyaNormalAnimator = controller.GetComponent<Animator>();
                _tomoyaNormalInput = controller.GetComponent<PlayerInputController>();
                ServiceLocator.Instance.GameManager.FocusCameraOn(_tomoyaNormalAnimator.gameObject.transform);
            };
        }

        public void StartTransformationSequence()
        {
            StartCoroutine(TransformationRoutine());
        }
        
        private IEnumerator TransformationRoutine()
        {
            float cameraSetting = _virtualCamera.m_Lens.OrthographicSize;
            yield return ZoomCamera(cameraSetting, 3f, 1f);
            
            // stop tomoya input
            _tomoyaNormalInput.enabled = false;
            PlayerMover tomoyaMover = _tomoyaNormalInput.gameObject.GetComponent<PlayerMover>();
            tomoyaMover.enabled = true;

            yield return MoveCharacterTo(tomoyaMover, _transformationLocation);
            yield return null;
            tomoyaMover.Face(PlayerMover.Direction.RIGHT);

            // fly to Tomoya
            bool reached = false;
            _butterflyController.MoveTo(_butterflyLocation1, () => reached = true);

            yield return new WaitUntil(() => reached);
            
            ServiceLocator.Instance.DialogManager.StartConversation(_convinceConversation1, null);
            
            // fly to her right her
            _butterflyLocation2.position = tomoyaMover.transform.position + new Vector3(0.7f, 2, 0);

            reached = false;
            _butterflyController.MoveTo(_butterflyLocation2, () => reached = true);
            yield return new WaitUntil(() => reached);

            // fly above her
            _butterflyLocation2.position= tomoyaMover.transform.position + new Vector3(0f, 2, 0);

            reached = false;
            _butterflyController.MoveTo(_butterflyLocation2, () => reached = true);
            yield return new WaitUntil(() => reached);

            
            // transformation sequence
            _butterflyController.Disappear();

            ServiceLocator.Instance.DialogManager.StartConversation(_convinceConversation2, null);

            yield return new WaitForSeconds(0.2f);
            
            _tomoyaNormalAnimator.SetTrigger(StartTransform);
            _tomoyaMagicalAnimator.SetTrigger(StartTransform);

            yield return new WaitForSeconds(1.5f);
            _tomoyaParticleEmitter.transform.position = _tomoyaNormalInput.transform.position;
            _tomoyaParticleEmitter.Play();
            _tomoyaMagicalInput.transform.position = _tomoyaNormalInput.transform.position;
            _tomoyaNormalInput.gameObject.SetActive(false);
            yield return FadeToColorCoroutine(_tomoyaColor, 0.2f);
            yield return FadeFromColorCoroutine(_tomoyaColor, 0.2f);

            //yield return FlashBlue();
            yield return new WaitForSeconds(1.5f);
            
            _tomoyaMagicalAnimator.SetTrigger(StopTransform);

            ServiceLocator.Instance.GameManager.FocusCameraOn(_tomoyaMagicalAnimator.gameObject.transform);
            yield return new WaitForSeconds(1f);
            
            yield return ZoomCamera(_virtualCamera.m_Lens.OrthographicSize, cameraSetting, 1f);
            _tomoyaMagicalInput.enabled = true;

            yield return new WaitForSeconds(1f);
            ServiceLocator.Instance.DialogManager.ShowQuip(_afterTF1);
        }
    }
}