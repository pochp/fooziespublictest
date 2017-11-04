using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CharacterState
{
    public int Position;
    public int StateFrames;
    public GameplayEnums.CharacterState State;
    public List<Hitbox_Gameplay> Hitboxes;
    public int Gauge;

    public CharacterState()
    {
        Position = 0;
        StateFrames = 0;
        State = GameplayEnums.CharacterState.Idle;
        Hitboxes = new List<Hitbox_Gameplay>();
        Gauge = 0;
    }
}