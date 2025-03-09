using System;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.MenuTypes
{
    public class SettingsMenuController: MenuController
    {
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        public override MenuType GetMenuType() => MenuType.SettingsMenu;

        private void Start()
        {
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        protected override void OnShow()
        {
            Redraw();
            base.OnShow();
        }

        private void Redraw()
        {
            _musicVolumeSlider.value = ServiceLocator.Instance.AudioManager.GetBGMVolume;
            _sfxVolumeSlider.value = ServiceLocator.Instance.AudioManager.GetSFXVolume;
        }

        private void OnMusicVolumeChanged(float volume)
        {
            ServiceLocator.Instance.AudioManager.SetBGMVolume(volume);
        }
        
        private void OnSFXVolumeChanged(float volume)
        {
            ServiceLocator.Instance.AudioManager.SetSFXVolume(volume);
        }
        
        public void OnClickClose()
        {
            ServiceLocator.Instance.MenuManager.HideTop();
        }
    }
}