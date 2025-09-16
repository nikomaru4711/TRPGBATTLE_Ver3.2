using System.Collections;
using UnityEngine;
    public class Skill
    {
        public string diceText;//例：CC<=55 【アイデア】
        public string actionName;//例：思いつく
        public string name;//例：アイデア
        public int successNum;//例：55
        public AudioManager.Move soundType;

        public Skill(string diceText, string actionName, int successNum, AudioManager.Move soundType)
        {
            this.diceText = diceText;
            string[] parts = diceText.Split(new char[] { '【', '】' });
            if (parts.Length >= 2)
            {
                this.name = parts[1];
                this.actionName = name;
            }
            this.actionName = actionName;
            this.successNum = successNum;
            this.soundType = soundType;
        }
        public virtual IEnumerator Move(Character character, UIManager _uiManager, DiceRoller _diceRoller, AudioManager _audioManager) { Debug.LogError("スーパークラスのMoveが呼び出されています！"); yield return null; }
    }

    public class Heal : Skill
    {
        int healUpper;
        public Heal(string diceText, string actionName, int successNum, AudioManager.Move soundType, int healUpper) : base(diceText, actionName, successNum, soundType)
        {
            this.healUpper = healUpper;
        }
        public override IEnumerator Move(Character character, UIManager _uiManager, DiceRoller _diceRoller, AudioManager _audioManager)
        {
            Debug.Log("Move() in Skill.");
            IEnumerator enumerator = _diceRoller.DiceRoll(1, healUpper);
            yield return enumerator;
            if(character.maxHP < character.currentHP + (int)enumerator.Current)
            {
                _uiManager.CreateLog("<size=35>[" + character.Cname + "]\nHP:" + character.currentHP + "→" + character.maxHP + "</size>",UIManager.Line.Line2);
                character.currentHP = character.maxHP;

            } else
            {
                _uiManager.CreateLog("<size=35>[" + character.Cname + "]\nHP:" + character.currentHP + "→" + (character.currentHP + (int)enumerator.Current) + "</size>", UIManager.Line.Line2);
                character.currentHP += (int)enumerator.Current;
            }
            _audioManager.MoveSound(soundType);
        　　_uiManager.UpdateCharacterUI(character);
        yield return null;
        }
    }