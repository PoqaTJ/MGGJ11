using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Audio
{
    public class AudioManager: MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        
        [SerializeField] AudioClip _mainmenuBGM;
        [SerializeField] AudioClip _introBGM;
        [SerializeField] AudioClip _outroBGM;
        [SerializeField] AudioClip[] _gameplayBGMs;

        private int _gameplayBGMIndex = 0;
        private bool _playingGameplay = false;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _gameplayBGMIndex = Random.Range(0, _gameplayBGMs.Length - 1);
        }

        private void Start()
        {
            ChooseSong(SceneManager.GetActiveScene().name);
        }

        private void Update()
        {
            if (_playingGameplay && !_audioSource.isPlaying)
            {
                _playingGameplay = false;
                ChooseGameplayBGM();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ChooseSong(scene.name);
        }
        
        private void ChooseSong(string sceneName)
        {
            switch (sceneName)
            {
                case "LevelOne":
                case "Gameplay":
                    ChooseGameplayBGM();
                    break;
                case "Main":
                    PlayMainMenuBGM();
                    break;
                case "Intro":
                    PlayIntroBGM();
                    break;
                case "Outro":
                    PlayOutroBGM();
                    break;
            }
        }

        private void ChooseGameplayBGM()
        {
            _audioSource.loop = false;
            if (!_playingGameplay)
            {
                _playingGameplay = true;
                _gameplayBGMIndex++;
                if (_gameplayBGMIndex >= _gameplayBGMs.Length)
                {
                    _gameplayBGMIndex = 0;
                }
                _audioSource.clip = _gameplayBGMs[_gameplayBGMIndex];
                _audioSource.Play();
            }
        }
        
        private void PlayMainMenuBGM()
        {
            _audioSource.loop = true;
            _playingGameplay = false;
            _audioSource.clip = _mainmenuBGM;
            _audioSource.Play();
        }
        
        private void PlayIntroBGM()
        {
            _audioSource.loop = true;
            _playingGameplay = false;
            _audioSource.clip = _introBGM;
            _audioSource.Play();
        }
        
        private void PlayOutroBGM()
        {
            _audioSource.loop = true;
            _playingGameplay = false;
            _audioSource.clip = _outroBGM;
            _audioSource.Play();
        }
    }
}