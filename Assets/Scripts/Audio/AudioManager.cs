using Menus;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Audio
{
    public class AudioManager: MonoBehaviour
    {
        [SerializeField] AudioSource _bgnAudioSource;
        [SerializeField] AudioClip _mainmenuBGM;
        [SerializeField] AudioClip _introBGM;
        [SerializeField] AudioClip _outroBGM;
        [SerializeField] AudioClip[] _gameplayBGMs;

        [SerializeField] AudioSource _jumpAudioSource;
        [SerializeField] AudioSource _breakHazardAudioSource;
        [SerializeField] AudioSource _collectMcGuffinAudioSource;

        [SerializeField] private AudioClip[] _jumpClips;
        [SerializeField] private AudioClip[] _breakHazardClips;
        [SerializeField] private AudioClip[] _collectClips;

        private string _bgmVolumeKey = "setting_bgm_volume";
        private string _sfxVolumeKey = "setting_sfx_volume";
        private float _defaultVolume = 0.7f;

        private float _bgmVolume;
        private float _sfxVolume;
        public float GetBGMVolume => _bgmVolume;
        public float GetSFXVolume => _sfxVolume;
        
        private int _gameplayBGMIndex = 0;
        private bool _playingGameplay = false;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _gameplayBGMIndex = Random.Range(0, _gameplayBGMs.Length - 1);
            Load();
        }

        private void Load()
        {
            if (!PlayerPrefs.HasKey(_sfxVolumeKey))
            {
                PlayerPrefs.SetFloat(_sfxVolumeKey, _defaultVolume);
            }
            _sfxVolume = PlayerPrefs.GetFloat(_sfxVolumeKey);
            
            if (!PlayerPrefs.HasKey(_bgmVolumeKey))
            {
                PlayerPrefs.SetFloat(_bgmVolumeKey, _defaultVolume);
            }
            _bgmVolume = PlayerPrefs.GetFloat(_bgmVolumeKey);
            
            _bgnAudioSource.volume = _bgmVolume;
            _jumpAudioSource.volume = _sfxVolume;
            _breakHazardAudioSource.volume = _sfxVolume;
            _collectMcGuffinAudioSource.volume = _sfxVolume;
        }
        
        private void Start()
        {
            ChooseSong(SceneManager.GetActiveScene().name);
        }

        private void Update()
        {
            if (_playingGameplay && !_bgnAudioSource.isPlaying)
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
                case "Credits":
                    break; // keep playing outro music
            }
        }

        private void ChooseGameplayBGM()
        {
            _bgnAudioSource.loop = false;
            if (!_playingGameplay)
            {
                _playingGameplay = true;
                _gameplayBGMIndex++;
                if (_gameplayBGMIndex >= _gameplayBGMs.Length)
                {
                    _gameplayBGMIndex = 0;
                }
                Play(_gameplayBGMs[_gameplayBGMIndex], false);
            }
        }
        
        private void PlayMainMenuBGM()
        {
            _playingGameplay = false;
            Play(_mainmenuBGM, true);        }
        
        private void PlayIntroBGM()
        {
            _playingGameplay = false;
            Play(_introBGM, true);
        }
        
        private void PlayOutroBGM()
        {
            _playingGameplay = false;
            Play(_outroBGM, true);
        }

        private void Play(AudioClip audioClip, bool looping)
        {
            _bgnAudioSource.loop = looping;
            _bgnAudioSource.clip = audioClip;
            _bgnAudioSource.Play();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = volume;
            Save();
        }
        
        public void SetBGMVolume(float volume)
        {
            _bgmVolume = volume;
            Save();
        }

        private void Save()
        {
            _bgnAudioSource.volume = _bgmVolume;
            _jumpAudioSource.volume = _sfxVolume;
            _breakHazardAudioSource.volume = _sfxVolume;
            _collectMcGuffinAudioSource.volume = _sfxVolume;
            PlayerPrefs.SetFloat(_bgmVolumeKey, _bgmVolume);
            PlayerPrefs.SetFloat(_sfxVolumeKey, _sfxVolume);
        }

        public void PlayJumpSFX()
        {
            _jumpAudioSource.Stop();
            _jumpAudioSource.clip = _jumpClips[Random.Range(0, _jumpClips.Length - 1)];
            _jumpAudioSource.Play();
        }
        
        public void PlayBreakHazardSFX()
        {
            _jumpAudioSource.Stop();
            _jumpAudioSource.clip = _breakHazardClips[Random.Range(0, _breakHazardClips.Length - 1)];
            _jumpAudioSource.Play();
        }
        
        public void PlayCollectSFX()
        {
            _jumpAudioSource.Stop();
            _jumpAudioSource.clip = _collectClips[Random.Range(0, _collectClips.Length - 1)];
            _jumpAudioSource.Play();
        }
    }
}