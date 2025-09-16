using System.Collections;
using UnityEngine;
using static UIManager;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private DiceRoller _diceRoller;
    [SerializeField] private AudioManager _audioManager;

    private bool _isAvoidable;
    private int _damageMultiplier;
    private Skill _skill;
    private IEnumerator _damage;
    private IEnumerator _avoidState;
    private IEnumerator _diceState;
    //攻撃の処理を行う。引数の代入方法を要件等
    //※引数に関して、必要なのは下記の者であるがWeapon型の関数から呼び出すので渡す方法がない。
    //→、アタッカー、ディフェンダーをGameManagerで管理して、そこから参照するのがよさそう。
    public IEnumerator AttackManage(Weapon weapon)
    {
        //状態の代入
        _isAvoidable = weapon.avoidable;
        _damageMultiplier = 1;

        //相手のファンブルは有効か？
        switch (_gameManager._dfner.fambleState)
        {
            case GameManager.FambleState.None:
                break;
            case GameManager.FambleState.Unavoidable:
                _isAvoidable = false;
                break;
            case GameManager.FambleState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking target \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }
        _gameManager._dfner.fambleState = GameManager.FambleState.None;

        //Attacjerのクリティカル状況を確認（ダメージ二倍のみ）
        switch (_gameManager._atker.criticalState)
        {
            case GameManager.CriticalState.None:
                break;
            case GameManager.CriticalState.Unavoidable:
                _isAvoidable = false;
                break;
            case GameManager.CriticalState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking attcker \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }

        //技能ダイスを振る。成功したら次へ（この時振ったダイスのクリティカル、ファンブルチェック）
        _diceState = _diceRoller.DiceRoll(weapon.successNum, "【" + weapon.actionName + "】", _gameManager._atker);
        yield return _diceState;
        //攻撃できたか？
        if ((GameManager.DiceState)_diceState.Current == GameManager.DiceState.Success || (GameManager.DiceState)_diceState.Current == GameManager.DiceState.Critical)
        {
            //クリティカルか？これもタイミングはおかしくなってるけど一旦省略。
            //（今の状態だと必ず次ターンの攻撃に影響。）


            //武器が回避可能か？
            if (_isAvoidable)
            {
                //敵は回避できる状況か？（ファンブル効果）
                if (_gameManager._dfner.fambleState != GameManager.FambleState.Unavoidable)
                {
                    //回避のスキル情報を取り出す
                    _skill = GetSkill(_gameManager._dfner, "回避");
                    Debug.Log(_skill);
                    Debug.Log(_gameManager._dfner.Cname);
                    //相手の回避ダイス
                    Debug.LogFormat("_dfner:{0}\nnum：{1}", _gameManager._dfner.Cname,_skill.successNum);
                    _avoidState = _diceRoller.DiceRoll(_skill.successNum, "【回避】", _gameManager._dfner);
                    yield return _avoidState;
                    if ((GameManager.DiceState)_avoidState.Current == GameManager.DiceState.Success || (GameManager.DiceState)_avoidState.Current == GameManager.DiceState.Critical)
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
            Debug.Log("ダメージダイスを振ります");
            _damage = _diceRoller.DiceRoll(weapon.diceNum, weapon.damageNum, _damageMultiplier);
            yield return _damage;
            _audioManager.MoveSound(weapon.soundType);
            int oldHP = _gameManager._dfner.currentHP;
            _gameManager._dfner.currentHP -= (int)_damage.Current;
            if (_gameManager._dfner.currentHP <= 0) { _gameManager._dfner.currentHP = 0; }
            _uiManager.UpdateCharacterUI(_gameManager._dfner);
            _uiManager.CreateLog("【" + _gameManager._dfner.Cname + "】\nHP : " + oldHP + "→" + _gameManager._dfner.currentHP, Line.Line2);
            yield return new WaitForSeconds(1.5f);
        }
        _gameManager._state = GameManager.SystemState.None;
        yield return null;
    }

    public Skill GetSkill(Character character, string name)
    {

        Debug.LogFormat("{0}の【{1}】を探します。", character.Cname, name);
            foreach(Skill skill in character.skills)
            {
                Debug.Log(skill.actionName);
                if(skill.actionName == name) { return skill; }
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
        if ((GameManager.DiceState)_diceState.Current == GameManager.DiceState.Success || (GameManager.DiceState)_diceState.Current == GameManager.DiceState.Critical)
        {
            Debug.LogFormat("【{0}】を実行します",skill.name);
            yield return skill.Move(_gameManager._atker, _uiManager, _diceRoller, _audioManager);
            yield return new WaitForSeconds(3.0f);
        }
        Debug.Log("行動終了.");
        _gameManager._state = GameManager.SystemState.None;
        yield return null;
    }
}
