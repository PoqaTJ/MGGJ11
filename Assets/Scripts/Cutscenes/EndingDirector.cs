using System.Collections;
using Dialogs;
using UnityEngine;

namespace Cutscenes
{
    public class EndingDirector: Director
    {


        public void PlayEnding()
        {
            StartCoroutine(EndingCoroutine());
        }

        [SerializeField] private ConversationDefinition _endingConvo1;
        [SerializeField] private ConversationDefinition _endingConvo2;
        [SerializeField] private ConversationDefinition _endingConvo3;
        
        private IEnumerator EndingCoroutine()
        {
            // create portal
            
            // show girls running out
            
            // tomoya de-transforms
            
            // akari excited to take turns
            StartConversation(_endingConvo1);
            
            // prism flies between them
            
            // prism tells them that they can both transform at once now
            StartConversation(_endingConvo2);
            
            // show both girls transforming
            StartConversation(_endingConvo3);
            
            // girls exit left
            
            // fade to credits
            
            yield return null;
        }
    }
}