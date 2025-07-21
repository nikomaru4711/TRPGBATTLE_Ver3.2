using JetBrains.Annotations;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using static AudioManager;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public enum CriticalState
    {
        None,
        DoubleDamage,
        Unavoidable,
    }
    public enum FambleState
    {
        None,
        DoubleDamage,
        Unavoidable,
    }
    public enum DiceState
    {
        Success,
        Fail,
        Critical,
        Famble,
        None
    }

    public enum MoveState
    {
        Fight,
        Act
    }

    public enum CharacterKind
    {
        Player,
        Enemy
    }

    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
    }

    //スクリプトのインポート
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;

    //初期のデータ取り込み
    private int[] data = new int[6];

    //各キャラクター
    [System.NonSerialized]public Character _player1;
    [System.NonSerialized] public Character _enemy1;
    [System.NonSerialized] public Weapon[] _weapon1 = new Weapon[10];
    [System.NonSerialized] public Skill[] _skill1 = new Skill[10];
    [System.NonSerialized] public Weapon[] _weapon2 = new Weapon[10];
    [System.NonSerialized] public Skill[] _skill2 = new Skill[10];

    private int i;
    private int _damageMultiplier = 1;
    private bool _isAvoidable;
    private int _index;
    private GameObject _obj;
    private TurnState _turnState;
    private IEnumerator _damage;
    private IEnumerator _num;
    private IEnumerator _actNum;
    private IEnumerator _chooseEffect;
    private string _message;
    public float _interval = 2.0f;
    IEnumerator _diceState;
    IEnumerator _avoidState;
    //DiceState _diceState = DiceState.None;
    //DiceState _avoidState = DiceState.None;
    //キャラクターや攻撃、行動などのアクションを追加する度にこれらの値もインクリメントしていく。
    private int _characternum = 0;
    private int _weaponIndex = 3;
    private int _skillIndex = 2;
    private int _enemyWeaponIndex = 2;
    private int _enemySkillIndex = 2;
    private int _round = 0;
    private int _turn = 0;

    //攻撃者と攻撃対象者を設定する
    [System.NonSerialized] public Character _attacker;
    [System.NonSerialized] public Character _defender;
    [System.NonSerialized] public Character _temp;

    //キャラクター一覧の格納配列
    public Character[] _characterArray = new Character[10];


    private void Start()
    {
        //タイトルでの入力データのインポート
        ImportFromTitle(ref data);
        //敵の元データの作成
        _weapon2[0] = new Weapon("殴る", "こぶし", 80, 1, 3, false, Move.Panch);
        _weapon2[1] = new Weapon("ハンマーで攻撃", "ハンマー", 65, 1, 8, false, Move.Hunmer);
        _skill2[0] = new Skill("修復", "応急手当", 30, Move.FirstAid);
        _skill2[1] = new Skill("避ける" , "回避", 25, Move.Dodge);


        //プレイヤーの作成
        //_player1 = new Character(1, "探索者", TitleSceneManager._playerStatus[0], TitleSceneManager._playerStatus[1], null, true, _weapon1, _skill1, CharacterKind.Player);
        SetCharacterAray(_player1);
        //プレイヤーの技能のUIButton生成
        _uiManager.CreateIcon(_player1);
        for (i = 0; i < _weaponIndex; i++)
        {
            _uiManager.CreateButton(_weapon1[i]);
        }
        for (i = 0; i < _skillIndex; i++)
        {
            _uiManager.CreateButton(_skill1[i]);
        }
        //ここで個別Panelとか作れたら複数PCいけるのになぁ（願望）
        //敵の作成
        _enemy1 = new Character(2, "スケルトン", 14, 13, "Enemy_Icon", true, _weapon2, _skill2, CharacterKind.Enemy);
        SetCharacterAray(_enemy1);
        //敵のUI反映
        _uiManager.CreateIcon(_enemy1);
        _uiManager.CreateEnemyAppearance(_enemy1);
        
        //攻撃順序の設定。今回はタイマンなので設定の仕方が特殊。
        SetAttackOrder();
        //UIにラウンド表示
        RoundManage();
    }
    //場にいる（一度でも）全てのキャラクターが入っている配列
    public void SetCharacterAray(Character character)
    {
        _characterArray[_characternum] = character;
        _characternum++;
    }

    //攻撃順序の設定をする
    public void SetAttackOrder()
    {
        for(i = 0; i < _characternum; i++)
        {
            if (_characterArray[i].id == _uiManager._displayOrder[0]._id)
            {
                _attacker = _characterArray[i];
            } else if (_characterArray[i].id == _uiManager._displayOrder[1]._id)
            {
                _defender = _characterArray[i];
            }
        }
        if(_attacker.kind == CharacterKind.Player)
        {
            _turnState = TurnState.PlayerTurn;
        } else
        {
            _turnState = TurnState.EnemyTurn;
            StartCoroutine("EnemyManage"); 
            _uiManager.IsInteractable(false);
        }
    }

    private void Update()
    {//戦闘ターンを管理。実際には動かさないけど手入れする場合はここでする。
        switch (_turnState)
        {
            case TurnState.PlayerTurn:
                break;
            case TurnState.EnemyTurn:
                break;
        }
    }

    public void RoundManage()
    {
        _turn++;
        //ラウンドの表記
        if (_turn % 2 == 1)
        {
            _round++;
            _uiManager.CreateLog("--Round" + _round + "------------", UIManager.Line.Line1, 55);
        }
        //ターンの表記
        switch (_turnState)
        {
            case TurnState.PlayerTurn:
                _uiManager.CreateLog("ー探索者のターンー", UIManager.Line.Line1, 45);
                break;
            case TurnState.EnemyTurn:
                _uiManager.CreateLog("ー敵のターンー", UIManager.Line.Line1, 45);
                break;
        }
        
    }
    //敵の行動処理から攻撃まで。
    public IEnumerator EnemyManage()
    {
        Debug.Log("敵が動くよ！");
        //どの攻撃を振るのかダイスを振る。
        yield return new WaitForEndOfFrame();
        _actNum = DiceRoll(1, 2);
        yield return _actNum;
        if((int)_actNum.Current == 1)
        {
            _index = GetWeaponIndex(_attacker, "殴る");
        } else
        {
            _index = GetWeaponIndex(_attacker, "ハンマーで攻撃");
        }
        StartCoroutine("AttackManage", _attacker.weapons[_index]);
    }

    //GetSkillIndex(Character character, string skillName)


    //攻撃の処理を行う。引数の代入方法を要件等
    //※引数に関して、必要なのは下記の者であるがWeapon型の関数から呼び出すので渡す方法がない。
    //→、アタッカー、ディフェンダーをGameManagerで管理して、そこから参照するのがよさそう。
    public IEnumerator AttackManage(Weapon weapon)
    {
        Debug.LogFormat("テスト：{0}で攻撃をしました！", weapon.name);
        //状態の代入
        _isAvoidable = weapon.avoidable;
        _damageMultiplier = 1;

        //相手のファンブルは有効か？
        switch (_defender.fambleState)
        {
            case FambleState.None:
                break;
            case FambleState.Unavoidable:
                _isAvoidable = false;
                break;
            case FambleState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking target \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }
        _defender.fambleState = FambleState.None;

        //Attacjerのクリティカル状況を確認（ダメージ二倍のみ）
        switch(_attacker.criticalState)
        {
            case CriticalState.None:
                break;
            case CriticalState.Unavoidable:
                _isAvoidable = false;
                break;
            case CriticalState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking attcker \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }

        //技能ダイスを振る。成功したら次へ（この時振ったダイスのクリティカル、ファンブルチェック）
        _diceState = DiceRoll(weapon.successNum, "【" + weapon.name + "】", _attacker);
        yield return _diceState;
        //攻撃できたか？
        if ((DiceState)_diceState.Current == DiceState.Success || (DiceState)_diceState.Current == DiceState.Critical)
        {
            //クリティカルか？これもタイミングはおかしくなってるけど一旦省略。
            //（今の状態だと必ず次ターンの攻撃に影響。）
            

            //武器が回避可能か？
            if (weapon.avoidable)
            {
                //敵は回避できる状況か？（ファンブル効果）
                if(_defender.fambleState != FambleState.Unavoidable)
                {
                    //回避のスキル情報を取り出す
                    _index = GetSkillIndex(_defender, "避ける");
                    //相手の回避ダイス
                    _avoidState = DiceRoll(_defender.skills[_index].successNum, "【回避】", _defender);
                    yield return _avoidState;
                    if ((DiceState)_avoidState.Current == DiceState.Success || (DiceState)_avoidState.Current == DiceState.Critical)
                    { _audioManager.MoveSound(_defender.skills[_index].soundType); }
                    yield return new WaitForSeconds(1f);
                    //ここで回避がクリティカルの場合を追記する。今回は回避のCF同じにするので省略。
                } else
                {
                    _uiManager.CreateLog("[ファンブル効果] 回避不可！",UIManager.Line.Line1);
                }

            }

            //ダメージ計算
            if (!weapon.avoidable || (DiceState)_avoidState.Current == DiceState.Fail || (DiceState)_avoidState.Current == DiceState.Famble)
            {
                Debug.Log("ダメージダイスを振ります");
                _damage = DiceRoll(weapon.diceNum, weapon.damageNum, _damageMultiplier);
                yield return _damage;
                _audioManager.MoveSound(weapon.soundType);
                _uiManager.UpdateCharacterHP(_defender, -(int)_damage.Current);
                yield return new WaitForSeconds(1.5f);
            }
        }
        Debug.Log("次のターンへ行きます");
        //次のターンへ
        ChangeTurn();
        yield break;
    }

    //ACTボタンの行動処理はすべてここで行う。
    public IEnumerator MoveManage(Skill skill)
    {//行動処理全般をここで行う
        Debug.LogFormat("{0}をしました！",skill.name);
        //技能ダイスを振る。成功したら次へ（この時振ったダイスのクリティカル、ファンブルチェック）
        _diceState = DiceRoll(skill.successNum, "【" + skill.name + "】", _attacker);
        yield return _diceState;

        //ダイスに成功したか？
        if ((DiceState)_diceState.Current == DiceState.Success || (DiceState)_diceState.Current == DiceState.Critical)
        {
            //応急手当の場合
            if(skill.name == "応急手当")
            {
                _num = DiceRoll(1, 3);
                yield return _num;
                if(_attacker.currentHP == _attacker.maxHP)
                {
                    _uiManager.CreateLog("これ以上回復しない\n（傷がない）", UIManager.Line.Line2);
                } else
                {
                    _uiManager.UpdateCharacterHP(_attacker, (int)_num.Current);
                }
            }
            _audioManager.MoveSound(skill.soundType);
            yield return new WaitForSeconds(3.0f);
        }

        //次のターンへ
        ChangeTurn();
        yield break;
    }

    //キャラクターの特定のスキルを取り出す。
    public int GetSkillIndex(Character character, string skillName)
    {
        Debug.LogFormat("Input：{0}",skillName);
        if (character.kind == CharacterKind.Player)
        {
            for (i = 0; i < _skillIndex; i++)
            {
                Debug.LogFormat("{0}", i);
                Debug.LogFormat("skills[{0}]：{1}", i, character.skills[i].actionName);
                if (character.skills[i].actionName == skillName)
                {
                    return i;
                }
                Debug.LogFormat("{0}", i);
            }
        } else
        {
            for (i = 0; i < _enemySkillIndex; i++)
            {
                if (character.skills[i].actionName == skillName)
                {
                    return i;
                }
            }
        }
        _uiManager.CreateLog("<color=red>Error!</color>SearchingSkill Failed", UIManager.Line.Line1);
        return -1;
    }
    public int GetWeaponIndex(Character character, string weaponName)
    {
        if (character.kind == CharacterKind.Player)
        {
            for (i = 0; i < _weaponIndex; i++)
            {
                
                if (character.weapons[i].actionName == weaponName)
                {
                    return i;
                }
            }
        }
        else
        {
            for (i = 0; i < _enemyWeaponIndex; i++)
            {
                if (character.weapons[i].actionName == weaponName)
                {
                    return i;
                }
            }
        }
        _uiManager.CreateLog("<color=red>Error!</color>SearchingWeapon Failed", UIManager.Line.Line1);
        return -1;
    }

    //誰が攻撃をするのかを決める。
    public void ChangeTurn()
    {
        _temp = _attacker;
        _attacker = _defender;
        _defender = _temp;
        switch (_turnState)
        {
            case TurnState.PlayerTurn:
                _turnState = TurnState.EnemyTurn;
                //もし敵が生きていたら...
                if(_attacker.isDead)
                {
                    RoundManage();
                    StartCoroutine("EnemyManage");
                }
                break;
            case TurnState.EnemyTurn:
                //もしPlayerが生きていたら...
                if (_attacker.isDead)
                {
                    _turnState = TurnState.PlayerTurn;
                    RoundManage();
                    _uiManager.IsInteractable(true);
                }
                break;
        }
    }

    //攻撃する対象を決定する。このverではタイマンなので未設定。
    public void DecideTarget()
    {

    }

    //Titleシーンの入力データを持ってきて代入
    private void ImportFromTitle(ref int[] data)
    {
        /*グローバル変数配列には以下のように格納されている
         * 0:HP
         * 1:DEX
         * 2:応急手当
         * 3:こぶし
         * 4:投擲
         * 5:回避
         */
        //_weapon1[0] = new Weapon("ナイフで刺す", "こぶし", TitleSceneManager._playerStatus[3], 1, 6, false, Move.Knife);
        //_weapon1[1] = new ThrowTorch("松明を投げる", "投擲", TitleSceneManager._playerStatus[4], 1, 3, true, Move.Throw);
        //_weapon1[2] = new Weapon("殴る", "こぶし", TitleSceneManager._playerStatus[3], 1, 3, false, Move.Panch);
        //_skill1[0] = new FirstAid("応急処置を施す", "応急手当", TitleSceneManager._playerStatus[2], Move.FirstAid);
        //_skill1[1] = new Dodge("避ける", "回避", TitleSceneManager._playerStatus[5], Move.Dodge);
    }

    //ダメージダイスを振って結果を返す。（ログにも書き込む）
    public IEnumerator DiceRoll(int times, int upper, int multiplier = 1)
    {
        int num;
        int total = 0;
        if(times == 1)
        {
            if (upper > 3)
            { _audioManager.DiceSound(Dice.SingleMiddle); }
            else
            { _audioManager.DiceSound(Dice.SingleSmall); }
        } else
        {
            if (upper > 3)
            { _audioManager.DiceSound(Dice.DoubleMiddle); }
            else
            { _audioManager.DiceSound(Dice.DoubleSmall); }
        }


            _message = times + "d" + upper + " > [";
        for (i = 0; i < times; i++)
        {
            num = Random.Range(1, upper + 1);
            total += num;
            if (i == 0)
            {
                _message += num;
            }
            else
            {
                _message += "," + num;

            }
        }
        if(multiplier != 1)
        {
            total *= multiplier;
            _message += "] * " + multiplier + " > " + total;
        } else
        {
            _message += "] > " + total;
        }
        _uiManager.CreateLog(_message, UIManager.Line.Line1);
        yield return new WaitForSeconds(_interval);
        yield return total;
    }
    //以下使用例
    //　　DiceRoll(2,4);
    //　→2d4 > [1,3] > 4

    //100面ダイスを振ってEnumで結果を返す。（ログにも書き込む）
    public IEnumerator DiceRoll(int successupper, string skill, Character character)
    {
        int total = Random.Range(1, 101);
        _audioManager.DiceSound(Dice.SingleBig);
        if (total <= successupper)
        {
            if (total <= 5)
            {
                _message = "1d100<=" + successupper + skill + "\n<color=blue>(1d100<=" + successupper + ") > " + total + " > \n決定的成功/スペシャル</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                _chooseEffect = DiceRoll(1, 2);
                yield return _chooseEffect;
                if ((int)_chooseEffect.Current == 1)
                {
                    _uiManager.CreateLog("次の与ダメージが二倍！", UIManager.Line.Line1);
                    character.criticalState = CriticalState.DoubleDamage;
                }
                else
                {
                    _uiManager.CreateLog("次の攻撃は回避されない", UIManager.Line.Line1);
                    character.criticalState = CriticalState.Unavoidable;
                }
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Critical;
            } else
            {
                _message = "1d100<=" + successupper + skill + "\n<color=blue>(1d100<=" + successupper + ") > " + total + " > \n成功</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Success;
            }
        }
        else
        {
            if (total >= 96)
            {
                _message = "1d100<=" + successupper + skill + "\n<color=red>(1d100<=" + successupper + ") > " + total + " > \n致命的失敗</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                _chooseEffect = DiceRoll(1, 2);
                yield return _chooseEffect;
                if ((int)_chooseEffect.Current == 1)
                {
                    _uiManager.CreateLog("次の被ダメージ二倍に. . .", UIManager.Line.Line1);
                    character.fambleState = FambleState.DoubleDamage;

                }
                else
                {
                    _uiManager.CreateLog("次の攻撃は回避不可. . .", UIManager.Line.Line1);
                    character.fambleState = FambleState.Unavoidable;
                }
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Famble;
            } else
            {
                _message = "1d100<=" + successupper + skill + "\n<color=red>(1d100<=" + successupper + ") > " + total + " > \n失敗</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Fail;
            }
        }   
    }
    //以下使用例
    //    DiceRoll(50, "【知識】");
    //　→1d100<=50【知識】(1d100<=50) > 27 > 成功
}
