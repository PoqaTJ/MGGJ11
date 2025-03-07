using System;
using System.Collections;
using Dialogs;
using Game;
using Player;
using Services;
using UnityEngine;

namespace Cutscenes
{
    public class OutroDirector: Director
    {
        [SerializeField] private PlayerMover _tomoyaMagicalMover;
        [SerializeField] private PlayerMover _akariMover;
        [SerializeField] private PlayerMover _akariMagicalaMover;
        [SerializeField] private ButterflyController _butterflyMover;
        [SerializeField] private Animator _portalAnimator;
        [SerializeField] private ParticleSystem _akariParticleSystem;

        [SerializeField] private Transform _markOneAkari;
        [SerializeField] private Transform _markOneTomoya;
        [SerializeField] private Transform _markOnePrism;

        [SerializeField] private Transform _finalMark;
        [SerializeField] private Transform _butterflyFinalMark;

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
            SnapCharacterTo(_tomoyaMagicalMover.gameObject, _portalAnimator.transform);

            _tomoyaMagicalMover.MoveTo(_markOneTomoya, () =>
            {
                _tomoyaMagicalMover.Face(PlayerMover.Direction.RIGHT);
                tomoyaReached1 = true;
            });

            yield return new WaitForSeconds(0.5f);

            SnapCharacterTo(_akariMover.gameObject, _portalAnimator.transform);

            _akariMover.MoveTo(_markOneAkari, () =>
            {
                _akariMover.Face(PlayerMover.Direction.LEFT);
                akariReached1 = true;
            });

            yield return new WaitUntil(()=> akariReached1 && tomoyaReached1);

            _portalAnimator.SetTrigger(Close);

            yield return new WaitForSeconds(2f);
            
            StartConversation(_conversationTakeTurns);
            
            _butterflyMover.gameObject.SetActive(true);
            SnapCharacterTo(_butterflyMover.gameObject, _tomoyaMagicalMover.transform);
            _butterflyMover.Appear();

            bool moved = false;
            _butterflyMover.MoveTo(_markOnePrism, () => moved = true);

            yield return new WaitUntil(() => moved == true);
            
            yield return new WaitForSeconds(0.5f);

            _akariMover.GetComponent<Animator>().SetTrigger(StartTransform);
            _akariMagicalaMover.GetComponent<Animator>().SetTrigger(StartTransform);

            yield return new WaitForSeconds(1.5f);
            _akariParticleSystem.transform.SetParent(null);
            _akariParticleSystem.Play();
            _akariMagicalaMover.transform.position = _akariMover.transform.position;
            _akariMover.gameObject.SetActive(false);

            yield return FadeToColorCoroutine(_akariColor, 0.1f);
            yield return FadeFromColorCoroutine(_akariColor, 0.1f);

            yield return new WaitForSeconds(1.5f);
            
            _akariMagicalaMover.GetComponent<Animator>().SetTrigger(StopTransform);
            
            yield return new WaitForSeconds(1.5f);
            
            StartConversation(_conversationPrism);
            
            // transform!
            _tomoyaMagicalMover.MoveTo(_finalMark, null);
            _akariMagicalaMover.MoveTo(_finalMark, null);

            _butterflyMover.MoveTo(_butterflyFinalMark, null);

            yield return new WaitForSeconds(1.5f);
            
            StartConversation(_conversationFinal);
            yield return new WaitForSeconds(1f);
            yield return FadeToColorCoroutine(Color.black, 3f);

            ServiceLocator.Instance.GameManager.SetState(State.Credits);
        }
    }
}