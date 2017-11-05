﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FooziesConstants;

public class CharacterState
{
    public int Position;
    public int StateFrames;
    public GameplayEnums.CharacterState State;
    public List<Hitbox_Gameplay> Hitboxes;
    public int Gauge;
    public bool FacingRight;
    public Character SelectedCharacter;
    public bool P1
    {
        get { return FacingRight; }
    }

    public CharacterState(bool _p1, Character _selectedCharacter)
    {
        Position = 0;
        StateFrames = 0;
        State = GameplayEnums.CharacterState.Idle;
        Hitboxes = new List<Hitbox_Gameplay>();
        Gauge = 0;
        FacingRight = _p1;
        SelectedCharacter = _selectedCharacter;
    }

    public CharacterState(CharacterState _toCopy)
    {
        Position = _toCopy.Position;
        StateFrames = _toCopy.StateFrames;
        State = _toCopy.State;
        Hitboxes = new List<Hitbox_Gameplay>();
        foreach(Hitbox_Gameplay hbox in _toCopy.Hitboxes)
        {
            Hitboxes.Add(new Hitbox_Gameplay(hbox));
        }
        Gauge = _toCopy.Gauge;
        FacingRight = _toCopy.FacingRight;
        SelectedCharacter = _toCopy.SelectedCharacter.CopyCharacter();
    }

    public void SetCharacterState(GameplayEnums.CharacterState _state)
    {
        State = _state;
        StateFrames = 0;
        switch (_state)
        {
            case GameplayEnums.CharacterState.AttackStartup:
                Hitboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hurtbox_Limb, GameplayConstants.HURTBOX_STARTUP));
                SetCharacterHurtboxStanding(Hitboxes);
                break;
            case GameplayEnums.CharacterState.ThrowStartup:
                Hitboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hurtbox_Limb, GameplayConstants.THROW_STARTUP_HURTBOX));
                SetCharacterHurtboxStanding(Hitboxes);
                break;
            case GameplayEnums.CharacterState.WalkBack:
            case GameplayEnums.CharacterState.WalkForward:
            case GameplayEnums.CharacterState.Idle:
                SetCharacterHurtboxStanding(Hitboxes);
                break;
            case GameplayEnums.CharacterState.Crouch:
                SetCharacterHurtboxCrouching(Hitboxes);
                break;
            case GameplayEnums.CharacterState.Special:
                if (SelectedCharacter.CanUseSpecial(this))
                    SelectedCharacter.SetSpecial(this);
                else
                    State = GameplayEnums.CharacterState.Idle;
                break;
        }
    }

    //updates this character's state by one frame
    public MatchOutcome UpdateCharacterState()
    {
        int positionOffset = 0;
        ++StateFrames;
        switch (State)
        {
            case GameplayEnums.CharacterState.AttackActive:
                if (StateFrames > GameplayConstants.ATTACK_ACTIVE)
                {
                    State = GameplayEnums.CharacterState.AttackRecovery;
                    StateFrames = 0;
                    Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack);
                    ModifyLimbHitbox(Hitboxes, GameplayConstants.HURTBOX_WHIFF_EARLY);
                }
                break;
            case GameplayEnums.CharacterState.AttackRecovery:
                if (StateFrames > GameplayConstants.ATTACK_RECOVERY_TOTAL)
                {
                    State = GameplayEnums.CharacterState.Idle;
                    StateFrames = 0;
                    Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                }
                if (StateFrames == GameplayConstants.ATTACK_RECOVERY_SHORTEN)
                {
                    ModifyLimbHitbox(Hitboxes, GameplayConstants.HURTBOX_WHIFF_LATE);
                }
                break;
            case GameplayEnums.CharacterState.AttackStartup:
                if (StateFrames > GameplayConstants.ATTACK_STARTUP)
                {
                    State = GameplayEnums.CharacterState.AttackActive;
                    StateFrames = 0;
                    Hitboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hitbox_Attack, GameplayConstants.HITBOX_ACTIVE));
                    ModifyLimbHitbox(Hitboxes, GameplayConstants.HURTBOX_ACTIVE);
                }
                break;
            case GameplayEnums.CharacterState.BeingThrown:
                SetCharacterHurtboxStanding(Hitboxes);
                if (StateFrames > GameplayConstants.THROW_BREAK_WINDOW)
                {
                    if (P1)
                        return new MatchOutcome(false, true, GameplayEnums.Outcome.Throw);
                    else
                        return new MatchOutcome(true, false, GameplayEnums.Outcome.Throw);
                }
                break;//this is handled in checking if a player wins
            case GameplayEnums.CharacterState.Blockstun:
                SetCharacterHurtboxStanding(Hitboxes);
                if (StateFrames < GameplayConstants.ATTACK_PUSHBACK_DURATION)
                {
                    positionOffset = GameplayConstants.ATTACK_PUSHBACK_SPEED;
                }
                if (StateFrames > GameplayConstants.ATTACK_BLOCKSTUN)
                {
                    State = GameplayEnums.CharacterState.Idle;
                    StateFrames = 0;
                }
                break;
            case GameplayEnums.CharacterState.Clash:
                if (StateFrames > GameplayConstants.CLASH_PUSHBACK_DURATION)
                {
                    State = GameplayEnums.CharacterState.Idle;
                    StateFrames = 0;
                }
                positionOffset = GameplayConstants.CLASH_PUSHBACK_SPEED;
                break;
            case GameplayEnums.CharacterState.Crouch:
                SetCharacterHurtboxCrouching(Hitboxes);
                break;
            case GameplayEnums.CharacterState.Idle:
                SetCharacterHurtboxStanding(Hitboxes);
                break;
            case GameplayEnums.CharacterState.Inactive:
                throw new System.Exception("character state was inactive. not supposed to happen???");
            case GameplayEnums.CharacterState.Special:
                MatchOutcome specialOutcome = SelectedCharacter.UpdateSpecial(this, ref positionOffset);
                if (specialOutcome.IsEnd())
                    return specialOutcome;
                break;
            case GameplayEnums.CharacterState.ThrowActive:
                if (StateFrames > GameplayConstants.THROW_ACTIVE)
                {
                    State = GameplayEnums.CharacterState.ThrowRecovery;
                    StateFrames = 0;
                    Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                    ModifyLimbHitbox(Hitboxes, GameplayConstants.THROW_RECOVERY_HURTBOX);
                }
                break;
            case GameplayEnums.CharacterState.ThrowBreak:
                if (StateFrames > GameplayConstants.BREAK_DURATION)
                {
                    State = GameplayEnums.CharacterState.Idle;
                    StateFrames = 0;
                }
                positionOffset = GameplayConstants.BREAK_PUSHBACK;
                break;
            case GameplayEnums.CharacterState.ThrowingOpponent:
                if (StateFrames > GameplayConstants.THROW_BREAK_WINDOW)
                {
                    if (P1)
                        return new MatchOutcome(true, false, GameplayEnums.Outcome.Throw);
                    else
                        return new MatchOutcome(false, true, GameplayEnums.Outcome.Throw);
                }
                break;//this is handled in checking if a player wins
            case GameplayEnums.CharacterState.ThrowRecovery:
                if (StateFrames > GameplayConstants.THROW_RECOVERY)
                {
                    State = GameplayEnums.CharacterState.Idle;
                    StateFrames = 0;
                    Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                }
                break;
            case GameplayEnums.CharacterState.ThrowStartup:
                SetCharacterHurtboxStanding(Hitboxes);
                if (StateFrames > GameplayConstants.THROW_STARTUP)
                {
                    State = GameplayEnums.CharacterState.ThrowActive;
                    StateFrames = 0;
                    Hitboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hitbox_Throw, GameplayConstants.THROW_ACTIVE_RANGE));
                    ModifyLimbHitbox(Hitboxes, GameplayConstants.THROW_STARTUP_HURTBOX);
                }
                break;
            case GameplayEnums.CharacterState.WalkBack:
                SetCharacterHurtboxStanding(Hitboxes);
                positionOffset = GameplayConstants.WALK_B_SPEED;
                break;
            case GameplayEnums.CharacterState.WalkForward:
                SetCharacterHurtboxStanding(Hitboxes);
                positionOffset = GameplayConstants.WALK_F_SPEED;
                break;
            default:
                throw new System.Exception("Ooops looks like I forgot to handle state : " + State.ToString());
        }

        if(FacingRight)
            Position += positionOffset;
        else
            Position -= positionOffset;


        return new MatchOutcome();
    }
    public Hitbox_Gameplay CreateHitbox(GameplayEnums.HitboxType _boxType, int _width)
    {
        Hitbox_Gameplay box = new Hitbox_Gameplay();
        box.HitboxType = _boxType;
        box.Width = _width;
        if (FacingRight)
            box.Position = GameplayConstants.CHARACTER_HURTBOX_WIDTH / 2 + _width / 2;
        else
            box.Position = GameplayConstants.CHARACTER_HURTBOX_WIDTH / -2 - _width / 2;
        return box;
    }

    public void ModifyLimbHitbox(List<Hitbox_Gameplay> _hitboxes, int _length)
    {
        Hitbox_Gameplay hbox = _hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
        hbox.Width = _length;
        if (FacingRight)
        {
            hbox.Position = GameplayConstants.CHARACTER_HURTBOX_WIDTH / 2 + _length / 2;
        }
        else
        {
            hbox.Position = GameplayConstants.CHARACTER_HURTBOX_WIDTH / 2 - _length / 2;
        }
    }

    public void SetCharacterHurtboxStanding(List<Hitbox_Gameplay> _hitboxes)
    {
        Hitbox_Gameplay hbox = _hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main);
        hbox.Width = GameplayConstants.CHARACTER_HURTBOX_WIDTH;
    }

    public void SetCharacterHurtboxCrouching(List<Hitbox_Gameplay> _hitboxes)
    {
        Hitbox_Gameplay hbox = _hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main);
        hbox.Width = GameplayConstants.CROUCHING_HURTBOX_WIDTH;
    }

    public void AddGauge(int _amount)
    {
        SelectedCharacter.AddGauge(this, _amount);
    }
}