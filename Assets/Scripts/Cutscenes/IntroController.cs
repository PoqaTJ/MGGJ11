using System;
using System.Collections;
using Dialogs;
using Effects;
using Game;
using Menus;
using Menus.MenuTypes;
using Player;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cutscenes
{
    public class IntroController: Director
    {
        [SerializeField] private PlayerMover _akariNormal;
        [SerializeField] private PlayerMover _akariTransformed;
        [SerializeField] private PlayerMover _tomoyaNormal;

        [SerializeField] private Transform _akariPosition1;
        [SerializeField] private Transform _tomoyaPosition1;
        [SerializeField] private Transform _tomoyaPosition2;
        [SerializeField] private Transform _butterflyPosition;

        [SerializeField] private ConversationDefinition _convoAarriveAtPortal;
        [SerializeField] private ConversationDefinition _tomoyaArriveAtPortal;
        [SerializeField] private ConversationDefinition _boredConversation1;
        [SerializeField] private ConversationDefinition _boredConversation2;
        [SerializeField] private ConversationDefinition _portalClosingConversation;

        [SerializeField] private Animator _portalController;
        [SerializeField] private Transform _portalLocation;

        [SerializeField] private ButterflyController _butterflyController;

        [SerializeField] private Animator _akariAnimator;
        [SerializeField] private Animator _akariAnimator2;
        [SerializeField] private ParticleSystem _akariParticleEmitter;
        
        [SerializeField] private Image _blackScreen;

        
        private void Start()
        {
            if (ServiceLocator.Instance == null)
            {
                Debug.Log("Kick start service locator.");
            };
            StartCoroutine(PlayIntro());
        }

        private IEnumerator PlayIntro()
        {
            yield return new WaitForSeconds(0.5f);

            bool akariReached1 = false;
            bool butterflyReached1 = false;

            _akariNormal.MoveTo(_akariPosition1, () =>
            {
                _akariNormal.Face(PlayerMover.Direction.LEFT);
                akariReached1 = true;
            });
            _butterflyController.MoveTo(_butterflyPosition, () =>
            {
                _butterflyController.Face(ButterflyController.Direction.LEFT);
                butterflyReached1 = true;
            });

            yield return new WaitUntil(()=> akariReached1 && butterflyReached1);
            yield return new WaitForSeconds(0.1f);

            ServiceLocator.Instance.DialogManager.StartConversation(_convoAarriveAtPortal, null);

            yield return MoveCharacterTo(_tomoyaNormal, _tomoyaPosition1);
            
            ServiceLocator.Instance.DialogManager.StartConversation(_tomoyaArriveAtPortal, null);

            yield return new WaitForSeconds(0.2f);
            
            _akariNormal.Face(PlayerMover.Direction.RIGHT);
            
            yield return new WaitForSeconds(0.2f);

            _butterflyController.Disappear();

            yield return new WaitForSeconds(0.2f);
            
            _akariAnimator.SetTrigger(StartTransform);
            _akariAnimator2.SetTrigger(StartTransform);

            yield return new WaitForSeconds(1.5f);
            _akariParticleEmitter.transform.SetParent(null);
            _akariParticleEmitter.Play();
            _akariTransformed.transform.position = _akariNormal.transform.position;
            _akariNormal.gameObject.SetActive(false);

            yield return FadeToColorCoroutine(_akariColor, 0.1f);
            yield return FadeFromColorCoroutine(_akariColor, 0.1f);

            yield return new WaitForSeconds(1.5f);
            
            _akariAnimator2.SetTrigger(StopTransform);
            
            yield return new WaitForSeconds(1.5f);
            
            // particles fly to the teleporter location
            yield return FlyParticlesTo(ParticleType.AkariMagic,
                _akariAnimator2.transform, _portalController.transform, 1f);
            
            _portalController.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
           
            yield return MoveCharacterTo(_akariTransformed, _portalLocation);
            _akariTransformed.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            yield return FadeToColorCoroutine(Color.black, 1f);
            yield return new WaitForSeconds(2f);
            
            // IF there's time, show Akari jumping off into the distance!
            
            yield return FadeFromColorCoroutine(Color.black, 1f);

            yield return MoveCharacterTo(_tomoyaNormal, _tomoyaPosition2);
            yield return new WaitForSeconds(0.1f);

            ServiceLocator.Instance.DialogManager.StartConversation(_boredConversation1, null);

            yield return MoveCharacterTo(_tomoyaNormal, _tomoyaPosition1);
            yield return new WaitForSeconds(0.1f);
            
            yield return MoveCharacterTo(_tomoyaNormal, _tomoyaPosition2);
            yield return new WaitForSeconds(0.2f);
            
            _tomoyaNormal.Face(PlayerMover.Direction.RIGHT);
            yield return new WaitForSeconds(0.2f);
            
            ServiceLocator.Instance.DialogManager.StartConversation(_boredConversation2, null);
            
            yield return new WaitForSeconds(0.2f);
            _portalController.SetTrigger(StartClosing);
            yield return new WaitForSeconds(2f);

            ServiceLocator.Instance.DialogManager.StartConversation(_portalClosingConversation, null);

            yield return MoveCharacterTo(_tomoyaNormal, _portalLocation);
            _tomoyaNormal.gameObject.SetActive(false);
            _portalController.SetTrigger(Close);

            yield return new WaitForSeconds(1f);
            yield return FadeToColorCoroutine(Color.black, 1f);

            ServiceLocator.Instance.SaveManager.WatchedIntro = true;
            ServiceLocator.Instance.GameManager.SetState(State.Gameplay);
        }
    }
}