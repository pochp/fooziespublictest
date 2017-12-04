using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CharacterSelectScreen
{
    public enum ScreenResult { StillSelecting, ReturnMainMenu, StartGame}
    public readonly int NumberOfSelectables;
    public int P1_Selection { get { return P1.Selection; } }
    public int P2_Selection { get { return P2.Selection; } }
    PlayerInCharacterSelectScreen P1;
    PlayerInCharacterSelectScreen P2;

    public CharacterSelectScreen()
    {
        NumberOfSelectables = 5;
        P1 = new PlayerInCharacterSelectScreen(this);
        P2 = new PlayerInCharacterSelectScreen(this);
    }

    public ScreenResult UpdateScreen(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs)
    {
        if(_p1Inputs.B || _p2Inputs.B)
        {
            return ScreenResult.ReturnMainMenu;
        }

        P1.UpdateWithInput(_p1Inputs);
        P2.UpdateWithInput(_p2Inputs);

        if (P1.Confirmed && P2.Confirmed)
            return ScreenResult.StartGame;

        return ScreenResult.StillSelecting;
    }
}

class PlayerInCharacterSelectScreen
{
    public int Selection;
    public bool Confirmed;
    public int TimeSinceLastSelected;
    CharacterSelectScreen Screen;
    private const int FramesOnSameCharacter = 15;

    public PlayerInCharacterSelectScreen(CharacterSelectScreen _screen)
    {
        Selection = 0;
        Confirmed = false;
        TimeSinceLastSelected = 0;
        Screen = _screen;
    }

    public void UpdateWithInput(SinglePlayerInputs _inputs)
    {
        if (_inputs.A)
            Confirmed = true;

        //this part is so that holding left will not increment every frame
        if(_inputs.JoystickDirection == 4 || _inputs.JoystickDirection == 6)
        {
            if (TimeSinceLastSelected <= 0)
            {
                if (_inputs.JoystickDirection == 4)
                    Selection--;
                if (_inputs.JoystickDirection == 6)
                    Selection++;
                TimeSinceLastSelected = FramesOnSameCharacter;
            }
            else
                TimeSinceLastSelected--;
        }
        else
        {
            TimeSinceLastSelected = 0;
        }

    }
}