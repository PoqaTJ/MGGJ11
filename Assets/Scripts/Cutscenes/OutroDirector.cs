using System;
using System.Collections;
using Dialogs;
using Player;
using UnityEngine;

namespace Cutscenes
{
    public class OutroDirector: Director
    {
        [SerializeField] private PlayerMover _tomoyaMover;
        [SerializeField] private PlayerMover _tomoyaMagicalMover;
        [SerializeField] private PlayerMover _akariMover;
        [SerializeField] private PlayerMover _akariMagicalaMover;
        [SerializeField] private ButterflyController _butterflyMover;
        [SerializeField] private Animator _portalAnimator;

        [SerializeField] private Transform _markOneAkari;
        [SerializeField] private Transform _markOneTomoya;
        [SerializeField] private Transform _markOnePrism;

        [SerializeField] private ConversationDefinition _conversationTakeTurns;
        [SerializeField] private ConversationDefinition _conversationPrism;
        [SerializeField] private ConversationDefinition _conversationFinal;
        
        private void Start()
        {
            StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            yield return FadeFromColorCoroutine(Color.black, 2f);


            bool akariReached1 = false;
            bool tomoyaReached1 = false;

            _akariMover.MoveTo(_markOneAkari, () =>
            {
                _akariMover.Face(PlayerMover.Direction.RIGHT);
                akariReached1 = true;
            });
            _tomoyaMagicalMover.MoveTo(_markOneTomoya, () =>
            {
                _tomoyaMagicalMover.Face(PlayerMover.Direction.LEFT);
                tomoyaReached1 = true;
            });

            yield return new WaitUntil(()=> akariReached1 && tomoyaReached1);

            _portalAnimator.SetTrigger(Close);

            yield return new WaitForSeconds(2f);
            
            StartConversation(_conversationTakeTurns);
            
            
        }
    }
}