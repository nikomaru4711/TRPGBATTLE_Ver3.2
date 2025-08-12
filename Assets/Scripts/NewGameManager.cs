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
    //�X�N���v�g�̃C���|�[�g
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;
    //
    [System.NonSerialized] public Character _player1;
    private List<Character> _characterDex_az = new List< Character>();
    void Start()
    {
        //title�V�[������C���|�[�g
        _player1 = TitleSceneManager._player;
        _uiManager.CreateIcon(_player1);
        //�G�̐���


        //�s�����i�C�j�V�A�`�u�j��r
        _characterDex_az.Add(_player1);
        _characterDex_az.Sort((a, b) => b.dex - a.dex);

        //�^�[���J�n
    }

    private int turnIndex = 0;
    private int turn = 1;
    public IEnumerator System()
    {
        while (isGameEnd())
        {
            //�^�[��
            //��������

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
                _uiManager.CreateLog("�[�T���҂̃^�[���[", UIManager.Line.Line1, 45);
                //�p�l���p��
                //�s���󂯕t��
                //����
                break;
            case GameManager.CharacterKind.Enemy:
                _uiManager.CreateLog("�[�G�̃^�[���[", UIManager.Line.Line1, 45);
                //�p�l���p��
                //�s���󂯕t��
                //����
                break;
        }

        yield return null;
    }
    /// <summary>
    ///���������Ă邩����
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
