using JetBrains.Annotations;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using static AudioManager;
using static GameManager;

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

    //�X�N���v�g�̃C���|�[�g
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;

    //�����̃f�[�^��荞��
    private int[] data = new int[6];

    //�e�L�����N�^�[
    [System.NonSerialized]public Character _player1;
    [System.NonSerialized] public Character _enemy1;
    [System.NonSerialized] public Weapon[] _weapon1 = new Weapon[10];
    [System.NonSerialized] public Skill[] _skill1 = new Skill[10];
    [System.NonSerialized] public Weapon[] _weapon2 = new Weapon[10];
    [System.NonSerialized] public Skill[] _skill2 = new Skill[10];

    private int i;
    private int _damageMultiplier = 1;
    private bool _isAvoidable;
    private int _index;
    private GameObject _obj;
    private TurnState _turnState;
    private IEnumerator _damage;
    private IEnumerator _num;
    private IEnumerator _actNum;
    private IEnumerator _chooseEffect;
    private string _message;
    public float _interval = 2.0f;
    IEnumerator _diceState;
    IEnumerator _avoidState;
    //DiceState _diceState = DiceState.None;
    //DiceState _avoidState = DiceState.None;
    //�L�����N�^�[��U���A�s���Ȃǂ̃A�N�V������ǉ�����x�ɂ����̒l���C���N�������g���Ă����B
    private int _characternum = 0;
    private int _weaponIndex = 3;
    private int _skillIndex = 2;
    private int _enemyWeaponIndex = 2;
    private int _enemySkillIndex = 2;
    private int _round = 0;
    private int _turn = 0;

    //�U���҂ƍU���Ώێ҂�ݒ肷��
    [System.NonSerialized] public Character _attacker;
    [System.NonSerialized] public Character _defender;
    [System.NonSerialized] public Character _temp;

    //�L�����N�^�[�ꗗ�̊i�[�z��
    public Character[] _characterArray = new Character[10];


    private void Start()
    {
        //�^�C�g���ł̓��̓f�[�^�̃C���|�[�g
        ImportFromTitle(ref data);
        //�G�̌��f�[�^�̍쐬
        _weapon2[0] = new Weapon("����", "���Ԃ�", 80, 1, 3, false, Move.Panch);
        _weapon2[1] = new Weapon("�n���}�[�ōU��", "�n���}�[", 65, 1, 8, false, Move.Hunmer);
        _skill2[0] = new Skill("�C��", "���}�蓖", 30, Move.FirstAid);
        _skill2[1] = new Skill("������" , "���", 25, Move.Dodge);


        //�v���C���[�̍쐬
        //_player1 = new Character(1, "�T����", TitleSceneManager._playerStatus[0], TitleSceneManager._playerStatus[1], null, true, _weapon1, _skill1, CharacterKind.Player);
        SetCharacterAray(_player1);
        //�v���C���[�̋Z�\��UIButton����
        _uiManager.CreateIcon(_player1);
        for (i = 0; i < _weaponIndex; i++)
        {
            _uiManager.CreateButton(_weapon1[i]);
        }
        for (i = 0; i < _skillIndex; i++)
        {
            _uiManager.CreateButton(_skill1[i]);
        }
        //�����Ō�Panel�Ƃ���ꂽ�畡��PC������̂ɂȂ��i��]�j
        //�G�̍쐬
        _enemy1 = new Character(2, "�X�P���g��", 14, 13, "Enemy_Icon", true, _weapon2, _skill2, CharacterKind.Enemy);
        SetCharacterAray(_enemy1);
        //�G��UI���f
        _uiManager.CreateIcon(_enemy1);
        _uiManager.CreateEnemyAppearance(_enemy1);
        
        //�U�������̐ݒ�B����̓^�C�}���Ȃ̂Őݒ�̎d��������B
        SetAttackOrder();
        //UI�Ƀ��E���h�\��
        RoundManage();
    }
    //��ɂ���i��x�ł��j�S�ẴL�����N�^�[�������Ă���z��
    public void SetCharacterAray(Character character)
    {
        _characterArray[_characternum] = character;
        _characternum++;
    }

    //�U�������̐ݒ������
    public void SetAttackOrder()
    {
        for(i = 0; i < _characternum; i++)
        {
            if (_characterArray[i].id == _uiManager._displayOrder[0]._id)
            {
                _attacker = _characterArray[i];
            } else if (_characterArray[i].id == _uiManager._displayOrder[1]._id)
            {
                _defender = _characterArray[i];
            }
        }
        if(_attacker.kind == CharacterKind.Player)
        {
            _turnState = TurnState.PlayerTurn;
        } else
        {
            _turnState = TurnState.EnemyTurn;
            StartCoroutine("EnemyManage"); 
            _uiManager.IsInteractable(false);
        }
    }

    private void Update()
    {//�퓬�^�[�����Ǘ��B���ۂɂ͓������Ȃ����ǎ���ꂷ��ꍇ�͂����ł���B
        switch (_turnState)
        {
            case TurnState.PlayerTurn:
                break;
            case TurnState.EnemyTurn:
                break;
        }
    }

    public void RoundManage()
    {
        _turn++;
        //���E���h�̕\�L
        if (_turn % 2 == 1)
        {
            _round++;
            _uiManager.CreateLog("--Round" + _round + "------------", UIManager.Line.Line1, 55);
        }
        //�^�[���̕\�L
        switch (_turnState)
        {
            case TurnState.PlayerTurn:
                _uiManager.CreateLog("�[�T���҂̃^�[���[", UIManager.Line.Line1, 45);
                break;
            case TurnState.EnemyTurn:
                _uiManager.CreateLog("�[�G�̃^�[���[", UIManager.Line.Line1, 45);
                break;
        }
        
    }
    //�G�̍s����������U���܂ŁB
    public IEnumerator EnemyManage()
    {
        Debug.Log("�G��������I");
        //�ǂ̍U����U��̂��_�C�X��U��B
        yield return new WaitForEndOfFrame();
        _actNum = DiceRoll(1, 2);
        yield return _actNum;
        if((int)_actNum.Current == 1)
        {
            _index = GetWeaponIndex(_attacker, "����");
        } else
        {
            _index = GetWeaponIndex(_attacker, "�n���}�[�ōU��");
        }
        StartCoroutine("AttackManage", _attacker.weapons[_index]);
    }

    //GetSkillIndex(Character character, string skillName)


    //�U���̏������s���B�����̑�����@��v����
    //�������Ɋւ��āA�K�v�Ȃ͉̂��L�̎҂ł��邪Weapon�^�̊֐�����Ăяo���̂œn�����@���Ȃ��B
    //���A�A�^�b�J�[�A�f�B�t�F���_�[��GameManager�ŊǗ����āA��������Q�Ƃ���̂��悳�����B
    public IEnumerator AttackManage(Weapon weapon)
    {
        Debug.LogFormat("�e�X�g�F{0}�ōU�������܂����I", weapon.name);
        //��Ԃ̑��
        _isAvoidable = weapon.avoidable;
        _damageMultiplier = 1;

        //����̃t�@���u���͗L�����H
        switch (_defender.fambleState)
        {
            case FambleState.None:
                break;
            case FambleState.Unavoidable:
                _isAvoidable = false;
                break;
            case FambleState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking target \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }
        _defender.fambleState = FambleState.None;

        //Attacjer�̃N���e�B�J���󋵂��m�F�i�_���[�W��{�̂݁j
        switch(_attacker.criticalState)
        {
            case CriticalState.None:
                break;
            case CriticalState.Unavoidable:
                _isAvoidable = false;
                break;
            case CriticalState.DoubleDamage:
                _damageMultiplier *= 2;
                break;
            default:
                _uiManager.CreateLog("<color=red>Error!</color>checking attcker \nCritical/Famble failed.", UIManager.Line.Line1);
                break;
        }

        //�Z�\�_�C�X��U��B���������玟�ցi���̎��U�����_�C�X�̃N���e�B�J���A�t�@���u���`�F�b�N�j
        _diceState = DiceRoll(weapon.successNum, "�y" + weapon.name + "�z", _attacker);
        yield return _diceState;
        //�U���ł������H
        if ((DiceState)_diceState.Current == DiceState.Success || (DiceState)_diceState.Current == DiceState.Critical)
        {
            //�N���e�B�J�����H������^�C�~���O�͂��������Ȃ��Ă邯�ǈ�U�ȗ��B
            //�i���̏�Ԃ��ƕK�����^�[���̍U���ɉe���B�j
            

            //���킪����\���H
            if (weapon.avoidable)
            {
                //�G�͉���ł���󋵂��H�i�t�@���u�����ʁj
                if(_defender.fambleState != FambleState.Unavoidable)
                {
                    //����̃X�L���������o��
                    _index = GetSkillIndex(_defender, "������");
                    //����̉���_�C�X
                    _avoidState = DiceRoll(_defender.skills[_index].successNum, "�y����z", _defender);
                    yield return _avoidState;
                    if ((DiceState)_avoidState.Current == DiceState.Success || (DiceState)_avoidState.Current == DiceState.Critical)
                    { _audioManager.MoveSound(_defender.skills[_index].soundType); }
                    yield return new WaitForSeconds(1f);
                    //�����ŉ�����N���e�B�J���̏ꍇ��ǋL����B����͉����CF�����ɂ���̂ŏȗ��B
                } else
                {
                    _uiManager.CreateLog("[�t�@���u������] ���s�I",UIManager.Line.Line1);
                }

            }

            //�_���[�W�v�Z
            if (!weapon.avoidable || (DiceState)_avoidState.Current == DiceState.Fail || (DiceState)_avoidState.Current == DiceState.Famble)
            {
                Debug.Log("�_���[�W�_�C�X��U��܂�");
                _damage = DiceRoll(weapon.diceNum, weapon.damageNum, _damageMultiplier);
                yield return _damage;
                _audioManager.MoveSound(weapon.soundType);
                _uiManager.UpdateCharacterHP(_defender, -(int)_damage.Current);
                yield return new WaitForSeconds(1.5f);
            }
        }
        Debug.Log("���̃^�[���֍s���܂�");
        //���̃^�[����
        ChangeTurn();
        yield break;
    }

    //ACT�{�^���̍s�������͂��ׂĂ����ōs���B
    public IEnumerator MoveManage(Skill skill)
    {//�s�������S�ʂ������ōs��
        Debug.LogFormat("{0}�����܂����I",skill.name);
        //�Z�\�_�C�X��U��B���������玟�ցi���̎��U�����_�C�X�̃N���e�B�J���A�t�@���u���`�F�b�N�j
        _diceState = DiceRoll(skill.successNum, "�y" + skill.name + "�z", _attacker);
        yield return _diceState;

        //�_�C�X�ɐ����������H
        if ((DiceState)_diceState.Current == DiceState.Success || (DiceState)_diceState.Current == DiceState.Critical)
        {
            //���}�蓖�̏ꍇ
            if(skill.name == "���}�蓖")
            {
                _num = DiceRoll(1, 3);
                yield return _num;
                if(_attacker.currentHP == _attacker.maxHP)
                {
                    _uiManager.CreateLog("����ȏ�񕜂��Ȃ�\n�i�����Ȃ��j", UIManager.Line.Line2);
                } else
                {
                    _uiManager.UpdateCharacterHP(_attacker, (int)_num.Current);
                }
            }
            _audioManager.MoveSound(skill.soundType);
            yield return new WaitForSeconds(3.0f);
        }

        //���̃^�[����
        ChangeTurn();
        yield break;
    }

    //�L�����N�^�[�̓���̃X�L�������o���B
    public int GetSkillIndex(Character character, string skillName)
    {
        Debug.LogFormat("Input�F{0}",skillName);
        if (character.kind == CharacterKind.Player)
        {
            for (i = 0; i < _skillIndex; i++)
            {
                Debug.LogFormat("{0}", i);
                Debug.LogFormat("skills[{0}]�F{1}", i, character.skills[i].actionName);
                if (character.skills[i].actionName == skillName)
                {
                    return i;
                }
                Debug.LogFormat("{0}", i);
            }
        } else
        {
            for (i = 0; i < _enemySkillIndex; i++)
            {
                if (character.skills[i].actionName == skillName)
                {
                    return i;
                }
            }
        }
        _uiManager.CreateLog("<color=red>Error!</color>SearchingSkill Failed", UIManager.Line.Line1);
        return -1;
    }
    public int GetWeaponIndex(Character character, string weaponName)
    {
        if (character.kind == CharacterKind.Player)
        {
            for (i = 0; i < _weaponIndex; i++)
            {
                
                if (character.weapons[i].actionName == weaponName)
                {
                    return i;
                }
            }
        }
        else
        {
            for (i = 0; i < _enemyWeaponIndex; i++)
            {
                if (character.weapons[i].actionName == weaponName)
                {
                    return i;
                }
            }
        }
        _uiManager.CreateLog("<color=red>Error!</color>SearchingWeapon Failed", UIManager.Line.Line1);
        return -1;
    }

    //�N���U��������̂������߂�B
    public void ChangeTurn()
    {
        _temp = _attacker;
        _attacker = _defender;
        _defender = _temp;
        switch (_turnState)
        {
            case TurnState.PlayerTurn:
                _turnState = TurnState.EnemyTurn;
                //�����G�������Ă�����...
                if(_attacker.isDead)
                {
                    RoundManage();
                    StartCoroutine("EnemyManage");
                }
                break;
            case TurnState.EnemyTurn:
                //����Player�������Ă�����...
                if (_attacker.isDead)
                {
                    _turnState = TurnState.PlayerTurn;
                    RoundManage();
                    _uiManager.IsInteractable(true);
                }
                break;
        }
    }

    //�U������Ώۂ����肷��B����ver�ł̓^�C�}���Ȃ̂Ŗ��ݒ�B
    public void DecideTarget()
    {

    }

    //Title�V�[���̓��̓f�[�^�������Ă��đ��
    private void ImportFromTitle(ref int[] data)
    {
        /*�O���[�o���ϐ��z��ɂ͈ȉ��̂悤�Ɋi�[����Ă���
         * 0:HP
         * 1:DEX
         * 2:���}�蓖
         * 3:���Ԃ�
         * 4:����
         * 5:���
         */
        //_weapon1[0] = new Weapon("�i�C�t�Ŏh��", "���Ԃ�", TitleSceneManager._playerStatus[3], 1, 6, false, Move.Knife);
        //_weapon1[1] = new ThrowTorch("�����𓊂���", "����", TitleSceneManager._playerStatus[4], 1, 3, true, Move.Throw);
        //_weapon1[2] = new Weapon("����", "���Ԃ�", TitleSceneManager._playerStatus[3], 1, 3, false, Move.Panch);
        //_skill1[0] = new FirstAid("���}���u���{��", "���}�蓖", TitleSceneManager._playerStatus[2], Move.FirstAid);
        //_skill1[1] = new Dodge("������", "���", TitleSceneManager._playerStatus[5], Move.Dodge);
    }

    //�_���[�W�_�C�X��U���Č��ʂ�Ԃ��B�i���O�ɂ��������ށj
    public IEnumerator DiceRoll(int times, int upper, int multiplier = 1)
    {
        int num;
        int total = 0;
        if(times == 1)
        {
            if (upper > 3)
            { _audioManager.DiceSound(Dice.SingleMiddle); }
            else
            { _audioManager.DiceSound(Dice.SingleSmall); }
        } else
        {
            if (upper > 3)
            { _audioManager.DiceSound(Dice.DoubleMiddle); }
            else
            { _audioManager.DiceSound(Dice.DoubleSmall); }
        }


            _message = times + "d" + upper + " > [";
        for (i = 0; i < times; i++)
        {
            num = Random.Range(1, upper + 1);
            total += num;
            if (i == 0)
            {
                _message += num;
            }
            else
            {
                _message += "," + num;

            }
        }
        if(multiplier != 1)
        {
            total *= multiplier;
            _message += "] * " + multiplier + " > " + total;
        } else
        {
            _message += "] > " + total;
        }
        _uiManager.CreateLog(_message, UIManager.Line.Line1);
        yield return new WaitForSeconds(_interval);
        yield return total;
    }
    //�ȉ��g�p��
    //�@�@DiceRoll(2,4);
    //�@��2d4 > [1,3] > 4

    //100�ʃ_�C�X��U����Enum�Ō��ʂ�Ԃ��B�i���O�ɂ��������ށj
    public IEnumerator DiceRoll(int successupper, string skill, Character character)
    {
        int total = Random.Range(1, 101);
        _audioManager.DiceSound(Dice.SingleBig);
        if (total <= successupper)
        {
            if (total <= 5)
            {
                _message = "1d100<=" + successupper + skill + "\n<color=blue>(1d100<=" + successupper + ") > " + total + " > \n����I����/�X�y�V����</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                _chooseEffect = DiceRoll(1, 2);
                yield return _chooseEffect;
                if ((int)_chooseEffect.Current == 1)
                {
                    _uiManager.CreateLog("���̗^�_���[�W����{�I", UIManager.Line.Line1);
                    character.criticalState = CriticalState.DoubleDamage;
                }
                else
                {
                    _uiManager.CreateLog("���̍U���͉������Ȃ�", UIManager.Line.Line1);
                    character.criticalState = CriticalState.Unavoidable;
                }
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Critical;
            } else
            {
                _message = "1d100<=" + successupper + skill + "\n<color=blue>(1d100<=" + successupper + ") > " + total + " > \n����</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Success;
            }
        }
        else
        {
            if (total >= 96)
            {
                _message = "1d100<=" + successupper + skill + "\n<color=red>(1d100<=" + successupper + ") > " + total + " > \n�v���I���s</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                _chooseEffect = DiceRoll(1, 2);
                yield return _chooseEffect;
                if ((int)_chooseEffect.Current == 1)
                {
                    _uiManager.CreateLog("���̔�_���[�W��{��. . .", UIManager.Line.Line1);
                    character.fambleState = FambleState.DoubleDamage;

                }
                else
                {
                    _uiManager.CreateLog("���̍U���͉��s��. . .", UIManager.Line.Line1);
                    character.fambleState = FambleState.Unavoidable;
                }
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Famble;
            } else
            {
                _message = "1d100<=" + successupper + skill + "\n<color=red>(1d100<=" + successupper + ") > " + total + " > \n���s</color>";
                _uiManager.CreateLog(_message, UIManager.Line.Line3);
                yield return new WaitForSeconds(_interval);
                yield return DiceState.Fail;
            }
        }   
    }
    //�ȉ��g�p��
    //    DiceRoll(50, "�y�m���z");
    //�@��1d100<=50�y�m���z(1d100<=50) > 27 > ����
}
