using System.Collections.Generic;
public class Character
{
    public int id;
    public string Cname;
    public int currentHP;
    public int maxHP;
    public int dex;
    public string imagePath;
    public bool isDead;
    public GameManager.CriticalState criticalState;
    public GameManager.FambleState fambleState;
    public List<Skill> skills = new List<Skill>();
    public List<Weapon> weapons = new List<Weapon>();
    public GameManager.CharacterKind kind;

    public Character(int id, string name, int hp, int dex, string imagePath, GameManager.CharacterKind kind)
    {
        this.id = id;
        this.Cname = name;
        currentHP = hp;
        maxHP = hp;
        this.dex = dex;
        this.imagePath = imagePath;
        this.isDead = false;
        skills.Clear();
        weapons.Clear();
        this.kind = kind;
        criticalState = GameManager.CriticalState.None;
        fambleState = GameManager.FambleState.None;
    }
    public void Damage(int damage)
    {
        currentHP -= damage;
    }
}
