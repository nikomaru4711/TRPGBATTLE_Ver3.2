using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameManager : MonoBehaviour
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
    //スクリプトのインポート
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;
    //
    [System.NonSerialized] public Character _player1;
    private List<Character> _characterDex_az = new List< Character>();
    void Start()
    {
        //titleシーンからインポート
        _player1 = TitleSceneManager._player;
        _uiManager.CreateIcon(_player1);
        //敵の生成


        //行動順（イニシアチブ）比較
        _characterDex_az.Add(_player1);
        _characterDex_az.Sort((a, b) => b.dex - a.dex);

        //ターン開始
    }

    private int turnIndex = 0;
    private int turn = 1;
    public IEnumerator System()
    {
        while (isGameEnd())
        {
            //ターン
            //←ここに

            IEnumerator enumerator = Turn(_characterDex_az[turnIndex]);
            yield return enumerator;
            turnIndex++;
            if(turnIndex <= _characterDex_az.Count) { turnIndex = 0; }
        }
        foreach (Character character in _characterDex_az)
        {
            IEnumerator enumerator = Turn(_characterDex_az[turnIndex]);
            yield return enumerator;
            turnIndex++;
        }
    }
    public IEnumerator Turn(Character actCharacter)
    {
        switch (actCharacter.kind)
        {
            case GameManager.CharacterKind.Player:
                _uiManager.CreateLog("ー探索者のターンー", UIManager.Line.Line1, 45);
                //パネル用意
                //行動受け付け
                //処理
                break;
            case GameManager.CharacterKind.Enemy:
                _uiManager.CreateLog("ー敵のターンー", UIManager.Line.Line1, 45);
                //パネル用意
                //行動受け付け
                //処理
                break;
        }

        yield return null;
    }
    /// <summary>
    ///決着がついてるか判定
    /// </summary>
    private bool _isAliveP;
    private bool _isAliveE;
    public bool isGameEnd()
    {
        _isAliveP = false;
        _isAliveE = false;
        foreach (Character character in _characterDex_az)
        {
            switch(character.kind)
            {
                case GameManager.CharacterKind.Player:
                    if (!character.isDead) { _isAliveP = true; }
                    break;
                case GameManager.CharacterKind.Enemy:
                    if (!character.isDead) { _isAliveE = true; }
                    break;
            }
        }
        if(_isAliveP && _isAliveE){ return false; }else{ return true; }
    }
}
