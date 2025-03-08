using System;
using Menus.MenuTypes;
using UnityEngine;

namespace Dialogs
{
    [CreateAssetMenu]
    public class QuipDefinition: ScriptableObject
    {
        public string ID = "";
        public DialogDefinition dialog;
    }
}
