﻿using System.Collections;
using Dialogs;
using Effects;
using Game;
using Menus;
using Menus.MenuTypes;
using Player;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Cutscenes
{
    public class GameplayDirector: Director
    {
        [SerializeField] private ConversationDefinition _akariOhNoConversation;
        [SerializeField] private ConversationDefinition _tomoyaDepressedConversation;

        [SerializeField] private GameObject _bottomAkari;
        
        [SerializeField] private Transform _tomoyaBottomLocation;

        [SerializeField] private PlayerMover _akariNormalMover;
        [SerializeField] private GameObject _findAkariScene;
        [SerializeField] private GameObject _afterFindAkariQuips;        
        [SerializeField] private GameObject _beforeFindAkariQuips;
        [SerializeField] private GameObject _rotatingHazard;
        
        private static readonly int StartTransform = Animator.StringToHash("TransformStart");
        private static readonly int StopTransform = Animator.StringToHash("TransformEnd");

        private void Start()
        {
            ServiceLocator.Instance.GameManager.OnFoundAkari += AkariFoundDone;
        }

        private void AkariFoundDone()
        {
            if (_findAkariScene != null)
            {
                _findAkariScene.SetActive(false);                
            }
            _bottomAkari.SetActive(true);
            _afterFindAkariQuips.SetActive(true);
            _beforeFindAkariQuips.SetActive(false);
            if (_rotatingHazard != null)
            {
                _rotatingHazard.SetActive(true);
            }
        }
        
        public void StartAkariFoundScene()
        {
            StartCoroutine(AkariFoundScene());
        }

        private IEnumerator AkariFoundScene()
        {
            // turn off Tomoya controls
            var tomoya = ServiceLocator.Instance.GameManager.CurrentPlayer;
            var tomoyaMover = tomoya.GetComponent<PlayerMover>();
            var tomoyaInputController = tomoya.GetComponent<PlayerInputController>();
            tomoya.Heal(3);

            tomoya.StopHorizontalMovement();
            tomoyaInputController.enabled = false;
            tomoya.enabled = false;
            tomoyaMover.enabled = true;
            
            // this starts after Akari has drunk the tea.
            
            yield return null;
            // akari jumps up

            _akariNormalMover.enabled = true;
            _akariNormalMover.gameObject.GetComponent<Animator>().enabled = true;
            _akariNormalMover.transform.rotation = new Quaternion();
            _akariNormalMover.Face(PlayerMover.Direction.LEFT);
            _akariNormalMover.Jump();
            
            // fix akari face
            _akariNormalMover.gameObject.transform.Find("torso/head/mouth").GetComponent<SpriteRenderer>().enabled = true;
            _akariNormalMover.gameObject.transform.Find("torso/head/frown").gameObject.SetActive(false);

            _akariNormalMover.gameObject.transform.Find("torso/head/eye-right").GetComponent<SpriteRenderer>().enabled = true;
            _akariNormalMover.gameObject.transform.Find("torso/head/eye-left").GetComponent<SpriteRenderer>().enabled = true;
            
            _akariNormalMover.gameObject.transform.Find("torso/head/eye-right-closed").gameObject.SetActive(false);
            _akariNormalMover.gameObject.transform.Find("torso/head/eye-left-closed").gameObject.SetActive(false);
            yield return new WaitForSeconds(2f);
            
            StartConversation(_akariOhNoConversation);

            _rotatingHazard.SetActive(true);

            tomoyaMover.Face(PlayerMover.Direction.LEFT);
            _akariNormalMover.Face(PlayerMover.Direction.LEFT);
            
            // akari and tomoya get knocked down

            yield return new WaitUntil(() => _akariNormalMover.CurrentHealth < 3);

            
            // remove akari and tomoya colliders
            _akariNormalMover.RemoveCollision();
            tomoyaMover.RemoveCollision();

            yield return new WaitForSeconds(1);

            // fade to purple
            yield return FadeToColorCoroutine(Color.black, 1f);

            _akariNormalMover.ReenableCollision();
            tomoyaMover.ReenableCollision();
            // move to bottom of stage
            _bottomAkari.SetActive(true);
            SnapCharacterTo(tomoyaMover.gameObject, _tomoyaBottomLocation);
            tomoyaMover.Face(PlayerMover.Direction.LEFT);
            
            // fade back
            yield return FadeFromColorCoroutine(Color.black, 1f);
            StartConversation(_tomoyaDepressedConversation);
            
            // Tomoya powers up!
            var akariAnimator = _bottomAkari.GetComponent<Animator>();
            var tomoyaAnimator = tomoya.GetComponent<Animator>();
            tomoyaAnimator.SetTrigger(StartTransform);
            akariAnimator.SetTrigger(StartTransform);
            yield return new WaitForSeconds(0.5f);
            
            yield return FlyParticlesTo(ParticleType.AkariMagic, _bottomAkari.transform.GetChild(0), tomoya.transform.GetChild(0), 1.5f);

            FadeToColorCoroutine(_mixedColor, 0.1f);
            FadeFromColorCoroutine(_mixedColor, 0.1f);
            
            var context = new PopupMenuOneButton.PopupMenuOneButtonContext();
            context.titleLocString = "dialog-powerup-break-hazards-title";
            context.bodyLocString = "dialog-powerup-break-hazards-body";
            context.buttonLocString = "dialog-powerup-break-hazards-button";
            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupOneButton, context);
            yield return new WaitForSeconds(1f);

            ServiceLocator.Instance.SaveManager.UnlockedBreakHazard = true;
            tomoyaAnimator.SetTrigger(StopTransform);
            akariAnimator.SetTrigger(StopTransform);
            
            // enable new quips
            ServiceLocator.Instance.GameManager.FindAkari();
            
            // regain control
            ServiceLocator.Instance.SaveManager.FoundAkari = true;
            tomoyaInputController.enabled = true;
            tomoya.enabled = true;
            tomoyaMover.enabled = false;
            
            _bottomAkari.GetComponent<AkariCheerController>().enabled = true;
        }
        
        public void StartStageEndScene()
        {
            StartCoroutine(StageEndScene());
        }
        
        [SerializeField]
        private ConversationDefinition _endingConversation2;

        [SerializeField] private Transform _tomoyaEndingLoc1;
        [SerializeField] private Animator _exitPortal;
        
        private IEnumerator StageEndScene()
        {
            _akariNormalMover = _bottomAkari.GetComponent<PlayerMover>();
            _bottomAkari.GetComponent<AkariCheerController>().enabled = false;
            
            // start evil realm collapsing effect
            var tomoya = ServiceLocator.Instance.GameManager.CurrentPlayer;
            var tomoyaMover = tomoya.GetComponent<PlayerMover>();
            var tomoyaInputController = tomoya.GetComponent<PlayerInputController>();
            
            tomoya.StopHorizontalMovement();
            tomoyaInputController.enabled = false;
            tomoya.enabled = false;
            tomoyaMover.enabled = true;

            // Tomoya jumps down, camera remains still

            _virtualCamera.Follow = null;
            
            tomoya.RemoveCollisions();

            yield return new WaitForSeconds(0.25f);
            
            // fade to black
            yield return FadeToColorCoroutine(Color.black, 2f);
            
            // move Tomoya next to Akari. move camera
            tomoya.RestoreCollisions();
            SnapCharacterTo(tomoya.gameObject, _tomoyaEndingLoc1);
            _virtualCamera.Follow = tomoya.gameObject.transform;
            tomoyaMover.Face(PlayerMover.Direction.LEFT);
            
            yield return new WaitForSeconds(2f);
            
            // fade back in
            yield return FadeFromColorCoroutine(Color.black, 2f);

            StartConversation(_endingConversation2);
            
            // Tomoya creates a portal
            yield return FlyParticlesTo(ParticleType.TomoyaMagic, tomoya.transform, _exitPortal.transform, 1.5f);
            _exitPortal.gameObject.SetActive(true);

            yield return new WaitForSeconds(1f);
            _akariNormalMover.enabled = true;
            _akariNormalMover.MoveTo(_exitPortal.transform, () => _akariNormalMover.gameObject.SetActive(false));

            yield return new WaitForSeconds(0.5f);
            // both leave through the portal
            tomoyaMover.MoveTo(_exitPortal.transform, () => tomoyaMover.gameObject.SetActive(false));

            yield return FadeToColorCoroutine(Color.black, 1f);

            ServiceLocator.Instance.GameManager.SetState(State.Outro);
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Save/TriggerEnding")]
        private static void TriggerEnding()
        {
            FindObjectOfType<GameplayDirector>().StartStageEndScene();
        }
#endif
    }
}