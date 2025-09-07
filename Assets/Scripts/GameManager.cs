using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public enum MoveState
    {
        Fight,
        Act
    }

    public enum CharacterKind
    {
        Player,
        Enemy
    }

    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
    }
    public enum SystemState
    {
        None,
        WaitingPlayerAction,
    }
    //スクリプトのインポート
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;

    [System.NonSerialized] public Character _player1;
    [System.NonSerialized] public List<Character> _allCharacterDex_az = new List<Character>();
    [System.NonSerialized] public List<Character> _allPlayer = new List<Character>();
    [System.NonSerialized] public List<Character> _allEnemy = new List<Character>();
    void Start()
    {
        //titleシーンからインポート
        _player1 = TitleSceneManager._player;
        _uiManager.CreateIcon(_player1);
        //敵の生成
        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        //敵のUI反映

        //プレイヤーの技能のUIButton生成

        //行動順（イニシアチブ）比較
        _allCharacterDex_az.Add(_player1);
        _allCharacterDex_az.Sort((a, b) => b.dex - a.dex);

        //PlayerとEnemyのリスト作成
        foreach (Character character in _allCharacterDex_az)
        {
            switch (character.kind)
            {
                case CharacterKind.Player: _allPlayer.Add(character); break;
                case CharacterKind.Enemy: _allEnemy.Add(character); break;
            }
        }

        //ゲーム開始
    }

    private int _turnIndex = 0;
    private int _turn = 0;
    private int _round = 0;
    [System.NonSerialized] public Character _atker;
    [System.NonSerialized] public Character _dfner;
    public IEnumerator System()
    {
        //バトル
        while (_allEnemy.Count != 0 && _allPlayer.Count != 0)
        {
            //ラウンドとターンの制御
            if (_turn == 0) { _round++; _uiManager.CreateLog("--Round" + _round + "------------", UIManager.Line.Line1, 55); }
            _turn++;
            switch (_allCharacterDex_az[_turnIndex].kind)
            {
                case CharacterKind.Player: _uiManager.CreateLog("ー探索者のターンー", UIManager.Line.Line1, 45); break;
                case CharacterKind.Enemy: _uiManager.CreateLog("ー敵のターンー", UIManager.Line.Line1, 45); break;
            }
            //ターンの開始
            IEnumerator enumerator = Turn(_allCharacterDex_az[_turnIndex]);
            yield return enumerator;
            //_deferがターン終了時に死んでいたら死亡処理をする。
            if (_dfner.currentHP <= 0) { _dfner.isDead = true; Dead(_dfner); }
            _turnIndex++;
            if (_turnIndex <= _allCharacterDex_az.Count) { _turnIndex = 0; }
        }
        //バトル終了後
        if (_allPlayer.Count != 0)
        { _uiManager.StartCoroutine("GameOverProcces"); }
        else
        { _uiManager.StartCoroutine("ClearProcess"); _audioManager.EnemyDeadSound(); }
        yield return null;
    }

    [System.NonSerialized] public SystemState _state = SystemState.None;
    public IEnumerator Turn(Character actCharacter)
    {
        _atker = actCharacter;
        _dfner = SelectDFN(actCharacter.kind);
        switch (actCharacter.kind)
        {
            case CharacterKind.Player:
                //パネル用意依頼
                _state = SystemState.WaitingPlayerAction;
                //行動受け付け
                ///変数を用意して置いて、そこに構想するものの種類を代入させる
                ///そして、_stateをNoneにして処理続行
                yield return new WaitUntil(() => { return _state != SystemState.WaitingPlayerAction; });
                //関連パネルをすべて非表示に（ここでしなくてもよいかも）
                break;
            case CharacterKind.Enemy:
                //パネル用意依頼
                //行動受け付け
                //処理
                break;
        }
        yield return null;
    }
    public Character SelectDFN(CharacterKind atkKind)
    {
        int index;
        if (_allCharacterDex_az.Count <= 2)
        {
            return _allEnemy[0];
        }
        switch (atkKind)
        {
            case CharacterKind.Player:
                index = Random.Range(0, _allEnemy.Count);
                return _allEnemy[index];
            case CharacterKind.Enemy:
                index = Random.Range(0, _allPlayer.Count);
                return _allPlayer[index];
            default: return null;
        }
    }

    public void Dead(Character character)
    {
        switch (character.kind)
        {
            case CharacterKind.Player: _allPlayer.Remove(character); break;
            case CharacterKind.Enemy: _allEnemy.Remove(character); break;
        }
        _allCharacterDex_az.Remove(character);
        return;
    }

    public void GameOver()
    {

    }

    public void GameClear()
    {

    }
}
