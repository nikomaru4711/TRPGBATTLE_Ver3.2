using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int id;
    public string Cname;
    public int currentHP;
    public int maxHP;
    public int dex;
    public string imagePath;
    public bool isDead;
    public GameManager.CriticalState criticalState = GameManager.CriticalState.None;
    public GameManager.FambleState fambleState = GameManager.FambleState.None;
    public List<Skill> skills = new List<Skill>();
    public List<Weapon> weapons = new List<Weapon>();
    public GameManager.CharacterKind kind;

    public Character(int id, string name, int hp, int dex, string imagePath, GameManager.CharacterKind kind)
    {
        this.id = id;
        this.name = name;
        currentHP = hp;
        maxHP = hp;
        this.dex = dex;
        this.imagePath = imagePath;
        this.isDead = false;
        this.kind = kind;
    }
    public void Damage(int damage)
    {
        currentHP -= damage;
    }
}
