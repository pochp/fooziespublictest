using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayEnums : MonoBehaviour {

    public enum HitboxType { Hurtbox_Main, Hurtbox_Limb, Hitbox_Attack, Hitbox_Throw}
    public enum CharacterState { Idle, Crouch, WalkBack, WalkForward, AttackStartup, AttackActive, AttackRecovery, ThrowStartup, ThrowActive, ThrowRecovery, ThrowBreak, Clash, Inactive, BeingThrown, ThrowingOpponent, SpecialStartup, SpecialActive, SpecialRecovery, Blockstun}
    public enum Outcome { Throw, Counter, WhiffPunish, StrayHit, Shimmy, StillGoing, TimeOut, Trade}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
