using System.Collections;
using Dialogs;
using Player;
using Services;
using UnityEngine;

namespace Cutscenes
{
    public class GameplayDirector: Director
    {
        [SerializeField] private ConversationDefinition _akariStandsConversation;
        [SerializeField] private ConversationDefinition _tomoyaDepressedConversation;

        [SerializeField] private PlayerMover _akariNormalMover;

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
            _akariNormalMover.gameObject.GetComponent<PlayerInputController>().enabled = true;
            _akariNormalMover.transform.rotation = new Quaternion();
            _akariNormalMover.Jump();

            yield return new WaitForSeconds(3f);
            StartConversation(_akariStandsConversation); //akari is happy to see Tomoya transformed
            
            // akari and tomoya get knocked down
            
            // fade to purple
            
            // move to bottom of stage
            
            // fade back
            
            StartConversation(_tomoyaDepressedConversation);
            
            // Tomoya powers up!
            
            // enable new quips
            
            // regain control
        }
    }
}