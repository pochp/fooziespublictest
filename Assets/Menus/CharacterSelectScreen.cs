﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Match;

namespace Assets.Menus
{
    class CharacterSelectScreen : Menu
    {
        public const string STR_SWEEP = "Sweep";
        public const string STR_DASH = "Dash";
        private SetData m_currentSetData;


        public static CharacterSelectScreen GetCharacterSelectScreen(SetData _currentSetData)
        {
            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem(STR_SWEEP, 0, 0));
            items.Add(new MenuItem(STR_DASH, 1, 0));
            return new CharacterSelectScreen(items, _currentSetData);
        }

        protected override void HandleMenuResult(MenuResult _result)
        {
            if (_result == MenuResult.Continue)
            {
                MatchInitializationData matchInitData = new MatchInitializationData();
                matchInitData.P1_Character = DetermineSelectedCharacter(P1);
                matchInitData.P2_Character = DetermineSelectedCharacter(P2);
                m_currentSetData.SetMatchInitData(matchInitData);

                ApplicationStateManager.GetInstance().SetGameplayState(m_currentSetData);
            }
            else if (_result == MenuResult.Back)
            {
                ApplicationStateManager.GetInstance().SetMainMenu();//go to main menu
            }
        }

        private CharacterSelectScreen(List<MenuItem> _items, SetData _currentSetData) : base(_items)
        {
            m_currentSetData = _currentSetData;
            P1.SelectedItem = Items.First(o => o.ItemName == GetMatchingCharacterName(m_currentSetData.P1_SelectedCharacter));
            P2.SelectedItem = Items.First(o => o.ItemName == GetMatchingCharacterName(m_currentSetData.P2_SelectedCharacter));
        }

        private string GetMatchingCharacterName(Character _char)
        {
            if (_char is Sweep)
                return STR_SWEEP;
            if (_char is Dash)
                return STR_DASH;
            return string.Empty;
        }

        private Character DetermineSelectedCharacter(PlayerInMenu _player)
        {
            switch(_player.SelectedItem.ItemName)
            {
                case STR_DASH:
                    return new Dash();
                case STR_SWEEP:
                    return new Sweep();
            }
            throw new Exception("No Character Selected");
        }
    }
}