﻿using System;
using System.Collections;
using Dialogs;
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
    public class IntroController: MonoBehaviour
    {
        [SerializeField] private PlayerMover _akariNormal;
        [SerializeField] private PlayerMover _akariTransformed;
        [SerializeField] private PlayerMover _tomoyaNormal;

        [SerializeField] private Transform _akariPosition1;
        [SerializeField] private Transform _tomoyaPosition1;
        [SerializeField] private Transform _tomoyaPosition2;


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
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int StartClosing = Animator.StringToHash("StartClosing");
        
        private static readonly int StartTransform = Animator.StringToHash("TransformStart");
        private static readonly int StopTransform = Animator.StringToHash("TransformEnd");
        
        private void Start()
        {
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
            _butterflyController.MoveTo(_akariPosition1, () =>
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

            yield return FlashRed();
            yield return new WaitForSeconds(1.5f);
            
            _akariAnimator2.SetTrigger(StopTransform);
            
            yield return new WaitForSeconds(1.5f);
            
            _portalController.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
           
            yield return MoveCharacterTo(_akariTransformed, _portalLocation);
            _akariTransformed.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            yield return FadeToBlack();
            yield return new WaitForSeconds(2f);

            yield return FadeFromBlack();

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
            yield return FadeToBlack();

            ServiceLocator.Instance.SaveManager.WatchedIntro = true;
            ServiceLocator.Instance.GameManager.SetState(State.Gameplay);
        }
        
        private IEnumerator MoveCharacterTo(PlayerMover mover, Transform loc)
        {
            bool reached = false;
            mover.MoveTo(loc, () =>
            {
                reached = true;
            });

            yield return new WaitUntil(() => reached);
        }

        private IEnumerator FlashRed()
        {
            Color originalColor = _blackScreen.color;
            _blackScreen.color = new Color(1f, 0.2f, 0.04f, 0f);
            while (_blackScreen.color.a < 1)
            {
                Color color = _blackScreen.color;
                color.a +=.05f;
                _blackScreen.color = color;
                yield return new WaitForEndOfFrame();
            }
            while (_blackScreen.color.a > 0)
            {
                Color color = _blackScreen.color;
                color.a -= .05f;
                _blackScreen.color = color;
                yield return new WaitForEndOfFrame();
            }

            _blackScreen.color = originalColor;
        }
        
        private IEnumerator FadeToBlack()
        {
            while (_blackScreen.color.a < 1)
            {
                Color color = _blackScreen.color;
                color.a += .01f;
                _blackScreen.color = color;
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator FadeFromBlack()
        {
            while (_blackScreen.color.a > 0)
            {
                Color color = _blackScreen.color;
                color.a -= .01f;
                _blackScreen.color = color;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}