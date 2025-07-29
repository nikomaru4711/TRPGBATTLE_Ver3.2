public class Character
{
    public int id;
    public string name;
    public int currentHP;
    public int maxHP;
    public int dex;
    public string imagePath;
    public bool isDead;
    public GameManager.CriticalState criticalState = GameManager.CriticalState.None;
    public GameManager.FambleState fambleState = GameManager.FambleState.None;
    public Weapon[] weapons = new Weapon[10];
    public Skill[] skills = new Skill[10];
    public GameManager.CharacterKind kind;

    public Character(int id,string name, int hp, int dex, string imagePath, bool isDead, Weapon[] weapons, Skill[] skills, GameManager.CharacterKind kind)
    {
        this.id = id;
        this.name = name;
        currentHP = hp;
        maxHP = hp;
        this.dex = dex;
        this.imagePath = imagePath;
        this.isDead = isDead;
        for (int i = 0; i < weapons.Length; i++)
        {
            this.weapons[i] = weapons[i];
        }
        for (int i = 0;i < skills.Length; i++)
        {
            this.skills = skills;
        }

        this.skills = skills;
        this.kind = kind;
    }
}
