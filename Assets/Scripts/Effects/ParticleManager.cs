using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class ParticleManager: MonoBehaviour
    {
        [SerializeField] private GameObject _akariMagicParticles;
        [SerializeField] private GameObject _tomoyaMagicParticles;
        [SerializeField] private GameObject _akariJumpParticles;
        [SerializeField] private GameObject _tomoyaJumpParticles;


        private Dictionary<ParticleType, ParticleSystem> _particlesDict = new();
        
        private void Start()
        {
            _particlesDict[ParticleType.AkariJump] = GameObject.Instantiate(_akariJumpParticles, transform).GetComponent<ParticleSystem>();
            _particlesDict[ParticleType.AkariMagic] = GameObject.Instantiate(_akariMagicParticles, transform).GetComponent<ParticleSystem>();
            _particlesDict[ParticleType.TomoyaJump] = GameObject.Instantiate(_tomoyaJumpParticles, transform).GetComponent<ParticleSystem>();
            _particlesDict[ParticleType.TomoyaMagic] = GameObject.Instantiate(_tomoyaMagicParticles, transform).GetComponent<ParticleSystem>();

            foreach (var particle in _particlesDict.Keys)
            {
                _particlesDict[particle].gameObject.SetActive(false);
            }
        }

        public void FlyParticles(ParticleType particleType, Transform from, Transform to, float time, Action onDone)
        {
            StartCoroutine(FlyCoroutine(particleType, from, to, time, onDone));
        }

        private IEnumerator FlyCoroutine(ParticleType particleType, Transform from, Transform to, float duration, Action onDone)
        {
            ParticleSystem particles = _particlesDict[particleType];

            particles.gameObject.SetActive(true);
            particles.transform.position = from.position;

            particles.Play();

            float startTime = Time.timeSinceLevelLoad;
            float endTime = startTime + duration;
            Debug.Log($"Start time: {startTime}. End time: {endTime}");
            while (Time.timeSinceLevelLoad < endTime)
            {
                particles.transform.position = Vector2.Lerp(from.position, to.position, (Time.timeSinceLevelLoad - startTime) / duration);
                yield return new WaitForEndOfFrame();
            }
            Debug.Log($"Done! Time: {Time.timeSinceLevelLoad}");
            particles.Stop();
            particles.gameObject.SetActive(false);
            onDone?.Invoke();
        }
    }

    public enum ParticleType
    {
        AkariMagic,
        TomoyaMagic,
        AkariJump,
        TomoyaJump
    }
}