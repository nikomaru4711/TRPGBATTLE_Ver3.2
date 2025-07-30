public class Skill
{
    public string diceText;//例：CC<=55 【アイデア】
    public string actionName;//例：思いつく
    public string name;//例：アイデア
    public int successNum;//例：55
    public AudioManager.Move soundType;

    public Skill(string actionName, string diceText, int successNum, AudioManager.Move soundType)
    {
        this.actionName = actionName;
        this.diceText = diceText;
        string[] parts = diceText.Split(new char[] { '【', '】' });
        if (parts.Length >= 2)
        {
            name = parts[1];
        }
        this.successNum = successNum;
        this.soundType = soundType;
    }
}