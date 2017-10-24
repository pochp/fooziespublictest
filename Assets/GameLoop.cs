using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// TO DO :
/// ok-keep working on finishing up hitbox detection
/// ok-make sure throw breaks remove hurtboxes
/// -renderer
/// -scoring
/// -main menu, declare winner
/// -gauge, special
/// </summary>

public class GameLoop : MonoBehaviour {

    public GameplayRenderer GameplayRendererObject;
    private GameState m_previousState;
    public MatchState Match;

    public bool P1_Joystick = true;
    public bool P2_Joystick = false;

    float deltaTime = 0.0f;//for fps display
    float m_timeSinceLastUpdate = 0.0f;
    SinglePlayerInputs m_p1LastInputs;//for debugging
    SinglePlayerInputs m_p2LastInputs;
    SplashState CurrentSplashState;

    /// <summary>
    /// GAMEPLAY CONSTANTS
    /// </summary>
    /// 

    const int ATTACK_STARTUP = 8;
    const int ATTACK_ACTIVE = 4;
    const int ATTACK_RECOVERY_TOTAL = 26;
    const int ATTACK_RECOVERY_SHORTEN = 10;
    const int HURTBOX_WHIFF_EARLY = 2000;
    const int HURTBOX_WHIFF_LATE = 1000;
    const int HURTBOX_STARTUP = 1000;
    const int HURTBOX_ACTIVE = 1500; //hurtbox when active is shorter than the recovery so it favorises clashes
    const int HITBOX_ACTIVE = 2200;
    const int CHARACTER_HURTBOX_WIDTH = 500;
    const int CHARACTER_HURTBOX_HEIGHT = 2500;
    const int CROUCHING_HURTBOX_WIDTH = 600;
    const int CROUCHING_HURTBOX_HEIGHT = 1000;
    const int THROW_STARTUP = 3;
    const int THROW_ACTIVE = 2;
    const int THROW_RECOVERY = 30;
    const int THROW_BREAK_WINDOW = 5;
    const int THROW_STARTUP_HURTBOX = 400;
    const int THROW_ACTIVE_RANGE = 500;
    const int THROW_RECOVERY_HURTBOX = 400;
    const int WALK_F_SPEED = 135;
    const int WALK_B_SPEED = -130;
    const int ATTACK_HITSTOP = 5;
    const int ATTACK_BLOCKSTUN = 20;
    const int ATTACK_PUSHBACK_SPEED = -120;
    const int ATTACK_PUSHBACK_DURATION = 5;
    const int BREAK_DURATION = 15;
    const int BREAK_PUSHBACK = -120;
    const int CLASH_PUSHBACK_SPEED = -150;
    const int CLASH_PUSHBACK_DURATION = 10;
    const int CLASH_HITSTOP = 20;
    const int BLOCK_HITSTOP = 10;
    const int STARTING_POSITION = 5000;
    const int FRAMES_PER_ROUND = 1800; //30 seconds
    const int ARENA_RADIUS = 8700;
    const int ROUNDS_TO_WIN = 5;

    /// <summary>
    /// TIME CONSTANTS
    /// </summary>
    const int FRAMES_END_ROUND_SPLASH = 180;
    const int FRAMES_COUNTDOWN = 30;
    const float FRAME_LENGTH = 0.016666666666f;

    bool Initialized = false;


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update() {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        m_timeSinceLastUpdate += deltaTime;
        if (m_timeSinceLastUpdate > FRAME_LENGTH)
            m_timeSinceLastUpdate = 0f;
        else
            return;

        if(!Initialized)
        {
            Match = new MatchState();
            CurrentSplashState = new SplashState();
            m_previousState = SetRoundStart();
            Initialized = true;
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
            //1. Checks for inputs
            SinglePlayerInputs p1_inputs = GetInputs(true);
            SinglePlayerInputs p2_inputs = GetInputs(false);

            //1.1 Check for pause? todo

            //2. Sees if the inputs can be applied to current action
            GameState currentState = UpdateGameStateWithInputs(p1_inputs, p2_inputs, m_previousState);
            m_p1LastInputs = p1_inputs;
            m_p2LastInputs = p2_inputs;

            //3. Resolves actions
            MatchOutcome outcome = ResolveActions(p1_inputs, p2_inputs, currentState);



            //4. Checks if the match is over
            if (outcome.IsEnd())
            {
                HandleOutcome(outcome);
                CurrentSplashState.CurrentState = SplashState.State.RoundOver_ShowResult;
                CurrentSplashState.FramesRemaining = FRAMES_END_ROUND_SPLASH;
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
            switch(CurrentSplashState.CurrentState)
            {
                case SplashState.State.RoundOver_ShowResult:
                    m_previousState = SetRoundStart();
                    break;
                case SplashState.State.RoundStart_3:
                    CurrentSplashState.CurrentState = SplashState.State.RoundStart_2;
                    CurrentSplashState.FramesRemaining = FRAMES_COUNTDOWN;
                    break;
                case SplashState.State.RoundStart_2:
                    CurrentSplashState.CurrentState = SplashState.State.RoundStart_1;
                    CurrentSplashState.FramesRemaining = FRAMES_COUNTDOWN;
                    break;
                case SplashState.State.RoundStart_1:
                    CurrentSplashState.CurrentState = SplashState.State.RoundStart_F;
                    CurrentSplashState.FramesRemaining = FRAMES_COUNTDOWN;
                    break;
                case SplashState.State.RoundStart_F:
                    CurrentSplashState.CurrentState = SplashState.State.None;
                    CurrentSplashState.FramesRemaining = FRAMES_COUNTDOWN;
                    break;
            }
        }
        if (CurrentSplashState.CurrentState == SplashState.State.RoundStart_F)
            return false;
        return true;
    }

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string fpstext = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        //string output = "FPS : " + fpstext +
        //    "\n\rP1 Current Action : " + m_previousState.P1_State.ToString() +
        //    "\n\rP1 dir : " + m_p1LastInputs.JoystickDirection.ToString() +
        //    "\n\rP1 A : " + m_p1LastInputs.A.ToString() +
        //    "\n\rP1 B : " + m_p1LastInputs.B.ToString() +
        //    "\n\rP1 X : " + Input.GetAxis("Horizontal_P1").ToString() +
        //    "\n\rP1 Y : " + Input.GetAxis("Vertical_P1").ToString() +
        //    "\n\rP2 Current Action : " + m_previousState.P2_State.ToString() +
        //    "\n\rP2 dir : " + m_p2LastInputs.JoystickDirection.ToString() +
        //    "\n\rP2 A : " + m_p2LastInputs.A.ToString() +
        //    "\n\rP2 B : " + m_p2LastInputs.B.ToString() +
        //    "\n\rP2 X : " + Input.GetAxis("Horizontal_P2").ToString() +
        //    "\n\rP2 Y : " + Input.GetAxis("Vertical_P2").ToString();
    
        string score = "P1 Score : " + Match.P1_Score.ToString() +
            "\n\rP2 Score : " + Match.P2_Score.ToString() +
            "\n\rTime : " + (m_previousState.RemainingTime / 60 ).ToString();


        //string splash = CurrentSplashState.CurrentState.ToString() + CurrentSplashState.FramesRemaining.ToString();
    
    
    
        GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height / 5), score);
    }

    private void HandleOutcome(MatchOutcome _outcome)
    {
        if(_outcome.P1_Scores)
        {
            Match.P1_Score++;
        }
        if(_outcome.P2_Scores)
        {
            Match.P2_Score++;
        }
        Match.Outcomes.Add(_outcome);
        if(_outcome.Outcome == GameplayEnums.Outcome.TimeOut || _outcome.Outcome == GameplayEnums.Outcome.Trade)
        {
            if (Match.P1_Score >= ROUNDS_TO_WIN)
                Match.P1_Score = ROUNDS_TO_WIN - 1;
            if (Match.P2_Score >= ROUNDS_TO_WIN)
                Match.P2_Score = ROUNDS_TO_WIN - 1;
        }
    }

    private GameState SetRoundStart()
    {
        CurrentSplashState.CurrentState = SplashState.State.RoundStart_3;
        CurrentSplashState.FramesRemaining = FRAMES_COUNTDOWN;

        GameState state = new GameState();

        state.P1_Gauge = 0;
        state.P1_Hitboxes.Add(CreateCharacterStartingHitbox());
        state.P1_Position = STARTING_POSITION * -1;
        state.P1_State = GameplayEnums.CharacterState.Idle;
        state.P1_StateFrames = 0;

        state.P2_Gauge = 0;
        state.P2_Hitboxes.Add(CreateCharacterStartingHitbox());
        state.P2_Position = STARTING_POSITION;
        state.P2_State = GameplayEnums.CharacterState.Idle;
        state.P2_StateFrames = 0;

        state.RemainingHitstop = 0;
        state.RemainingTime = FRAMES_PER_ROUND;


        m_p1LastInputs = new SinglePlayerInputs();
        m_p2LastInputs = new SinglePlayerInputs();

        return state;
    }



    private Hitbox_Gameplay CreateCharacterStartingHitbox()
    {
        Hitbox_Gameplay hbox = new Hitbox_Gameplay(GameplayEnums.HitboxType.Hurtbox_Main, 0, CHARACTER_HURTBOX_WIDTH);
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
                h = Input.GetAxis("Horizontal_PsStick1") + Input.GetAxis("Horizontal_KB1"); ;
                v = Input.GetAxis("Vertical_PsStick1") + Input.GetAxis("Vertical_KB1");
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
                h = Input.GetAxis("Horizontal_PsStick2") + Input.GetAxis("Horizontal_KB2");
                v = Input.GetAxis("Vertical_PsStick2") + Input.GetAxis("Vertical_KB2");
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
        if(h > 0.1)
        {
            if(v > 0.1)
            {
                if (_p1)
                    direction = 9;
                else
                    direction = 7;

            }
            else if(v < -0.1)
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
        else if(h < -0.1)
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
        switch(updatedState.P1_State)
        {
            case GameplayEnums.CharacterState.Crouch:
            case GameplayEnums.CharacterState.Idle:
            case GameplayEnums.CharacterState.WalkBack:
            case GameplayEnums.CharacterState.WalkForward:
                SetCharacterState( GetCharacterAction(_p1Inputs, m_p1LastInputs), true, updatedState);
                break;
            case GameplayEnums.CharacterState.BeingThrown:
                if (_p1Inputs.B && !m_p1LastInputs.B)
                {
                    updatedState.P2_State = GameplayEnums.CharacterState.ThrowBreak;
                    updatedState.P2_StateFrames = 0;
                    updatedState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                    updatedState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                    updatedState.P1_State = GameplayEnums.CharacterState.ThrowBreak;
                    updatedState.P1_StateFrames = 0;
                    updatedState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                    updatedState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                }
                break;
        }

        //2. p2
        switch (updatedState.P2_State)
        {
            case GameplayEnums.CharacterState.Crouch:
            case GameplayEnums.CharacterState.Idle:
            case GameplayEnums.CharacterState.WalkBack:
            case GameplayEnums.CharacterState.WalkForward:
                SetCharacterState(GetCharacterAction(_p2Inputs, m_p2LastInputs), false, updatedState);
                break;
            case GameplayEnums.CharacterState.BeingThrown:
                if (_p2Inputs.B && !m_p2LastInputs.B)
                {
                    updatedState.P2_State = GameplayEnums.CharacterState.ThrowBreak;
                    updatedState.P2_StateFrames = 0;
                    updatedState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                    updatedState.P2_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                    updatedState.P1_State = GameplayEnums.CharacterState.ThrowBreak;
                    updatedState.P1_StateFrames = 0;
                    updatedState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                    updatedState.P1_Hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                }
                break;
        }


        return updatedState;
    }

    private void SetCharacterState(GameplayEnums.CharacterState _state, bool _p1, GameState _currentState)
    {
        List<Hitbox_Gameplay> _hboxes;
        if (_p1)
        {
            _hboxes = _currentState.P1_Hitboxes;
            _currentState.P1_State = _state;
            _currentState.P1_StateFrames = 0;
        }
        else
        {
            _hboxes = _currentState.P2_Hitboxes;
            _currentState.P2_State = _state;
            _currentState.P2_StateFrames = 0;
        }
        switch (_state)
        {
            case GameplayEnums.CharacterState.AttackStartup:
                _hboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hurtbox_Limb, _p1, HURTBOX_STARTUP));
                SetCharacterHurtboxStanding(_hboxes);
                break;
            case GameplayEnums.CharacterState.SpecialStartup:
                _hboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hurtbox_Limb, _p1, HURTBOX_STARTUP));
                SetCharacterHurtboxStanding(_hboxes);
                break;
            case GameplayEnums.CharacterState.ThrowStartup:
                _hboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hurtbox_Limb, _p1, THROW_STARTUP_HURTBOX));
                SetCharacterHurtboxStanding(_hboxes);
                break;
            case GameplayEnums.CharacterState.WalkBack:
            case GameplayEnums.CharacterState.WalkForward:
            case GameplayEnums.CharacterState.Idle:
                SetCharacterHurtboxStanding(_hboxes);
                break;
            case GameplayEnums.CharacterState.Crouch:
                SetCharacterHurtboxCrouching(_hboxes);
                break;
        }
    }
    
    private GameplayEnums.CharacterState GetCharacterAction(SinglePlayerInputs _inputs, SinglePlayerInputs _previousInputs)
    {
        if(_inputs.C && !_previousInputs.C)
        {
            return GameplayEnums.CharacterState.SpecialStartup;
        }
        if(_inputs.B && !_previousInputs.B)
        {
            return GameplayEnums.CharacterState.ThrowStartup;
        }
        if(_inputs.A && !_previousInputs.A)
        {
            return GameplayEnums.CharacterState.AttackStartup;
        }
        switch(_inputs.JoystickDirection)
        {
            case 9:
            case 6:
                return GameplayEnums.CharacterState.WalkForward;
            case 7:
            case 4:
                return GameplayEnums.CharacterState.WalkBack;
            case 1:
            case 2:
            case 3:
                return GameplayEnums.CharacterState.Crouch;
        }
        return GameplayEnums.CharacterState.Idle;
    }

    private MatchOutcome ResolveActions(SinglePlayerInputs _p1Inputs, SinglePlayerInputs _p2Inputs, GameState _currentState)
    {
        //1. Update individual actions and positions
        MatchOutcome p1_update_outcome = UpdateSinglePlayerActions(_currentState, true);
        if (p1_update_outcome.IsEnd())
            return p1_update_outcome;
        MatchOutcome p2_update_outcome = UpdateSinglePlayerActions(_currentState, false);
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
            if(overlapAmount > 0)
            {
                int pushback = overlapAmount / 2;
                _currentState.P1_Position -= pushback;
                _currentState.P2_Position += pushback;
            }
        }

        //2. if either character extends beyond the edge, push him out of the corner.
        //2.1 if this causes characters to overlap, push the other character back
        if (_currentState.P1_Position < ARENA_RADIUS * -1)
        {
            _currentState.P1_Position -= ARENA_RADIUS + _currentState.P1_Position;
            if (DoHitboxesOverlap(p1, p2, _currentState))
            {
                int overlapAmount = (_currentState.P1_Position + p1.Position + p1.Width / 2) - (_currentState.P2_Position + p2.Position + p2.Width / 2);
                if (overlapAmount > 0)
                {
                    _currentState.P2_Position += overlapAmount;
                }
            }
        }
        if(_currentState.P2_Position > ARENA_RADIUS)
        {
            _currentState.P2_Position -= _currentState.P2_Position - ARENA_RADIUS;
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
        foreach(Hitbox_Gameplay hg in _currentState.P1_Hitboxes)
        {
            if(hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack || hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw)
            {
                if (hg.HasStruck)
                    continue;
                foreach(Hitbox_Gameplay p2_hg in _currentState.P2_Hitboxes)
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
                            if (DoHitboxesOverlap(hg, p2_hg, _currentState))
                            { 
                                p1_throws_p2 = true;
                                hg.HasStruck = true;
                            }
                    }
                    else if(p2_hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
                    {
                        if(hg.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack)
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
                            if (DoHitboxesOverlap(p1_hg, hg, _currentState))
                            { 
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

        if(p1_hits_p2)
        {
            if(DoesPlayerBlock(false, _currentState, _p2Inputs))
            {
                //set p2 to be in blockstun
                _currentState.P2_State = GameplayEnums.CharacterState.Blockstun;
                _currentState.P2_StateFrames = 0;
                //set hitstop
                _currentState.RemainingHitstop = BLOCK_HITSTOP;
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
                _currentState.RemainingHitstop = BLOCK_HITSTOP;
            }
            else
            {
                p1_is_hit = true;
            }
        }

        if (p1_is_hit && p2_is_hit)
            res = new MatchOutcome(true, true, GameplayEnums.Outcome.Trade);
        else if (p1_is_hit)
            res = new MatchOutcome(false, true, GetOutcomeFromOpponentState(_currentState.P1_State));
        else if (p2_is_hit)
            res = new MatchOutcome(true, false, GetOutcomeFromOpponentState(_currentState.P2_State));
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
        else if(p1_throws_p2)
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
        else if(p2_throws_p1)
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
        else if(clash)
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
        if(_p1)
        {
            charState = _currentState.P1_State;
        }
        else
        {
            charState = _currentState.P2_State;
        }
        if(charState == GameplayEnums.CharacterState.Crouch || charState == GameplayEnums.CharacterState.Idle || charState == GameplayEnums.CharacterState.WalkBack)
        {
            return true;
        }
        return false;
    }

    private GameplayEnums.Outcome GetOutcomeFromOpponentState(GameplayEnums.CharacterState _opponentState)
    {
       switch(_opponentState)
        {
            case GameplayEnums.CharacterState.AttackActive:
            case GameplayEnums.CharacterState.AttackStartup:
            case GameplayEnums.CharacterState.SpecialActive:
            case GameplayEnums.CharacterState.SpecialStartup:
            case GameplayEnums.CharacterState.ThrowActive:
            case GameplayEnums.CharacterState.ThrowStartup:
                return GameplayEnums.Outcome.Counter;
            case GameplayEnums.CharacterState.AttackRecovery:
            case GameplayEnums.CharacterState.SpecialRecovery:
                return GameplayEnums.Outcome.WhiffPunish;
            case GameplayEnums.CharacterState.ThrowRecovery:
                return GameplayEnums.Outcome.Shimmy;
            default:
                return GameplayEnums.Outcome.StrayHit;
        }
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

    private MatchOutcome UpdateSinglePlayerActions(GameState _currentState, bool _p1)
    {
        GameplayEnums.CharacterState currentPlayerState;
        int stateFrames;
        List<Hitbox_Gameplay> hitboxes;
        int positionOffset = 0;
        if(_p1)
        {
            currentPlayerState = _currentState.P1_State;
            stateFrames = _currentState.P1_StateFrames;
            hitboxes = _currentState.P1_Hitboxes;
        }
        else
        {
            currentPlayerState = _currentState.P2_State;
            stateFrames = _currentState.P2_StateFrames;
            hitboxes = _currentState.P2_Hitboxes;
        }
        ++stateFrames;
        switch (currentPlayerState)
        {
            case GameplayEnums.CharacterState.AttackActive:
                if (stateFrames > ATTACK_ACTIVE)
                {
                    currentPlayerState = GameplayEnums.CharacterState.AttackRecovery;
                    stateFrames = 0;
                    hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Attack);
                    ModifyLimbHitbox(hitboxes, _p1, HURTBOX_WHIFF_EARLY);
                }
                break;
            case GameplayEnums.CharacterState.AttackRecovery:
                if(stateFrames > ATTACK_RECOVERY_TOTAL)
                {
                    currentPlayerState = GameplayEnums.CharacterState.Idle;
                    stateFrames = 0;
                    hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                }
                if(stateFrames == ATTACK_RECOVERY_SHORTEN)
                {
                    ModifyLimbHitbox(hitboxes, _p1, HURTBOX_WHIFF_LATE);
                }
                break;
            case GameplayEnums.CharacterState.AttackStartup:
                SetCharacterHurtboxStanding(hitboxes);
                if (stateFrames > ATTACK_STARTUP)
                {
                    currentPlayerState = GameplayEnums.CharacterState.AttackActive;
                    stateFrames = 0;
                    hitboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hitbox_Attack, _p1, HITBOX_ACTIVE));
                    ModifyLimbHitbox(hitboxes, _p1, HURTBOX_ACTIVE);
                }
                break;
            case GameplayEnums.CharacterState.BeingThrown:
                SetCharacterHurtboxStanding(hitboxes);
                if(stateFrames > THROW_BREAK_WINDOW)
                {
                    if (_p1)
                        return new MatchOutcome(false, true, GameplayEnums.Outcome.Throw);
                    else
                        return new MatchOutcome(true, false, GameplayEnums.Outcome.Throw);
                }
                break;//this is handled in checking if a player wins
            case GameplayEnums.CharacterState.Blockstun:
                SetCharacterHurtboxStanding(hitboxes);
                if (stateFrames < ATTACK_PUSHBACK_DURATION)
                {
                    positionOffset = ATTACK_PUSHBACK_SPEED;
                }
                if(stateFrames > ATTACK_BLOCKSTUN)
                {
                    currentPlayerState = GameplayEnums.CharacterState.Idle;
                    stateFrames = 0;
                }
                break;
            case GameplayEnums.CharacterState.Clash:
                if(stateFrames > CLASH_PUSHBACK_DURATION)
                {
                    currentPlayerState = GameplayEnums.CharacterState.Idle;
                    stateFrames = 0;
                }
                positionOffset = CLASH_PUSHBACK_SPEED;
                break;
            case GameplayEnums.CharacterState.Crouch:
                SetCharacterHurtboxCrouching(hitboxes);
                break;
            case GameplayEnums.CharacterState.Idle:
                SetCharacterHurtboxStanding(hitboxes);
                break;
            case GameplayEnums.CharacterState.Inactive:
                throw new System.Exception("character state was inactive. not supposed to happen???");
            case GameplayEnums.CharacterState.SpecialActive:
            case GameplayEnums.CharacterState.SpecialRecovery:
            case GameplayEnums.CharacterState.SpecialStartup:
                throw new System.Exception("character state was special. not implemented yet");
            case GameplayEnums.CharacterState.ThrowActive:
                if(stateFrames > THROW_ACTIVE)
                {
                    currentPlayerState = GameplayEnums.CharacterState.ThrowRecovery;
                    stateFrames = 0;
                    hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hitbox_Throw);
                    ModifyLimbHitbox(hitboxes, _p1, THROW_RECOVERY_HURTBOX);
                }
                break;
            case GameplayEnums.CharacterState.ThrowBreak:
                if(stateFrames > BREAK_DURATION)
                {
                    currentPlayerState = GameplayEnums.CharacterState.Idle;
                    stateFrames = 0;
                }
                break;
            case GameplayEnums.CharacterState.ThrowingOpponent:
                if (stateFrames > THROW_BREAK_WINDOW)
                {
                    if (_p1)
                        return new MatchOutcome(true, false, GameplayEnums.Outcome.Throw);
                    else
                        return new MatchOutcome(false, true, GameplayEnums.Outcome.Throw);
                }
                break;//this is handled in checking if a player wins
            case GameplayEnums.CharacterState.ThrowRecovery:
                if (stateFrames > THROW_RECOVERY)
                {
                    currentPlayerState = GameplayEnums.CharacterState.Idle;
                    stateFrames = 0;
                    hitboxes.RemoveAll(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
                }
                break;
            case GameplayEnums.CharacterState.ThrowStartup:
                SetCharacterHurtboxStanding(hitboxes);
                if (stateFrames > THROW_STARTUP)
                {
                    currentPlayerState = GameplayEnums.CharacterState.ThrowActive;
                    stateFrames = 0;
                    hitboxes.Add(CreateHitbox(GameplayEnums.HitboxType.Hitbox_Throw, _p1, THROW_ACTIVE_RANGE));
                    ModifyLimbHitbox(hitboxes, _p1, THROW_STARTUP_HURTBOX);
                }
                break;
            case GameplayEnums.CharacterState.WalkBack:
                SetCharacterHurtboxStanding(hitboxes);
                positionOffset = WALK_B_SPEED;
                break;
            case GameplayEnums.CharacterState.WalkForward:
                SetCharacterHurtboxStanding(hitboxes);
                positionOffset = WALK_F_SPEED;
                break;
            default:
                throw new System.Exception("Ooops looks like I forgot to handle state : " + currentPlayerState.ToString());
        }
        if (_p1)
        {
            _currentState.P1_State = currentPlayerState;
            _currentState.P1_StateFrames = stateFrames;
            _currentState.P1_Position += positionOffset;
        }
        else
        {
            _currentState.P2_State = currentPlayerState;
            _currentState.P2_StateFrames = stateFrames;
            _currentState.P2_Position -= positionOffset;
        }
        return new MatchOutcome();
    }

    private Hitbox_Gameplay CreateHitbox(GameplayEnums.HitboxType _boxType, bool _p1, int _width)
    {
        Hitbox_Gameplay box = new Hitbox_Gameplay();
        box.HitboxType = _boxType;
        box.Width = _width;
        if(_p1)
            box.Position = CHARACTER_HURTBOX_WIDTH / 2 + _width / 2;
        else
            box.Position = CHARACTER_HURTBOX_WIDTH / -2 - _width / 2;
        return box;
    }

    private void ModifyLimbHitbox(List<Hitbox_Gameplay> _hitboxes, bool _p1, int _length)
    {
        Hitbox_Gameplay hbox = _hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Limb);
        hbox.Width = _length;
        if(_p1)
        {
            hbox.Position = CHARACTER_HURTBOX_WIDTH / 2 + _length / 2;
        }
        else
        {
            hbox.Position = CHARACTER_HURTBOX_WIDTH / 2 - _length / 2;
        }
    }

    private void SetCharacterHurtboxStanding(List<Hitbox_Gameplay> _hitboxes)
    {
        Hitbox_Gameplay hbox = _hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main);
        hbox.Width = CHARACTER_HURTBOX_WIDTH;
    }

    private void SetCharacterHurtboxCrouching(List<Hitbox_Gameplay> _hitboxes)
    {
        Hitbox_Gameplay hbox = _hitboxes.Find(o => o.HitboxType == GameplayEnums.HitboxType.Hurtbox_Main);
        hbox.Width = CROUCHING_HURTBOX_WIDTH;
    }
}

public class SinglePlayerInputs
{
    //789
    //456
    //123
    public short JoystickDirection;
    public bool A, B, C, Start;
    public SinglePlayerInputs()
    {
        JoystickDirection = 5;
        A = false;
        B = false;
        C = false;
        Start = false;
    }
}

