﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _menuRoot;
        [SerializeField] private List<GameObject> _prefabList = new();
        [SerializeField] private RectTransform _clickBlocker;
        
        private Dictionary<MenuType, GameObject> _prefabDict = new();
        
        private Stack<MenuController> _menus = new();

        private void Start()
        {
            _menuRoot.transform.parent = null;
            DontDestroyOnLoad(_menuRoot.gameObject);
            foreach (var prefab in _prefabList)
            {
                var controller = prefab.GetComponent<MenuController>();
                if (controller == null)
                {
                    Debug.LogError("No MenuController on prefab " + prefab.name);
                    continue;
                }
                if (_prefabDict.ContainsKey(controller.GetMenuType()))
                {
                    Debug.LogError($"Duplicate menu prefabs of type {controller.GetMenuType()}");
                    continue;
                }
                _prefabDict[controller.GetMenuType()] = controller.gameObject;
            }
        }

        public void Show(MenuType menuType, MenuController.DialogContext context)
        {
            var menu = Instantiate(_prefabDict[menuType]).GetComponent<MenuController>();
            RectTransform menuRectTransform = menu.GetComponent<RectTransform>();
            
            _clickBlocker.gameObject.SetActive(true);
            _clickBlocker.SetSiblingIndex(1);
            
            menu.transform.SetParent(_menuRoot);
            menuRectTransform.localPosition = new Vector3(0, 0, menuRectTransform.position.z);
             //   new Vector3((_menuRoot.rect.width / 2) - menuRectTransform.rect.width / 4, (_menuRoot.rect.height / 2) - menuRectTransform.rect.height / 4, menu.transform.position.z);
            _menus.Push(menu);
            menu.Setup(context);
            menu.Show();
            
            // temp
            Time.timeScale = 0;
        }

        public void HideTop()
        {
            MenuController menu = _menus.Pop();
            menu.Hide();
            Destroy(menu.gameObject);
            if (_menus.Count == 0)
            {
                _clickBlocker.gameObject.SetActive(false);
                // temp
                Time.timeScale = 1;
            }
            else
            {
                _clickBlocker.gameObject.SetActive(true);
                _clickBlocker.SetSiblingIndex(1);
            }
        }
    }
}