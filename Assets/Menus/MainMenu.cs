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

        protected override void HandleMenuResult(MenuResult _result)
        {
            if (_result == MenuResult.Continue)
            {
                Match.SetData set = new Match.SetData();
                if (P1.SelectionState == PlayerInMenu.SelectionStates.Confirmed)
                    if (P1.SelectedItem.ItemName == STR_CHARACTERSELECT)
                        ApplicationStateManager.GetInstance().SetCharacterSelectScreen(set); //go to character select screen
                if (P2.SelectionState == PlayerInMenu.SelectionStates.Confirmed)
                    if (P2.SelectedItem.ItemName == STR_CHARACTERSELECT)
                        ApplicationStateManager.GetInstance().SetCharacterSelectScreen(set); //go to character select screen
            }
        }

        protected override MenuResult EvaluateMenuResult()
        {
            if (P1.SelectionState == PlayerInMenu.SelectionStates.Confirmed ||
                P2.SelectionState == PlayerInMenu.SelectionStates.Confirmed)
                return MenuResult.Continue;
            if (P1.SelectionState == PlayerInMenu.SelectionStates.Cancel ||
                P2.SelectionState == PlayerInMenu.SelectionStates.Cancel)
                return MenuResult.Back;
            return MenuResult.Remain;
        }

        public override string GetDebugInfo()
        {
            return base.GetDebugInfo() + Environment.NewLine + Environment.NewLine  +
                 Environment.NewLine + Environment.NewLine
                 + "Updated versions coming soon." + Environment.NewLine + 
                 "You can reach me on twitter @pochp_";
       
        }
    }
}
