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

//public class ���O : Skill
//{
//    public ���O(string name, int successNum) : base(name, successNum)
//    {

//    }
//    //�@�\�����L�q
//}


public class FirstAid : Skill
{
    public FirstAid(string actionName, string name, int successNum, AudioManager.Move soundType) : base(actionName, name, successNum, soundType)
    {

    }
    //HP���񕜂���
}

public class Dodge : Skill
{
    public Dodge(string actionName, string name, int successNum, AudioManager.Move soundType) : base(actionName, name, successNum, soundType)
    {

    }
    //�_�C�X�����ōU���������
}