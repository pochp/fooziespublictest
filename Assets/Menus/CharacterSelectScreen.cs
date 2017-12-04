using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Menus
{
    class CharacterSelectScreen : Menu
    {
        public static CharacterSelectScreen GetCharacterSelectScreen()
        {
            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem("Sweep", 0, 0));
            items.Add(new MenuItem("Dash", 1, 0));
            return new CharacterSelectScreen(items);
        }

        private CharacterSelectScreen(List<MenuItem> _items) : base(_items)
        {

        }
    }
}
