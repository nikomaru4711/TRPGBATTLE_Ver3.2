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
 * �N���X�̌p����p����DB��p���_���[�W�������L�ځB
 * ���A�p���_���[�W�̏����ɖ����Ă�B
 * ������Ɍp���_���[�W��bool����Ă��ꂪtrue�̊ԃ_���[�W...�ł������I
 */

//public class ���O : Weapon
//{
//    public ���O(string actionName, string name, int successNum, int diceNum, int damageNum, bool countinuousDamage) : base(actionName, name, successNum, diceNum, damageNum, continuousDamage)
//    {

//    }
//    //�@�\�����L�q
//}

public class ThrowTorch : Weapon
{
    public ThrowTorch(string actionName, string name, int successNum, int diceNum, int damageNum, bool countinuousDamage, AudioManager.Move soundType) : base(actionName, name, successNum, diceNum, damageNum, countinuousDamage, soundType)
    {

    }
    //DB���n�[�t�B�R�ăA���B
}