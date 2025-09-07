using UnityEngine;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private float _interval = 2.0f;
    private string _message = string.Empty;
    private IEnumerator _chooseEffect;
    //ダメージダイスを振って結果（int）を返す。（ログにも書き込む）
    public IEnumerator DiceRoll(int times, int upper, int multiplier = 1)
    {
        int num;
        int total = 0;
        //音を出す
        if (times == 1)
        {
            if (upper > 3)
            { _audioManager.DiceSound(AudioManager.Dice.SingleMiddle); }
            else
            { _audioManager.DiceSound(AudioManager.Dice.SingleSmall); }
        }
        else
        {
            if (upper > 3)
            { _audioManager.DiceSound(AudioManager.Dice.DoubleMiddle); }
            else
            { _audioManager.DiceSound(AudioManager.Dice.DoubleSmall); }
        }

        //ダイスを振る && メッセージの作成
        _message = times + "d" + upper + " > [";
        for (int i = 0; i < times; i++)
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
        if (multiplier != 1)
        {
            total *= multiplier;
            _message += "] * " + multiplier + " > " + total;
        }
        else
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
        //音を出す
        _audioManager.DiceSound(AudioManager.Dice.SingleBig);

        if (total <= successupper)
        {
            ////////////////
            //決定的成功！//
            ////////////////
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
                    character.criticalState = GameManager.CriticalState.DoubleDamage;
                }
                else
                {
                    _uiManager.CreateLog("次の攻撃は回避されない", UIManager.Line.Line1);
                    character.criticalState = GameManager.CriticalState.Unavoidable;
                }
                yield return new WaitForSeconds(_interval);
                yield return GameManager.DiceState.Critical;
            }
            else//成功//
            {
                _message = "1d100<=" + successupper + skill + "\n<color=blue>(1d100<=" + successupper + ") > " + total + " > \n成功</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                yield return GameManager.DiceState.Success;
            }
        }
        else
        {
            ////////////////
            //致命的失敗!!//
            ////////////////
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
                    character.fambleState = GameManager.FambleState.DoubleDamage;

                }
                else
                {
                    _uiManager.CreateLog("次の攻撃は回避不可. . .", UIManager.Line.Line1);
                    character.fambleState = GameManager.FambleState.Unavoidable;
                }
                yield return new WaitForSeconds(_interval);
                yield return GameManager.DiceState.Famble;
            }
            else//失敗//
            {
                _message = "1d100<=" + successupper + skill + "\n<color=red>(1d100<=" + successupper + ") > " + total + " > \n失敗</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                yield return GameManager.DiceState.Fail;
            }
        }
    }
    //以下使用例
    //    DiceRoll(50, "【知識】");
    //　→1d100<=50【知識】(1d100<=50) > 27 > 成功
}
