﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using FooziesConstants;

namespace Gameplay
{


    /// <summary>
    /// TO DO :
    /// ok-keep working on finishing up hitbox detection
    /// ok-make sure throw breaks remove hurtboxes
    /// -renderer
    /// -scoring
    /// -main menu, declare winner
    /// -gauge, special
    /// </summary>

    public class GameplayState : ApplicationState
    {

        public GameplayRenderer GameplayRendererObject;
        private GameState m_previousState;
        public MatchState Match;
        SinglePlayerInputs m_p1LastInputs;//for debugging
        SinglePlayerInputs m_p2LastInputs;
        SplashState CurrentSplashState;
        Match.SetData m_setData;

        static public GameplayState CreateGameplayState(Match.SetData _setData, GameplayRenderer _renderer)
        {
            GameplayState gs = new GameplayState(_setData);
            gs.GameplayRendererObject = _renderer;
            return gs;
        }

        private GameplayState(Match.SetData _setData)
        {
            m_setData = _setData;
            Match = new MatchState();
            CurrentSplashState = new SplashState();
            m_previousState = new GameState();
            m_previousState = SetRoundStart();
        }

        
        public override void Update(Inputs _inputs)
        {
            GameplayStateUpdate(_inputs.P1_Inputs, _inputs.P2_Inputs);
        }

        void GameplayStateUpdate(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs)
        {


            //1.1 Check for pause? todo
            if (CurrentSplashState.CurrentState == SplashState.State.GameOver)
            {
                //check if any button is pressed to start a new game
                if (_p1Inputs.A || _p1Inputs.B || _p1Inputs.C || _p1Inputs.Start ||
                    _p2Inputs.A || _p2Inputs.B || _p2Inputs.C || _p2Inputs.Start)
                {
                    ApplicationStateManager.GetInstance().SetCharacterSelectScreen(m_setData);
                }
            }
            if (UpdateSplashScreen())
            {

            }
            else if (m_previousState.RemainingHitstop > 0)
            {
                m_previousState.RemainingHitstop--;
            }
            else
            {

                //2. Sees if the inputs can be applied to current action
                GameState currentState = UpdateGameStateWithInputs(_p1Inputs, _p2Inputs, m_previousState);
                m_p1LastInputs = _p1Inputs;
                m_p2LastInputs = _p2Inputs;

                //3. Resolves actions
                MatchOutcome outcome = ResolveActions(_p1Inputs, _p2Inputs, currentState);



                //4. Checks if the match is over
                if (outcome.IsEnd())
                {
                    //Extra render on game end?
                    GameplayRendererObject.RenderScene(m_previousState, Match, CurrentSplashState);
                    HandleOutcome(outcome);
                    CurrentSplashState.CurrentState = SplashState.State.RoundOver_ShowResult;
                    CurrentSplashState.FramesRemaining = GameplayConstants.FRAMES_END_ROUND_SPLASH;
                }
                --currentState.RemainingTime;
                m_previousState = currentState;

            }

            //5. Render Scene
            GameplayRendererObject.RenderScene(m_previousState, Match, CurrentSplashState);
        }

        //returns whether or not to stop here and wait a few more frames
        public bool UpdateSplashScreen()
        {


            //removing frames has to be done before checking the state as if the splash happens during gameplay it should still be rendered
            if (CurrentSplashState.FramesRemaining > 0)
                --CurrentSplashState.FramesRemaining;

            if (CurrentSplashState.CurrentState == SplashState.State.None)
                return false;
            if (CurrentSplashState.FramesRemaining == 0)
            {
                switch (CurrentSplashState.CurrentState)
                {
                    case SplashState.State.RoundOver_ShowResult:
                        if (Match.GameOver)
                        {
                            CurrentSplashState.CurrentState = SplashState.State.GameOver;
                            CurrentSplashState.FramesRemaining = GameplayConstants.GAME_OVER_LENGTH;
                        }
                        else
                            m_previousState = SetRoundStart();
                        break;
                    case SplashState.State.RoundStart_3:
                        CurrentSplashState.CurrentState = SplashState.State.RoundStart_2;
                        CurrentSplashState.FramesRemaining = GameplayConstants.FRAMES_COUNTDOWN;
                        break;
                    case SplashState.State.RoundStart_2:
                        CurrentSplashState.CurrentState = SplashState.State.RoundStart_1;
                        CurrentSplashState.FramesRemaining = GameplayConstants.FRAMES_COUNTDOWN;
                        break;
                    case SplashState.State.RoundStart_1:
                        CurrentSplashState.CurrentState = SplashState.State.RoundStart_F;
                        CurrentSplashState.FramesRemaining = GameplayConstants.FRAMES_COUNTDOWN;
                        break;
                    case SplashState.State.RoundStart_F:
                        CurrentSplashState.CurrentState = SplashState.State.None;
                        CurrentSplashState.FramesRemaining = GameplayConstants.FRAMES_COUNTDOWN;
                        break;
                    case SplashState.State.GameOver:
                        //hmm this could go on indefinitely
                        //ApplicationStateManager.GetInstance().SetCharacterSelectScreen(m_setData);
                        break;

                }
            }
            if (CurrentSplashState.CurrentState == SplashState.State.RoundStart_F)
                return false;
            return true;
        }

        void OnGUI()
        {
            //if (!Initialized)
            //    return;
            //float msec = deltaTime * 1000.0f;
            //float fps = 1.0f / deltaTime;
            //string fpstext = string.Format("\n\r{0:0.0} ms ({1:0.} fps)", msec, fps);
            //string clock = "\n\r" + System.DateTime.Now.Second.ToString();
            //string dtime = "\n\r" + Time.deltaTime.ToString();
            //string output = "FPS : " + fpstext +
            //    "\n\rP1 Current Action : " + m_previousState.P1_State.ToString() +
            //    "\n\rP1 dir : " + m_p1LastInputs.JoystickDirection.ToString() +
            //    "\n\rP1 A : " + m_p1LastInputs.A.ToString() +
            //    "\n\rP1 B : " + m_p1LastInputs.B.ToString() +
            //    "\n\rP1 X : " + Input.GetAxisRaw("Horizontal_PsStick1").ToString() +
            //    "\n\rP1 Y : " + Input.GetAxisRaw("Vertical_PsStick1").ToString() +
            //    "\n\rP2 Current Action : " + m_previousState.P2_State.ToString() +
            //    "\n\rP2 dir : " + m_p2LastInputs.JoystickDirection.ToString() +
            //    "\n\rP2 A : " + m_p2LastInputs.A.ToString() +
            //    "\n\rP2 B : " + m_p2LastInputs.B.ToString() +
            //    "\n\rP2 X : " + Input.GetAxisRaw("Horizontal_PsStick2").ToString() +
            //    "\n\rP2 Y : " + Input.GetAxisRaw("Vertical_PsStick2").ToString();

            string sweepdebug = "P1 Gauge : " + m_previousState.P1_Gauge.ToString() + "/" + m_previousState.P1_CState.SelectedCharacter.MaxGauge.ToString() +
                "\n\rP1 Current Action : " + m_previousState.P1_State.ToString() +
                "\n\rP1 StateFrames : " + m_previousState.P1_CState.StateFrames.ToString() +
                "\n\rP1 SweepState : " + (m_previousState.P1_CState.SelectedCharacter as Sweep).State.ToString();

            //string joystickdirs = "P2 dir : " + m_p2LastInputs.JoystickDirection.ToString() +
            //    "\n\rP2 X : " + Input.GetAxisRaw("Horizontal_PsStick2").ToString();

            //
            string score = "\n\rP1 Score : " + Match.P1_Score.ToString() +
                "\n\rP2 Score : " + Match.P2_Score.ToString() +
                "\n\rTime : " + (m_previousState.RemainingTime / 60).ToString();


            //string splash = CurrentSplashState.CurrentState.ToString() + CurrentSplashState.FramesRemaining.ToString();



            GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height / 2), score + sweepdebug);
        }

        private void HandleOutcome(MatchOutcome _outcome)
        {
            if (_outcome.P1_Scores)
            {
                Match.P1_Score++;
            }
            if (_outcome.P2_Scores)
            {
                Match.P2_Score++;
            }
            Match.Outcomes.Add(_outcome);
            if (_outcome.Outcome == GameplayEnums.Outcome.TimeOut || _outcome.Outcome == GameplayEnums.Outcome.Trade)
            {
                if (Match.P1_Score >= GameplayConstants.ROUNDS_TO_WIN)
                    Match.P1_Score = GameplayConstants.ROUNDS_TO_WIN - 1;
                if (Match.P2_Score >= GameplayConstants.ROUNDS_TO_WIN)
                    Match.P2_Score = GameplayConstants.ROUNDS_TO_WIN - 1;
            }
            if (Match.P1_Score >= GameplayConstants.ROUNDS_TO_WIN || Match.P2_Score >= GameplayConstants.ROUNDS_TO_WIN)
            {
                Match.GameOver = true;
            }
        }

        private GameState SetRoundStart()
        {
            CurrentSplashState.CurrentState = SplashState.State.RoundStart_3;
            CurrentSplashState.FramesRemaining = GameplayConstants.FRAMES_COUNTDOWN;

            GameState state = new GameState();

            state.P1_Gauge = m_previousState.P1_Gauge;
            state.P1_Hitboxes.Add(CreateCharacterStartingHitbox());
            state.P1_Position = GameplayConstants.STARTING_POSITION * -1;
            state.P1_State = GameplayEnums.CharacterState.Idle;
            state.P1_StateFrames = 0;

            state.P2_Gauge = m_previousState.P2_Gauge;
            state.P2_Hitboxes.Add(CreateCharacterStartingHitbox());
            state.P2_Position = GameplayConstants.STARTING_POSITION;
            state.P2_State = GameplayEnums.CharacterState.Idle;
            state.P2_StateFrames = 0;

            state.RemainingHitstop = 0;
            state.RemainingTime = GameplayConstants.FRAMES_PER_ROUND;


            m_p1LastInputs = new SinglePlayerInputs();
            m_p2LastInputs = new SinglePlayerInputs();

            return state;
        }



        private Hitbox_Gameplay CreateCharacterStartingHitbox()
        {
            Hitbox_Gameplay hbox = new Hitbox_Gameplay(GameplayEnums.HitboxType.Hurtbox_Main, 0, GameplayConstants.CHARACTER_HURTBOX_WIDTH);
            return hbox;
        }

        private SinglePlayerInputs GetInputs(bool _p1)
        {
            short direction = 5;
            SinglePlayerInputs inputs = new SinglePlayerInputs();
            float h;
            float v;

            if (_p1)
            {
                //if(P1_Joystick)
                {
                    h = Input.GetAxisRaw("Horizontal_PsStick1") + Input.GetAxisRaw("Horizontal_KB1"); ;
                    v = Input.GetAxisRaw("Vertical_PsStick1") + Input.GetAxisRaw("Vertical_KB1");
                }
                //else
                //{
                //    h = Input.GetAxis("Horizontal_KB1");
                //    v = Input.GetAxis("Vertical_KB1");
                //}
                inputs.A = Input.GetButton("A_P1");
                inputs.B = Input.GetButton("B_P1");
                inputs.C = Input.GetButton("C_P1");
                inputs.Start = Input.GetButtonDown("Start_P1");
            }
            else
            {
                //if (P2_Joystick)
                {
                    h = Input.GetAxisRaw("Horizontal_PsStick2") + Input.GetAxisRaw("Horizontal_KB2");
                    v = Input.GetAxisRaw("Vertical_PsStick2") + Input.GetAxisRaw("Vertical_KB2");
                }
                //else
                //{
                //    h = Input.GetAxis("Horizontal_KB2");
                //    v = Input.GetAxis("Vertical_KB2");
                //}
                inputs.A = Input.GetButton("A_P2");
                inputs.B = Input.GetButton("B_P2");
                inputs.C = Input.GetButton("C_P2");
                inputs.Start = Input.GetButtonDown("Start_P2");
            }
            if (h > 0.1)
            {
                if (v > 0.1)
                {
                    if (_p1)
                        direction = 9;
                    else
                        direction = 7;

                }
                else if (v < -0.1)
                {
                    if (_p1)
                        direction = 3;
                    else
                        direction = 1;
                }
                else
                {
                    if (_p1)
                        direction = 6;
                    else
                        direction = 4;
                }
            }
            else if (h < -0.1)
            {
                if (v > 0.1)
                {
                    if (_p1)
                        direction = 7;
                    else
                        direction = 9;

                }
                else if (v < -0.1)
                {
                    if (_p1)
                        direction = 1;
                    else
                        direction = 3;
                }
                else
                {
                    if (_p1)
                        direction = 4;
                    else
                        direction = 6;
                }
            }
            else
            {
                if (v > 0.1)
                {
                    if (_p1)
                        direction = 8;
                    else
                        direction = 8;

                }
                else if (v < -0.1)
                {
                    if (_p1)
                        direction = 2;
                    else
                        direction = 2;
                }
                else
                {
                    direction = 5;
                }
            }
            inputs.JoystickDirection = direction;
            return inputs;
        }

        GameState UpdateGameStateWithInputs(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs, GameState _previousState)
        {
            GameState updatedState = new GameState(_previousState);

            //Check if current state can be affected by inputs (idle, crouch, walking into buttons, as well as throw tech)

            //1. p1
            updatedState.P1_CState.UpdateStateWithInputs(_p1Inputs, m_p1LastInputs, updatedState.P2_CState);

            //2. p2
            updatedState.P2_CState.UpdateStateWithInputs(_p2Inputs, m_p2LastInputs, updatedState.P1_CState);


            return updatedState;
        }



        private MatchOutcome ResolveActions(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs, GameState _currentState)
        {
            //1. Update individual actions and positions
            MatchOutcome p1_update_outcome = _currentState.P1_CState.UpdateCharacterState();
            if (p1_update_outcome.IsEnd())
                return p1_update_outcome;
            MatchOutcome p2_update_outcome = _currentState.P2_CState.UpdateCharacterState();
            if (p2_update_outcome.IsEnd())
                return p2_update_outcome;

            //2. Resolve hitbox interaction
            MatchOutcome hitbox_outcome = ResolveHitboxInteractions(_p1Inputs, _p2Inputs, _currentState);
            if (hitbox_outcome.IsEnd())
                return hitbox_outcome;

            //3. Resolve Positions
            ResolvePositions(_currentState);

            //4. Check timer
            if (_currentState.RemainingTime < 0)
                return new MatchOutcome(true, true, GameplayEnums.Outcome.TimeOut);

            return new MatchOutcome();
        }

        private void ResolvePositions(GameState _currentState)
        {
            Hitbox_Gameplay p1 = _currentState.P1_Hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main);
            Hitbox_Gameplay p2 = _currentState.P2_Hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main);
            //1. if characters are overlapping, push each other back enough that they don't overlap
            if (DoHitboxesOverlap(p1, p2, _currentState))
            {
                int overlapAmount = (_currentState.P1_Position + p1.Position + p1.Width / 2) - (_currentState.P2_Position - p2.Position - p2.Width / 2);
                if (overlapAmount > 0)
                {
                    int pushback = overlapAmount / 2;
                    _currentState.P1_Position -= pushback;
                    _currentState.P2_Position += pushback;
                }
            }

            //2. if either character extends beyond the edge, push him out of the corner.
            //2.1 if this causes characters to overlap, push the other character back
            if (_currentState.P1_Position < GameplayConstants.ARENA_RADIUS * -1)
            {
                _currentState.P1_Position -= GameplayConstants.ARENA_RADIUS + _currentState.P1_Position;
                if (DoHitboxesOverlap(p1, p2, _currentState))
                {
                    int overlapAmount = (_currentState.P1_Position + p1.Position + p1.Width / 2) - (_currentState.P2_Position + p2.Position + p2.Width / 2);
                    if (overlapAmount > 0)
                    {
                        _currentState.P2_Position += overlapAmount;
                    }
                }
            }
            if (_currentState.P2_Position > GameplayConstants.ARENA_RADIUS)
            {
                _currentState.P2_Position -= _currentState.P2_Position - GameplayConstants.ARENA_RADIUS;
                if (DoHitboxesOverlap(p1, p2, _currentState))
                {
                    int overlapAmount = (_currentState.P1_Position + p1.Position + p1.Width / 2) - (_currentState.P2_Position + p2.Position + p2.Width / 2);
                    if (overlapAmount > 0)
                    {
                        _currentState.P1_Position -= overlapAmount;
                    }
                }
            }

        }

        private MatchOutcome ResolveHitboxInteractions(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs, GameState _currentState)
        {
            bool p1_hits_p2 = false;
            bool p2_hits_p1 = false;
            bool clash = false;
            bool throwBreak = false;
            bool p1_throws_p2 = false;
            bool p2_throws_p1 = false;
            //check p1 attacks first
            foreach (Hitbox_Gameplay hg in _currentState.P1_Hitboxes)
            {
                if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack || hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                {
                    if (hg.HasStruck)
                        continue;
                    foreach (Hitbox_Gameplay p2_hg in _currentState.P2_Hitboxes)
                    {
                        if (p2_hg.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb || p2_hg.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main)
                        {
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
                                if (DoHitboxesOverlap(hg, p2_hg, _currentState))
                                {
                                    p1_hits_p2 = true;
                                    hg.HasStruck = true;
                                }
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                                if (DoHitboxesOverlap(hg, p2_hg, _currentState) && p2_hg.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main)
                                {
                                    if (_currentState.P2_State == GameplayEnums.CharacterState.ThrowStartup)
                                        throwBreak = true;
                                    else
                                        p1_throws_p2 = true;
                                    hg.HasStruck = true;
                                }
                        }
                        else if (p2_hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
                        {
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
                                if (DoHitboxesOverlap(hg, p2_hg, _currentState))
                                {
                                    clash = true;
                                    hg.HasStruck = true;
                                }
                        }
                        else if (p2_hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                        {
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                                if (DoHitboxesOverlap(hg, p2_hg, _currentState))
                                {
                                    throwBreak = true;
                                    hg.HasStruck = true;
                                }
                        }
                    }
                }
            }

            //check p2
            foreach (Hitbox_Gameplay hg in _currentState.P2_Hitboxes)
            {
                if (hg.HasStruck)
                    continue;
                if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack || hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                {
                    foreach (Hitbox_Gameplay p1_hg in _currentState.P1_Hitboxes)
                    {
                        if (p1_hg.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb || p1_hg.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main)
                        {
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
                                if (DoHitboxesOverlap(p1_hg, hg, _currentState))
                                {
                                    p2_hits_p1 = true;
                                    hg.HasStruck = true;
                                }
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                                if (DoHitboxesOverlap(p1_hg, hg, _currentState) && p1_hg.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main)
                                {
                                    if (_currentState.P1_State == GameplayEnums.CharacterState.ThrowStartup)
                                        throwBreak = true;
                                    else
                                        p2_throws_p1 = true;
                                    hg.HasStruck = true;
                                }
                        }
                        else if (p1_hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
                        {
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
                                if (DoHitboxesOverlap(p1_hg, hg, _currentState))
                                {
                                    clash = true;
                                    hg.HasStruck = true;
                                }
                        }
                        else if (p1_hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                        {
                            if (hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
                                if (DoHitboxesOverlap(p1_hg, hg, _currentState))
                                {
                                    throwBreak = true;
                                    hg.HasStruck = true;
                                }
                        }
                    }
                }
            }
            MatchOutcome res = new MatchOutcome();
            //handle results :
            bool p1_is_hit = false;
            bool p2_is_hit = false;

            if (p1_hits_p2)
            {
                if (DoesPlayerBlock(false, _currentState, _p2Inputs))
                {
                    //set p2 to be in blockstun
                    _currentState.P2_State = GameplayEnums.CharacterState.Blockstun;
                    _currentState.P2_StateFrames = 0;
                    //set hitstop
                    _currentState.RemainingHitstop = GameplayConstants.BLOCK_HITSTOP;
                    //give gauge
                    _currentState.P1_CState.AddGauge(1);
                }
                else
                {
                    p2_is_hit = true;
                }
            }
            if (p2_hits_p1)
            {
                if (DoesPlayerBlock(true, _currentState, _p1Inputs))
                {
                    //set p2 to be in blockstun
                    _currentState.P1_State = GameplayEnums.CharacterState.Blockstun;
                    _currentState.P1_StateFrames = 0;
                    //set hitstop
                    _currentState.RemainingHitstop = GameplayConstants.BLOCK_HITSTOP;
                    //give gauge
                    _currentState.P2_CState.AddGauge(1);
                }
                else
                {
                    p1_is_hit = true;
                }
            }

            if (p1_is_hit && p2_is_hit)
                res = new MatchOutcome(true, true, GameplayEnums.Outcome.Trade);
            else if (p1_is_hit)
            {
                GameplayEnums.Outcome outcome = GetOutcomeFromOpponentState(_currentState.P1_CState);
                outcome = GetOutcomeFromPlayerState(_currentState.P2_CState, outcome);
                res = new MatchOutcome(false, true, outcome);
            }
            else if (p2_is_hit)
            {
                GameplayEnums.Outcome outcome = GetOutcomeFromOpponentState(_currentState.P2_CState);
                outcome = GetOutcomeFromPlayerState(_currentState.P1_CState, outcome);
                res = new MatchOutcome(true, false, outcome);
            }
            else if ((p2_throws_p1 && p1_throws_p2) || throwBreak)
            {
                _currentState.P2_State = GameplayEnums.CharacterState.ThrowBreak;
                _currentState.P2_StateFrames = 0;
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                _currentState.P1_State = GameplayEnums.CharacterState.ThrowBreak;
                _currentState.P1_StateFrames = 0;
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
            }
            else if (p1_throws_p2)
            {
                _currentState.P2_State = GameplayEnums.CharacterState.BeingThrown;
                _currentState.P2_StateFrames = 0;
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                _currentState.P1_State = GameplayEnums.CharacterState.ThrowingOpponent;
                _currentState.P1_StateFrames = 0;
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
            }
            else if (p2_throws_p1)
            {
                _currentState.P2_State = GameplayEnums.CharacterState.ThrowingOpponent;
                _currentState.P2_StateFrames = 0;
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                _currentState.P1_State = GameplayEnums.CharacterState.BeingThrown;
                _currentState.P1_StateFrames = 0;
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
            }
            else if (clash)
            {
                _currentState.P2_State = GameplayEnums.CharacterState.Clash;
                _currentState.P2_StateFrames = 0;
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack);
                _currentState.P1_State = GameplayEnums.CharacterState.Clash;
                _currentState.P1_StateFrames = 0;
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                _currentState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack);
            }

            return res;
        }

        private bool DoesPlayerBlock(bool _p1, GameState _currentState, SinglePlayerInputs _inputs)
        {
            if (!(_inputs.JoystickDirection == 1 || _inputs.JoystickDirection == 4 || _inputs.JoystickDirection == 7))
                return false;
            GameplayEnums.CharacterState charState;

            Hitbox_Gameplay hitbox = _currentState.P1_CState.Hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack);
            if (_p1)
            {
                charState = _currentState.P1_State;
                hitbox = _currentState.P2_CState.Hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack);
            }
            else
            {
                charState = _currentState.P2_State;
                hitbox = _currentState.P1_CState.Hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack);
            }
            if (charState == GameplayEnums.CharacterState.Crouch || charState == GameplayEnums.CharacterState.Idle || charState == GameplayEnums.CharacterState.WalkBack)
            {
                switch (hitbox.AttackAttribute)
                {
                    case GameplayEnums.AttackAttribute.High:
                        if ((_inputs.JoystickDirection == 1 || _inputs.JoystickDirection == 2 || _inputs.JoystickDirection == 3))
                            return false;
                        break;
                    case GameplayEnums.AttackAttribute.Low:
                        if (!(_inputs.JoystickDirection == 1 || _inputs.JoystickDirection == 2 || _inputs.JoystickDirection == 3))
                            return false;
                        break;
                }
                return true;
            }
            return false;
        }

        private GameplayEnums.Outcome GetOutcomeFromOpponentState(CharacterState _charState)
        {
            switch (_charState.State)
            {
                case GameplayEnums.CharacterState.AttackActive:
                case GameplayEnums.CharacterState.AttackStartup:
                case GameplayEnums.CharacterState.ThrowActive:
                case GameplayEnums.CharacterState.ThrowStartup:
                    return GameplayEnums.Outcome.Counter;
                case GameplayEnums.CharacterState.AttackRecovery:
                    return GameplayEnums.Outcome.WhiffPunish;
                case GameplayEnums.CharacterState.ThrowRecovery:
                    return GameplayEnums.Outcome.Shimmy;
                case GameplayEnums.CharacterState.Special:
                    return _charState.SelectedCharacter.GetOutcomeIfHit();
                default:
                    return GameplayEnums.Outcome.StrayHit;
            }
        }

        private GameplayEnums.Outcome GetOutcomeFromPlayerState(CharacterState _winner, GameplayEnums.Outcome _previousOutcome)
        {
            //if previous outcome is "stray hit", it can be overridden with 
            if (_previousOutcome == GameplayEnums.Outcome.StrayHit)
            {
                if (_winner.State == GameplayEnums.CharacterState.Special)
                {
                    return _winner.SelectedCharacter.GetCurrentCharacterSpecialOutcome();
                }
            }
            return _previousOutcome;
        }

        private bool DoHitboxesOverlap(Hitbox_Gameplay _p1Box, Hitbox_Gameplay _p2Box, GameState _currentState)
        {
            int p1_left = _currentState.P1_Position + _p1Box.Position - _p1Box.Width / 2;
            int p1_right = _currentState.P1_Position + _p1Box.Position + _p1Box.Width / 2;
            int p2_left = _currentState.P2_Position + _p2Box.Position - _p2Box.Width / 2;
            int p2_right = _currentState.P2_Position + _p2Box.Position + _p2Box.Width / 2;

            if (p1_right < p2_left)
                return false;
            if (p2_right < p1_left)
                return false;
            return true;
        }

    }

}


