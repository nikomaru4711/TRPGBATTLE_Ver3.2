using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UIManager;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private NewGameManager _gameManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private DiceRoller _diceRoller;
    [SerializeField] private AudioManager _audioManager;

    private bool _isAvoidable;
    private int _damageMultiplier;
    private Skill _skill;
    private IEnumerator _damage;
    private IEnumerator _avoidState;
    private IEnumerator _diceState;
    private IEnumerator _num;
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
        switch (_gameManager._dfner.fambleState)
        {
            case NewGameManager.FambleState.None:
                break;
            case NewGameManager.FambleState.Unavoidable:
                _isAvoidable = false;
                break;
            case NewGameManager.FambleState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking target \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }
        _gameManager._dfner.fambleState = NewGameManager.FambleState.None;

        //Attacjerのクリティカル状況を確認（ダメージ二倍のみ）
        switch (_gameManager._atker.criticalState)
        {
            case NewGameManager.CriticalState.None:
                break;
            case NewGameManager.CriticalState.Unavoidable:
                _isAvoidable = false;
                break;
            case NewGameManager.CriticalState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking attcker \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }

        //技能ダイスを振る。成功したら次へ（この時振ったダイスのクリティカル、ファンブルチェック）
        _diceState = _diceRoller.DiceRoll(weapon.successNum, "【" + weapon.name + "】", _gameManager._atker);
        yield return _diceState;
        //攻撃できたか？
        if ((NewGameManager.DiceState)_diceState.Current == NewGameManager.DiceState.Success || (NewGameManager.DiceState)_diceState.Current == NewGameManager.DiceState.Critical)
        {
            //クリティカルか？これもタイミングはおかしくなってるけど一旦省略。
            //（今の状態だと必ず次ターンの攻撃に影響。）


            //武器が回避可能か？
            if (_isAvoidable)
            {
                //敵は回避できる状況か？（ファンブル効果）
                if (_gameManager._dfner.fambleState != NewGameManager.FambleState.Unavoidable)
                {
                    //回避のスキル情報を取り出す
                    _skill = GetSkill(_gameManager._dfner, "避ける");
                    //相手の回避ダイス
                    _avoidState = _diceRoller.DiceRoll(_skill.successNum, "【回避】", _gameManager._dfner);
                    yield return _avoidState;
                    if ((NewGameManager.DiceState)_avoidState.Current == NewGameManager.DiceState.Success || (NewGameManager.DiceState)_avoidState.Current == NewGameManager.DiceState.Critical)
                    { _audioManager.MoveSound(_skill.soundType); }
                    yield return new WaitForSeconds(1f);
                    //ここで回避がクリティカルの場合を追記する。今回は回避のCF同じにするので省略。
                }
                else
                {
                    _uiManager.CreateLog("[ファンブル効果] 回避不可！", UIManager.Line.Line1);
                }

            }

            //ダメージ計算
            if (!weapon.avoidable || (NewGameManager.DiceState)_avoidState.Current == NewGameManager.DiceState.Fail || (NewGameManager.DiceState)_avoidState.Current == NewGameManager.DiceState.Famble)
            {
                Debug.Log("ダメージダイスを振ります");
                _damage = _diceRoller.DiceRoll(weapon.diceNum, weapon.damageNum, _damageMultiplier);
                yield return _damage;
                _audioManager.MoveSound(weapon.soundType);
                int oldHP = _gameManager._dfner.currentHP;
                _gameManager._dfner.currentHP -= (int)_damage.Current;
                if (_gameManager._dfner.currentHP <= 0) { _gameManager._dfner.currentHP = 0; }
                _uiManager.UpdateCharacterUI(_gameManager._dfner);
                _uiManager.CreateLog("【" + _gameManager._dfner.name + "】HP : " + oldHP + "→" + _gameManager._dfner.currentHP, Line.Line1);
                yield return new WaitForSeconds(1.5f);
            }
        }
        _gameManager._state = NewGameManager.SystemState.None;
        yield break;
    }

    public Skill GetSkill(Character character, string skillName)
    {
        Debug.LogFormat("Input：{0}", skillName);
        if (character.kind == NewGameManager.CharacterKind.Player)
        {
            foreach(Skill skill in character.skills)
            {
                if(skill.actionName == skillName) { return skill; }
            }
        }
        _uiManager.CreateLog("<color=red>Error!</color>SearchingSkill Failed", UIManager.Line.Line1);
        return null;
    }

    public IEnumerator MoveManage(Skill skill)
    {//行動処理全般をここで行う
        Debug.LogFormat("{0}をしました！", skill.diceText);
        //技能ダイスを振る。成功したら次へ（この時振ったダイスのクリティカル、ファンブルチェック）
        _diceState = _diceRoller.DiceRoll(skill.successNum, "【" + skill.name + "】", _gameManager._atker);
        yield return _diceState;

        //ダイスに成功したか？
        if ((NewGameManager.DiceState)_diceState.Current == NewGameManager.DiceState.Success || (NewGameManager.DiceState)_diceState.Current == NewGameManager.DiceState.Critical)
        {
            ////////////////////////////////////////////////////////////////////////////////////////
            //応急手当の場合
            ////////////////////////////////////////////////////////////////////////////////////////
            if (skill.name == "応急手当")
            {
                _num = _diceRoller.DiceRoll(1, 3);
                yield return _num;
                if (_gameManager._atker.currentHP == _gameManager._atker.maxHP)
                {
                    _uiManager.CreateLog("これ以上回復しない\n（傷がない）", UIManager.Line.Line2);
                }
                else
                {
                    _uiManager.UpdateCharacterHP(_gameManager._atker, (int)_num.Current);
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////

            _audioManager.MoveSound(skill.soundType);
            yield return new WaitForSeconds(3.0f);
        }
        _gameManager._state = NewGameManager.SystemState.None;
        yield break;
    }
}
