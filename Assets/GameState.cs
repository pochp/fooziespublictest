using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState {

    public int P1_Position;
    public int P2_Position;
    public int P1_StateFrames;
    public int P2_StateFrames;
    public GameplayEnums.CharacterState P1_State;
    public GameplayEnums.CharacterState P2_State;
    public List<Hitbox_Gameplay> P1_Hitboxes;
    public List<Hitbox_Gameplay> P2_Hitboxes;
    public int P1_Gauge;
    public int P2_Gauge;
    public int RemainingHitstop;
    public int RemainingTime;

    public GameState()
    {
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
        P1_Position = _other.P1_Position;
        P2_Position = _other.P2_Position;
        P1_StateFrames = _other.P1_StateFrames;
        P2_StateFrames = _other.P2_StateFrames;
        P1_State = _other.P1_State;
        P2_State = _other.P2_State;
        P1_Hitboxes = new List<Hitbox_Gameplay>();
        P2_Hitboxes = new List<Hitbox_Gameplay>();
        foreach (Hitbox_Gameplay hbg in _other.P1_Hitboxes)
        {
            P1_Hitboxes.Add(new Hitbox_Gameplay(hbg));
        }
        foreach (Hitbox_Gameplay hbg in _other.P2_Hitboxes)
        {
            P2_Hitboxes.Add(new Hitbox_Gameplay(hbg));
        }
        P1_Gauge = _other.P1_Gauge;
        P2_Gauge = _other.P2_Gauge;

        RemainingHitstop = _other.RemainingHitstop;
        RemainingTime = _other.RemainingTime;
    }
}

public class Hitbox_Gameplay
{
    public GameplayEnums.HitboxType HitboxType;
    public int Position;
    public int Width;
    public int RemainingTime;
    public bool HasStruck;
    public readonly long ID;

    static int ID_Counter = 0;

    public Hitbox_Gameplay()
    {
        HitboxType = GameplayEnums.HitboxType.Hitbox_Attack;
        Position = 0;
        Width = 0;
        RemainingTime = -1;
        HasStruck = false;
        ID = ID_Counter++;
    }

    public Hitbox_Gameplay(GameplayEnums.HitboxType _boxType, int _position, int _width, int _remainingTime = -1)
    {
        HitboxType = _boxType;
        Position = _position;
        Width = _width;
        RemainingTime = _remainingTime;
        HasStruck = false;
        ID = ID_Counter++;
    }

    public Hitbox_Gameplay(Hitbox_Gameplay _other)
    {
        HitboxType = _other.HitboxType;
        Position = _other.Position;
        Width = _other.Width;
        RemainingTime = _other.RemainingTime;
        HasStruck = _other.HasStruck;
        ID = ID_Counter++;
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

    public MatchState()
    {
        P1_Score = 0;
        P2_Score = 0;
        //P1_Outcomes = new List<GameplayEnums.Outcome>();
        //P2_Outcomes = new List<GameplayEnums.Outcome>();

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