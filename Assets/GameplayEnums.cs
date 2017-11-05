using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayEnums : MonoBehaviour {

    public enum HitboxType { Hurtbox_Main, Hurtbox_Limb, Hitbox_Attack, Hitbox_Throw}
    public enum CharacterState { Idle, Crouch, WalkBack, WalkForward, AttackStartup, AttackActive, AttackRecovery, ThrowStartup, ThrowActive, ThrowRecovery, ThrowBreak, Clash, Inactive, BeingThrown, ThrowingOpponent, Special, Blockstun}
    public enum Outcome { Throw, Counter, WhiffPunish, StrayHit, Shimmy, StillGoing, TimeOut, Trade, Sweep}
    public enum AttackAttribute { Mid, High, Low, Unblockable, TechableThrow, UntechableThrow, NotAttack}
    public enum HurtboxAttribute { Normal, Invuln, Armor}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
