using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    public enum CriticalState
    {
        None,
        DoubleDamage,
        Unavoidable,
    }
    public enum FambleState
    {
        None,
        DoubleDamage,
        Unavoidable,
    }
    public enum DiceState
    {
        Success,
        Fail,
        Critical,
        Famble,
        None
    }
    public enum CharacterKind
    {
        Player,
        Enemy
    }
    public enum SystemState
    {
        None,
        WaitingPlayerAction,
    }
    public enum PieceType
    {
        luck_CC,
        luck_CCB,
        Noluck_CC,
        Noluck_CCB,
    }
    //スクリプトのインポート
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private PlayerActionController _playerActionController;
    [SerializeField] private DiceRoller _diceRoller;

    [System.NonSerialized] public Character _player1;
    [System.NonSerialized] public Character _enemy1;
    [System.NonSerialized] public List<Character> _aliveCharacterDex_az = new List<Character>();
    [System.NonSerialized] public List<Character> _allCharacter = new List<Character>();
    [System.NonSerialized] public List<Character> _allPlayer = new List<Character>();
    [System.NonSerialized] public List<Character> _allEnemy = new List<Character>();
    void Start()
    {
        //titleシーンからインポート
        _player1 = TitleSceneManager._player;
        _aliveCharacterDex_az.Add(_player1);
        _allCharacter.Add(_player1);
        _uiManager.CreateIcon(_player1);
        //敵の生成
        _enemy1 = new Character(1, "Skeleton", 15, 18, "Enemy_Icon", CharacterKind.Enemy);
        _enemy1.weapons.Add(new Weapon("カマ","カマ",75,1,7,AudioManager.Move.Hunmer));
        _enemy1.weapons.Add(new Weapon("殴る","近接攻撃",85,1,3,AudioManager.Move.Panch));
        _enemy1.skills.Add(new Skill("CC<=55【回避】","回避",55,AudioManager.Move.Dodge));
        _aliveCharacterDex_az.Add(_enemy1);
        _allCharacter.Add(_enemy1);

        _uiManager.CreateIcon(_enemy1);
        _uiManager.CreateEnemyAppearance(_enemy1);


        //プレイヤーの技能のUIButton生成
        foreach (Skill skill in _player1.skills) { 
            if(skill.name == "応急手当") { _uiManager.CreateButton(skill); }
        }
        foreach (Weapon weapon in _player1.weapons) { _uiManager.CreateButton(weapon); }

        //行動順（イニシアチブ）比較
        _aliveCharacterDex_az.Sort((a, b) => b.dex - a.dex);

        //PlayerとEnemyのリスト作成
        foreach (Character character in _aliveCharacterDex_az)
        {
            switch (character.kind)
            {
                case CharacterKind.Player: _allPlayer.Add(character); break;
                case CharacterKind.Enemy: _allEnemy.Add(character); break;
            }
        }

        //ボタンのインタラクティブ初期化
        _uiManager.IsInteractable(false);

        //ゲーム開始
        StartCoroutine("System");
    }

    private int _turnIndex = 0;
    private int _turn = 0;
    private int _round = 0;
    private IEnumerator enumerator;
    [System.NonSerialized] public Character _atker;
    [System.NonSerialized] public Character _dfner;
    public IEnumerator System()
    {
        //バトル
        while (_allEnemy.Count != 0 && _allPlayer.Count != 0)
        {
            //Debug.LogFormat("_turn：{0}\n_turnIndex：{1}\n_allCharacterDex_az.Count：{2}", _turn, _turnIndex, _allCharacterDex_az.Count);
            //ラウンドとターンの制御
            if (_turn == 0) { _round++; _uiManager.CreateLog("--Round" + _round + "------------", UIManager.Line.Line1, 55); }
            _turn++;
            switch (_aliveCharacterDex_az[_turnIndex].kind)
            {
                case CharacterKind.Player: _uiManager.CreateLog("ー探索者のターンー", UIManager.Line.Line1, 45); break;
                case CharacterKind.Enemy: _uiManager.CreateLog("ー敵のターンー", UIManager.Line.Line1, 45); break;
            }
            //ターンの開始
            IEnumerator enumerator = Turn(_aliveCharacterDex_az[_turnIndex]);
            yield return enumerator;
            //_deferがターン終了時に死んでいたら死亡処理をする。
            if (_dfner.currentHP <= 0) { _dfner.isDead = true; Dead(_dfner); }
            _turnIndex++;
            if (_aliveCharacterDex_az.Count <= _turnIndex) { _turnIndex = 0;  _turn = 0; }
        }
        //バトル終了後
        if (_allPlayer.Count == 0)
        { _uiManager.StartCoroutine("GameOverProcces"); }
        else
        { _uiManager.StartCoroutine("ClearProcess"); _audioManager.EnemyDeadSound(); }
        yield return null;
    }

    [System.NonSerialized] public SystemState _state = SystemState.None;
    public IEnumerator Turn(Character actCharacter)
    {
        _atker = actCharacter;
        enumerator = SelectDFN(actCharacter.kind);
        yield return enumerator;
        _dfner = (Character)enumerator.Current;
        Debug.LogFormat("このターンのatk：{0}\nこのターンのdfn：{1}", _atker.Cname, _dfner.Cname);
        switch (actCharacter.kind)
        {
            case CharacterKind.Player:
                //パネル用意依頼
                _uiManager.IsInteractable(true);
                //行動受け付け
                _state = SystemState.WaitingPlayerAction;
                //Debug.Log("PL行動受け付け");
                ///変数を用意して置いて、そこに構想するものの種類を代入させる
                ///そして、_stateをNoneにして処理続行
                yield return new WaitUntil(() => { return _state != SystemState.WaitingPlayerAction; });
                //関連パネルをすべて非表示にする
                ///ボタンを押したときの処理に含めている。
                break;
            case CharacterKind.Enemy:

                IEnumerator _index = _diceRoller.DiceRoll(1, _atker.weapons.Count);
                yield return _index;
                Debug.LogFormat("_atker.weapons.Count：{0}", _atker.weapons.Count);
                Debug.LogFormat("num：{0}", (int)_index.Current);
                yield return _playerActionController.AttackManage(_atker.weapons[(int)_index.Current - 1]);
                
                //処理
                break;
        }
        //Debug.Log("ターン終了.");
        yield return null;
    }
    public IEnumerator SelectDFN(CharacterKind atkKind)
    {
        int index;
        if (_aliveCharacterDex_az.Count <= 2)
        {
            yield return _allEnemy[0];
        }
        switch (atkKind)
        {
            case CharacterKind.Player:
                index = Random.Range(0, _allEnemy.Count);
                Debug.LogFormat("SelectDFN\n_dfner = {0}", _allEnemy[index].Cname);
                yield return _allEnemy[index];
                break;
            case CharacterKind.Enemy:
                index = Random.Range(0, _allPlayer.Count);
                Debug.LogFormat("SelectDFN\n_dfner = {0}", _allPlayer[index].Cname);
                yield return _allPlayer[index];
                break;
            default: yield return null; break;
        }
    }

    public void Dead(Character character)
    {
        switch (character.kind)
        {
            case CharacterKind.Player: _allPlayer.Remove(character); break;
            case CharacterKind.Enemy: _allEnemy.Remove(character); break;
        }
        _aliveCharacterDex_az.Remove(character);
        return;
    }

    //public void GameOver()
    //{

    //}

    //public void GameClear()
    //{

    //}
}
