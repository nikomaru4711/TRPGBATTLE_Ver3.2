public class Skill
{
    public string diceText;//��FCC<=55 �y�A�C�f�A�z
    public string actionName;//��F�v����
    public string name;//��F�A�C�f�A
    public int successNum;//��F55
    public AudioManager.Move soundType;

    public Skill(string actionName, string diceText, int successNum, AudioManager.Move soundType)
    {
        this.actionName = actionName;
        this.diceText = diceText;
        string[] parts = diceText.Split(new char[] { '�y', '�z' });
        if (parts.Length >= 2)
        {
            name = parts[1];
        }
        this.successNum = successNum;
        this.soundType = soundType;
    }
}