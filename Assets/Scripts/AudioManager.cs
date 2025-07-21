using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //音声
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioClip _criticalSound;
    [SerializeField] private AudioClip _criticalSound2;
    [SerializeField] private AudioClip _fambleSound;
    [SerializeField] private AudioClip _bigDice;
    [SerializeField] private AudioClip _middleDice;
    [SerializeField] private AudioClip _middleDice2;
    [SerializeField] private AudioClip _smallDice;
    [SerializeField] private AudioClip _smallDice2;
    [SerializeField] private AudioClip _panch;
    [SerializeField] private AudioClip _panch2;
    [SerializeField] private AudioClip _knife;
    [SerializeField] private AudioClip _throw;
    [SerializeField] private AudioClip _firstAid;
    [SerializeField] private AudioClip _dodged;
    [SerializeField] private AudioClip _dodged2;
    [SerializeField] private AudioClip _hunmer;
    [SerializeField] private AudioClip _skeletonDead;
    [SerializeField] private AudioClip _clearBGM;
    [SerializeField] private AudioClip _gameOverBGM;

    //ダイス音声選択用Enum
    public enum Dice
    {
        SingleSmall,
        DoubleSmall,
        SingleMiddle,
        DoubleMiddle,
        SingleBig,
        Critical,
        Famble,
    }

    public enum Move
    {
        Panch,
        Knife,
        Throw, 
        Hunmer,
        FirstAid,
        Dodge,
    }

    public enum BGMType
    {
        Clear,
        GameOver,
    }

    private void Start()
    {
        _bgm.Play();
    }

    public void ChangeBGM(BGMType newbgm)
    {
        _bgm.Stop();
        //新しいBGMを設定
        switch (newbgm)
        {
            case BGMType.Clear:
                _bgm.resource = _clearBGM;
                break;
            case BGMType.GameOver:
                _bgm.resource = _gameOverBGM;
                break;
        }
        _bgm.Play();
        //あたらしいBGMをフェードイン
    }

    public void DiceSound(Dice dice)
    {
        switch(dice)
        {
            case Dice.SingleSmall:
                _audioSource.PlayOneShot(_smallDice);
                break;
            case Dice.DoubleSmall:
                _audioSource.PlayOneShot(_smallDice2);
                break;
            case Dice.SingleMiddle:
                _audioSource.PlayOneShot(_middleDice);
                break;
            case Dice.DoubleMiddle:
                _audioSource.PlayOneShot(_middleDice2);
                break;
            case Dice.SingleBig:
                _audioSource.PlayOneShot(_bigDice);
                break;
            case Dice.Critical:
                if (Random.Range(0,2) ==0)
                {
                    _audioSource.PlayOneShot(_criticalSound);
                } else
                {
                    _audioSource.PlayOneShot(_criticalSound2);
                }
                    break;
            case Dice.Famble:
                _audioSource.PlayOneShot(_fambleSound);
                break;


        }
    }
    public void MoveSound(Move move)
    {

        switch (move)
        {
            case Move.Panch:
                if (Random.Range(0, 2) == 0)
                {
                    _audioSource.PlayOneShot(_panch);
                }
                else
                {
                    _audioSource.PlayOneShot(_panch2);
                }
                break;
            case Move.Knife:
                _audioSource.PlayOneShot(_knife);
                break;
            case Move.Throw:
                _audioSource.PlayOneShot(_throw);
                break;
            case Move.Hunmer:
                _audioSource.PlayOneShot(_hunmer);
                break;
            case Move.FirstAid:
                _audioSource.PlayOneShot(_firstAid);
                break;
            case Move.Dodge://2
                if (Random.Range(0, 2) == 0)
                {
                    _audioSource.PlayOneShot(_dodged);
                }
                else
                {
                    _audioSource.PlayOneShot(_dodged2);
                }
                break;
        }
    }
    public void EnemyDeadSound()
    {
        _audioSource.PlayOneShot(_skeletonDead);
    }
}
