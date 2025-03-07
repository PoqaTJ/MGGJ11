using System.Collections;
using Dialogs;
using Game;
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
        [SerializeField] private GameObject _rotatingHazard;
        
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

            yield return new WaitForSeconds(2f);
            
            StartConversation(_akariOhNoConversation);

            _rotatingHazard.SetActive(true);
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
            yield return FadeToColorCoroutine(Color.black, 1f);
            
            // move Tomoya next to Akari. move camera
            tomoya.RestoreCollisions();
            SnapCharacterTo(tomoya.gameObject, _tomoyaEndingLoc1);
            _virtualCamera.Follow = tomoya.gameObject.transform;

            // fade back in
            yield return FadeFromColorCoroutine(Color.black, 1f);

            StartConversation(_endingConversation2);
            
            // Tomoya creates a portal
            _exitPortal.gameObject.SetActive(true);

            // both leave through the portal
            tomoyaMover.MoveTo(_exitPortal.transform, () => tomoyaMover.gameObject.SetActive(false));
            _akariNormalMover.MoveTo(_exitPortal.transform, () => _akariNormalMover.gameObject.SetActive(false));
            
            yield return FadeToColorCoroutine(Color.black, 2f);

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