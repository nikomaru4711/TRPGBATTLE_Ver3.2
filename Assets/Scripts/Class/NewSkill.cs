public class NewSkill
{
    public string diceText;//例：CC<=55 【アイデア】
    public int successNum;//例：55

    public NewSkill(string diceText, int successNum)
    {
        this.diceText = diceText;
        this.successNum = successNum;
    }
}