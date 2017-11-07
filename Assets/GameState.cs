using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState {

    public int P1_Position
    {
        get
        {
            return P1_CState.Position;
        }
        set
        {
            P1_CState.Position = value;
        }
    }
    public int P2_Position
    {
        get
        {
            return P2_CState.Position;
        }
        set
        {
            P2_CState.Position = value;
        }
    }
    public int P1_StateFrames
    {
        get
        {
            return P1_CState.StateFrames;
        }
        set
        {
            P1_CState.StateFrames = value;
        }
    }
    public int P2_StateFrames
    {
        get
        {
            return P2_CState.StateFrames;
        }
        set
        {
            P2_CState.StateFrames = value;
        }
    }
    public GameplayEnums.CharacterState P1_State
    {
        get
        {
            return P1_CState.State;
        }
        set
        {
            P1_CState.State = value;
        }
    }
    public GameplayEnums.CharacterState P2_State
    {
        get
        {
            return P2_CState.State;
        }
        set
        {
            P2_CState.State = value;
        }
    }
    public List<Hitbox_Gameplay> P1_Hitboxes
    {
        get
        {
            return P1_CState.Hitboxes;
        }
        set
        {
            P1_CState.Hitboxes = value;
        }
    }
    public List<Hitbox_Gameplay> P2_Hitboxes
    {
        get
        {
            return P2_CState.Hitboxes;
        }
        set
        {
            P2_CState.Hitboxes = value;
        }
    }
    public int P1_Gauge
    {
        get
        {
            return P1_CState.Gauge;
        }
        set
        {
            P1_CState.Gauge = value;
        }
    }
    public int P2_Gauge
    {
        get
        {
            return P2_CState.Gauge;
        }
        set
        {
            P2_CState.Gauge = value;
        }
    }
    public int RemainingHitstop;
    public int RemainingTime;

    public CharacterState P1_CState;
    public CharacterState P2_CState;

    public GameState()
    {
        P1_CState = new CharacterState(true, new Sweep());
        P2_CState = new CharacterState(false, new Dash());


        P1_Position = 0;
        P2_Position = 0;
        P1_StateFrames = 0;
        P2_StateFrames = 0;
        P1_State = GameplayEnums.CharacterState.Idle;
        P2_State = GameplayEnums.CharacterState.Idle;
        P1_Hitboxes = new List<Hitbox_Gameplay>();
        P2_Hitboxes = new List<Hitbox_Gameplay>();
        P1_Gauge = 0;
        P2_Gauge = 0;
        RemainingHitstop = 0;
        RemainingTime = 0;
    }

    public GameState(GameState _other)
    {
        P1_CState = new CharacterState(_other.P1_CState);
        P2_CState = new CharacterState(_other.P2_CState);


        //P1_Position = _other.P1_Position;
        //P2_Position = _other.P2_Position;
        //P1_StateFrames = _other.P1_StateFrames;
        //P2_StateFrames = _other.P2_StateFrames;
        //P1_State = _other.P1_State;
        //P2_State = _other.P2_State;
        //P1_Hitboxes = new List<Hitbox_Gameplay>();
        //P2_Hitboxes = new List<Hitbox_Gameplay>();
        //foreach (Hitbox_Gameplay hbg in _other.P1_Hitboxes)
        //{
        //    P1_Hitboxes.Add(new Hitbox_Gameplay(hbg));
        //}
        //foreach (Hitbox_Gameplay hbg in _other.P2_Hitboxes)
        //{
        //    P2_Hitboxes.Add(new Hitbox_Gameplay(hbg));
        //}
        //P1_Gauge = _other.P1_Gauge;
        //P2_Gauge = _other.P2_Gauge;

        RemainingHitstop = _other.RemainingHitstop;
        RemainingTime = _other.RemainingTime;
    }
}



public class MatchState
{
    public int P1_Score;
    //{
    //    get
    //    {
    //        int sum = 0;
    //        foreach(MatchOutcome mo in Outcomes)
    //        {
    //            if (mo.P1_Scores)
    //                ++sum;
    //        }
    //        return sum;
    //    }
    //}
    public int P2_Score;
    //{
    //    get
    //    {
    //        int sum = 0;
    //        foreach (MatchOutcome mo in Outcomes)
    //        {
    //            if (mo.P2_Scores)
    //                ++sum;
    //        }
    //        return sum;
    //    }
    //}
    //public List<GameplayEnums.Outcome> P1_Outcomes;
    //public List<GameplayEnums.Outcome> P2_Outcomes;

    public List<MatchOutcome> Outcomes;
    public bool GameOver;

    public MatchState()
    {
        P1_Score = 0;
        P2_Score = 0;
        //P1_Outcomes = new List<GameplayEnums.Outcome>();
        //P2_Outcomes = new List<GameplayEnums.Outcome>();
        GameOver = false;
        Outcomes = new List<MatchOutcome>();
    }
}

public class MatchOutcome
{
    public bool P1_Scores;
    public bool P2_Scores;
    public GameplayEnums.Outcome Outcome;

    public MatchOutcome()
    {
        P1_Scores = false;
        P2_Scores = false;
        Outcome = GameplayEnums.Outcome.StillGoing;
    }

    public MatchOutcome(bool _p1Scores, bool _p2Scores, GameplayEnums.Outcome _outcome)
    {
        P1_Scores = _p1Scores;
        P2_Scores = _p2Scores;
        Outcome = _outcome;
    }

    public bool IsSame(MatchOutcome _other)
    {
        if (P1_Scores != _other.P1_Scores)
            return false;
        if (P2_Scores != _other.P2_Scores)
            return false;
        if (Outcome != _other.Outcome)
            return false;
        return true;
    }

    public bool IsEnd()
    {
        MatchOutcome notDone = new MatchOutcome();
        return !IsSame(notDone);
    }
}