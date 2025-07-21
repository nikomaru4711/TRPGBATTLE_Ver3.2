public class Skill
{
    public string actionName;
    public string name;
    public int successNum;
    public AudioManager.Move soundType;

    public Skill(string actionName, string name, int successNum, AudioManager.Move soundType)
    {
        this.actionName = actionName;
        this.name = name;
        this.successNum = successNum;
        this.soundType = soundType;
    }
}

//public class 名前 : Skill
//{
//    public 名前(string name, int successNum) : base(name, successNum)
//    {

//    }
//    //機能名を記述
//}


public class FirstAid : Skill
{
    public FirstAid(string actionName, string name, int successNum, AudioManager.Move soundType) : base(actionName, name, successNum, soundType)
    {

    }
    //HPが回復する
}

public class Dodge : Skill
{
    public Dodge(string actionName, string name, int successNum, AudioManager.Move soundType) : base(actionName, name, successNum, soundType)
    {

    }
    //ダイス成功で攻撃を避ける
}