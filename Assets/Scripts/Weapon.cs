using UnityEngine;

public class Weapon
{
    public string actionName;
    public string name;
    public int successNum;
    public int diceNum;
    public int damageNum;
    public bool continuousDamage;
    public bool avoidable = true;
    public AudioManager.Move soundType;

    public Weapon(string actionName, string name, int successNum, int diceNum, int damageNum, bool countinuousDamage, AudioManager.Move soundType)
    {
        this.actionName = actionName;
        this.name = name;
        this.successNum = successNum;
        this.diceNum = diceNum;
        this.damageNum = damageNum;
        this.continuousDamage = countinuousDamage;
        this.soundType = soundType;
    }
}
/*
 * クラスの継承を用いてDBや継続ダメージ処理を記載。
 * 今、継続ダメージの処理に迷ってる。
 * →相手に継続ダメージのbool作ってそれがtrueの間ダメージ...でいいか！
 */

//public class 名前 : Weapon
//{
//    public 名前(string actionName, string name, int successNum, int diceNum, int damageNum, bool countinuousDamage) : base(actionName, name, successNum, diceNum, damageNum, continuousDamage)
//    {

//    }
//    //機能名を記述
//}

public class ThrowTorch : Weapon
{
    public ThrowTorch(string actionName, string name, int successNum, int diceNum, int damageNum, bool countinuousDamage, AudioManager.Move soundType) : base(actionName, name, successNum, diceNum, damageNum, countinuousDamage, soundType)
    {

    }
    //DBがハーフ。燃焼アリ。
}