using System;
using System.Collections;
using Dialogs;
using Menus;
using Menus.MenuTypes;
using Player;
using Services;
using UnityEngine;

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
        
        private static readonly int StartTransform = Animator.StringToHash("TransformStart");
        private static readonly int StopTransform = Animator.StringToHash("TransformEnd");

        private void Start()
        {
            ServiceLocator.Instance.GameManager.OnFoundAkari += AkariFoundDone;
        }

        private void AkariFoundDone()
        {
            _findAkariScene.SetActive(false);
            _bottomAkari.SetActive(true);
            _afterFindAkariQuips.SetActive(true);
            _beforeFindAkariQuips.SetActive(false);
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

            yield return new WaitForSeconds(2f);
            
            StartConversation(_akariOhNoConversation);
            
            // akari and tomoya get knocked down
            yield return new WaitForSeconds(3f);

            
            // fade to purple
            yield return new WaitForSeconds(1f);

            
            // move to bottom of stage
            _bottomAkari.SetActive(true);
            SnapCharacterTo(tomoyaMover, _tomoyaBottomLocation);
            tomoyaMover.Face(PlayerMover.Direction.LEFT);
            
            // fade back
            yield return new WaitForSeconds(2f);
            StartConversation(_tomoyaDepressedConversation);
            
            // Tomoya powers up!
            tomoya.GetComponent<Animator>().SetTrigger(StartTransform);
            yield return new WaitForSeconds(1.5f);
            var context = new PopupMenuOneButton.PopupMenuOneButtonContext();
            context.titleLocString = "dialog-powerup-break-hazards-title";
            context.bodyLocString = "dialog-powerup-break-hazards-body";
            context.buttonLocString = "dialog-powerup-break-hazards-button";
            ServiceLocator.Instance.MenuManager.Show(MenuType.PopupOneButton, context);
            yield return new WaitForSeconds(1f);

            ServiceLocator.Instance.SaveManager.UnlockedBreakHazard = true;
            tomoya.GetComponent<Animator>().SetTrigger(StopTransform);
            
            // enable new quips
            ServiceLocator.Instance.GameManager.FindAkari();
            
            // regain control
            ServiceLocator.Instance.SaveManager.FoundAkari = true;
            tomoyaInputController.enabled = true;
            tomoya.enabled = true;
            tomoyaMover.enabled = false;
        }
    }
}