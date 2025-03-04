using System;
using Menus.MenuTypes;

namespace Dialogs
{
    [Serializable]
    public struct DialogDefinition
    {
        public DialogCharacter Character;
        public string Text;
        public DialogSide Side;
        public SpriteFace Expression;
    }

    public enum DialogCharacter
    {
        Tomoya = 0,
        Akari = 1,
        MagicalTomoya = 3,
        MagicalAkari = 4,
        Butterfly = 2,
    }

    public enum DialogSide
    {
        Left,
        Right
    }
}