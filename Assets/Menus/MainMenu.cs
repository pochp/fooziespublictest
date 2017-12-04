using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Menus
{
    public class MainMenu : Menu
    {
        public const string STR_CHARACTERSELECT = "CharacterSelect";

        private MainMenu(List<MenuItem> _items) : base(_items)
        {

        }

        public static MainMenu CreateMainMenu()
        {
            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem(STR_CHARACTERSELECT, 0, 0));
            MainMenu mm = new MainMenu(items);
            return mm;
        }
    }
}
