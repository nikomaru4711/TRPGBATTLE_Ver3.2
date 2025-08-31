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
    public enum SystemState
    {
        None,
        WaitingPlayerAction,
    }
    //スクリプトのインポート
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;
    
    [System.NonSerialized] public Character _player1;
    private List<Character> _allCharacterDex_az = new List< Character>();
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

        //ゲーム開始
    }

    private int _turnIndex = 0;
    private int _turn = 0;
    private int _round = 0;
    public IEnumerator System()
    {
        //バトル
        while (isGameEnd())
        {
            //ラウンドとターンの制御
            if (_turn == 0) { _round++; /*UIManagerにラウンド更新の依頼*/ }
            _turn++;
            /*UIManagerにターン更新の依頼*/

            IEnumerator enumerator = Turn(_allCharacterDex_az[_turnIndex]);
            yield return enumerator;
            _turnIndex++;
            if(_turnIndex <= _allCharacterDex_az.Count) { _turnIndex = 0; }
        }
        //バトル終了後
        ///PLが負けたのか勝ったのかを判定
        ///それに応じてパネルの依頼
    }

    private SystemState _state = SystemState.None;
    public IEnumerator Turn(Character actCharacter)
    {
        switch (actCharacter.kind)
        {
            case GameManager.CharacterKind.Player:
                _uiManager.CreateLog("ー探索者のターンー", UIManager.Line.Line1, 45);
                //パネル用意
                _state = SystemState.WaitingPlayerAction;
                //行動受け付け
                ///変数を用意して置いて、そこに構想するものの種類を代入させる
                ///そして、_stateをNoneにして処理続行
                yield return new WaitUntil(() => { return _state != SystemState.WaitingPlayerAction; });
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
        foreach (Character character in _allCharacterDex_az)
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
        if(_isAliveP && _isAliveE){ return true; }else{ return false; }
    }

    public void GameOver()
    {

    }

    public void GameClear()
    {

    }
}
