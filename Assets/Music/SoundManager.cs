using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField]
    private AudioClip BattleTheme1;
    [SerializeField]
    private AudioClip MenuTheme1;
    

    [SerializeField]
    private AudioClip Counterhit; // ouch
    [SerializeField]
    private AudioClip Feet; // ouille
    [SerializeField]
    private AudioClip Shimmy; // ohshit
    [SerializeField]
    private AudioClip StrayHit; // oups
    [SerializeField]
    private AudioClip Throw; // bold
    [SerializeField]
    private AudioClip TimeOut; // ouf
    [SerializeField]
    private AudioClip Trade; // iiish
    [SerializeField]
    private AudioClip WhiffPunish; // nice

    private AudioSource BgmSource;
    private AudioSource VoiceSource;

    public enum BgmType { Menu, Battle, None}
    private BgmType m_currentBgmType = BgmType.None;
    private bool m_isInit = false;

    public void PlayRoundEndVoice(GameplayEnums.Outcome _outcome)
    {
        AudioClip clip = null;
        switch(_outcome)
        {
            case GameplayEnums.Outcome.Counter:
                clip = Counterhit;
                break;
            case GameplayEnums.Outcome.Shimmy:
                clip = Shimmy;
                break;
            case GameplayEnums.Outcome.StrayHit:
                clip = StrayHit;
                break;
            case GameplayEnums.Outcome.Sweep:
                clip = Feet;
                break;
            case GameplayEnums.Outcome.Throw:
                clip = Throw;
                break;
            case GameplayEnums.Outcome.TimeOut:
                clip = TimeOut;
                break;
            case GameplayEnums.Outcome.Trade:
                clip = Trade;
                break;
            case GameplayEnums.Outcome.WhiffPunish:
                clip = WhiffPunish;
                break;
        }
        if (clip != null)
        {
            VoiceSource.clip = clip;
            VoiceSource.time = 0f;
            VoiceSource.Play();
        }
    }

    public void SetGameplayAudio()
    {
        Init();
        BgmSource.clip = BattleTheme1;
        BgmSource.Play();
        BgmSource.loop = true;
        BgmSource.time = 0f;
        m_currentBgmType = BgmType.Battle;
    }

    public void SetMenuAudio()
    {
        Init();
        if (m_currentBgmType == BgmType.Menu)
            return;
        BgmSource.clip = MenuTheme1;
        BgmSource.Play();
        BgmSource.loop = true;
        BgmSource.time = 2f;
        m_currentBgmType = BgmType.Menu;
    }

    public void LowerVolume()
    {
        Init();
        BgmSource.volume = 0.5f;
    }

    public void RestoreVolume()
    {
        Init();
        BgmSource.volume = 1f;
    }


    // Use this for initialization
    void Start ()
    {
        Init();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Init()
    {
        if (m_isInit)
            return;
        m_isInit = true;
        BgmSource = Camera.main.gameObject.AddComponent<AudioSource>();
        VoiceSource = Camera.main.gameObject.AddComponent<AudioSource>();
        VoiceSource.loop = false;
        VoiceSource.playOnAwake = false;
        VoiceSource.volume = 0.4f;
    }
}

public class VoiceClips
{
    public GameplayEnums.Outcome Outcome;
    public AudioClip Clip;
}
