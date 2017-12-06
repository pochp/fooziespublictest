using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Menus;

public class ApplicationStateManager
{
    private ApplicationState m_currentApplicationState;
    private GameplayRenderer m_gameplayRenderer;
    static public ApplicationStateManager GetInstance()
    {
        if (m_instance == null)
            m_instance = new ApplicationStateManager();
        return m_instance;
    }

    static private ApplicationStateManager m_instance;

    private ApplicationStateManager()
    {
        m_currentApplicationState = MainMenu.CreateMainMenu();
    }

    public void UpdateCurrentState(Inputs _inputs)
    {
        if(m_currentApplicationState == null)
        {
            m_currentApplicationState = MainMenu.CreateMainMenu();
        }
        m_currentApplicationState.Update(_inputs);
    }

    public void InitManager(GameplayRenderer _renderer)
    {
        m_gameplayRenderer = _renderer;
    }

    public void SetMainMenu()
    {
        m_currentApplicationState = MainMenu.CreateMainMenu();
    }

    public void SetCharacterSelectScreen(Match.SetData _setData)
    {
        m_currentApplicationState = CharacterSelectScreen.GetCharacterSelectScreen(_setData);
    }

    public void SetGameplayState(Match.SetData _setData)
    {
        m_currentApplicationState = Gameplay.GameplayState.CreateGameplayState(_setData, m_gameplayRenderer);
    }




    //for debugging
    public static string GetCurrentStateName()
    {
        return GetInstance().m_currentApplicationState.GetDebugInfo();

        ApplicationState ass = GetInstance().m_currentApplicationState;
        if (ass is MainMenu)
            return "MAIN MENU";
        if (ass is CharacterSelectScreen)
            return "CHARACTER SELECT SCREEN";
        if (ass is Gameplay.GameplayState)
            return "GAMEPLAY";
        return "UNDEFINED";
    }
}