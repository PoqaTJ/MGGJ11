using System.Collections;
using Dialogs;
using Player;
using UnityEngine;

namespace Cutscenes
{
    public class GameplayDirector: Director
    {
        [SerializeField] private ConversationDefinition _akariStandsConversation;
        [SerializeField] private ConversationDefinition _tomoyaDepressedConversation;

        [SerializeField] private PlayerMover _akariMover;

        public void StartAkariFoundScene()
        {
            StartCoroutine(AkariFoundScene());
        }

        private IEnumerator AkariFoundScene()
        {
            // turn off Tomoya controls
            
            // this starts after Akari has drunk the tea.
            
            yield return null;
            // akari jumps up
            StartConversation(_akariStandsConversation); //akari is happy to see Tomoya transformed
            
            // akari and tomoya get knocked down
            
            // fade to purple
            
            // move to bottom of stage
            
            // fade back
            
            StartConversation(_tomoyaDepressedConversation);
            
            // Tomoya powers up!
            
            // regain control
        }
    }
}