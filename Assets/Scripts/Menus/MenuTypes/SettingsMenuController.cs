using System;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.MenuTypes
{
    public class SettingsMenuController: MenuController
    {
        [SerializeField] private Slider _musicVolumeSlider;

        public override MenuType GetMenuType() => MenuType.SettingsMenu;

        private void Start()
        {
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        protected override void OnShow()
        {
            Redraw();
            base.OnShow();
        }

        private void Redraw()
        {
            _musicVolumeSlider.value = ServiceLocator.Instance.AudioManager.GetVolume;
        }

        private void OnMusicVolumeChanged(float volume)
        {
            ServiceLocator.Instance.AudioManager.SetVolume(volume);
        }
        
        public void OnClickClose()
        {
            ServiceLocator.Instance.MenuManager.HideTop();
        }
    }
}