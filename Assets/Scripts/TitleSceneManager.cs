using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using System;
public class TitleSceneManager : MonoBehaviour
{
    public enum State
    {
        phase1,
        phase2
    }
    //�A�N�Z�X�C���q�̌��static�ŃO���[�o����
    public PlayerCharacter _playerJson;
    public static Character _player;

[SerializeField] private ReadJson _readJson;
    [SerializeField] private GameObject _textTitle;
    [SerializeField] private GameObject _textDetail;
    [SerializeField] private GameObject _specialThanks;
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private GameObject _inputJson;
    [SerializeField] private GameObject _buttonStart;
    [SerializeField] private GameObject _validateErrorPanel;
    [SerializeField] private GameObject _inputErrorPanel;
    [SerializeField] private GameObject _methodPref;
    [SerializeField] private GameObject _registAtk;
    [SerializeField] private GameObject _atkScrollView;
    [SerializeField] private GameObject _scrollContent;
    private GameObject _dump;
    private List<GameObject> _atkMethodList = new List<GameObject>();
    private List<string> _optionList = new List<string>();
    private List<string> _atkList = new List<string>();
    private int _atkCount = 0;
    private string[] commands = new string[64];
    private TMP_Dropdown _dropdown;
    private State _state;
    

    //�O�̂��߁A�ŏ��Ɍ����Ȃ����̂��\���ɂ���悤�L�q
    private void Start()
    {
        _endGamePanel.SetActive(false);
        _atkList.Add("���Ԃ��ŉ���");
        _atkList.Add("�n���Ő؂肩����");
        _atkList.Add("���𓊂���");
    }

    private string _dumpText;
    private string _dumpText_dp1;
    private string _dumpText_dp2;
    private int _dumpInt;
    private string[] _dumpString_dice = new string[2];
    //�v���C���[���쐬���ăQ�[���J�n�B
    public void GameStart()
    {
        switch (_state)
        {
            case State.phase1:
                //////////////////
                //JSON�̓ǂݍ���//
                //////////////////
                try
                {
                    string _jsonText = _inputJson.GetComponent<TMP_InputField>().text.Replace("\"params\":", "\"param\":");
                    _playerJson = _readJson.ReadJsonFile(_jsonText);
                    if (_playerJson == null) { return; }
                    Debug.Log("JSON�̓ǂݍ��݊���");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSON�̓ǂݍ��݂Ɏ��s");
                    Debug.Log(e.ToString());
                    ValidateErrorPanel(true);
                    return;
                }
                //////////////////////
                //NewCharacter�̍쐬//
                //////////////////////
                _player = new Character(0, _playerJson.data.name, _playerJson.data.status[0].value, int.Parse(_playerJson.data.param[3].value), _playerJson.data.iconUrl, GameManager.CharacterKind.Player);
                ///////////////////
                //NewSkills�̍쐬//
                ///////////////////
                for (int i = 0; i < commands.Length; i++)
                {
                    commands = _playerJson.data.commands.Split('\n');
                }
                string marker = "CC<=";
                string dump_text;
                int startIndex;
                int endIndex;
                int index = 0;
                for (int i = 0; i < commands.Length - 12; i++)
                {
                    if (i == 1 || (2 < i && i < 50))
                    {
                        startIndex = commands[i].IndexOf(marker);
                        endIndex = commands[i].IndexOf(" �y");

                        if (startIndex != -1 && endIndex != -1)
                        {
                            startIndex += marker.Length;
                            dump_text = commands[i].Substring(startIndex, endIndex - startIndex);
                        }
                        else
                        {//�G���[
                            Debug.LogError("NewSkill�̍쐬�Ɏ��s���܂����I");
                            Debug.LogErrorFormat("i = {0}\ncommand[{1}] = {2}", i, i, commands[i]);
                            return;
                        }
                        Debug.LogFormat("i = {0}���쐬���܂��B", i);
                        Debug.LogFormat("dump_text = {0}", dump_text);
                        Debug.LogFormat("commands[{0}] = {1}", i, commands[i]);
                        _player.skills.Add(new Skill("",commands[i], int.Parse(dump_text),AudioManager.Move.None));
                        index++;
                    }
                }
                
                ///////////////////////
                //�P�D�I�v�V�����ǉ� //
                ///////////////////////
                PhaseChange(State.phase2);
                _dropdown = _methodPref.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>();
                for (int i = 0; i < commands.Length; i++)
                {
                    _optionList.Add(commands[i]);
                }
                _dropdown.ClearOptions();
                _dropdown.AddOptions(_optionList);
                _state = State.phase2;
                break;
            case State.phase2:
                foreach(GameObject obj in _atkMethodList)
                {
                    try
                    {
                        ////////////////////
                        //�e�v�f�̎��o��//
                        ////////////////////
                        _dumpText = obj.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
                        _dumpInt = obj.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().value;
                        _dumpText_dp1 = _optionList[_dumpInt];
                        _dumpString_dice = obj.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text.Split('d');
                        _dumpInt = obj.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().value;
                        _dumpText_dp2 = _atkList[_dumpInt];
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("�G���[�B���̓~�X�H");
                        Debug.Log(e.ToString());
                        InputErrorPanel(true);
                        return;
                    }
                    ////////////////
                    //Weapon�̍쐬//
                    ////////////////
                    _dumpInt = _player.skills.FindIndex(skill => skill.diceText == _dumpText_dp1);
                    Debug.LogErrorFormat("Index��������Ȃ��B�T���Fskill.diceText = {0}\n�T�����ʁFindex = {1}", _dumpText_dp1,_dumpInt);
                    switch (_dumpText_dp2)
                    {
                        case"���Ԃ��ŉ���":
                            _player.weapons.Add(new Weapon(_dumpText, _dumpText_dp1, _player.skills[_dumpInt].successNum, int.Parse(_dumpString_dice[0]), int.Parse(_dumpString_dice[1]), AudioManager.Move.Panch));
                            break;
                        case"�n���Ő؂肩����":
                            _player.weapons.Add(new Weapon(_dumpText, _dumpText_dp1, _player.skills[_dumpInt].successNum, int.Parse(_dumpString_dice[0]), int.Parse(_dumpString_dice[1]), AudioManager.Move.Knife));
                            break;
                        case "���𓊂���":
                            _player.weapons.Add(new Weapon(_dumpText, _dumpText_dp1, _player.skills[_dumpInt].successNum, int.Parse(_dumpString_dice[0]), int.Parse(_dumpString_dice[1]), AudioManager.Move.Throw));
                            break;
                    }
                }
                Debug.Log("�V�[���J�ڂ��܂��B");
                SceneManager.LoadScene("Battle");
                break;
        }
    }

    public void AddMethod()
    {
        _dump = Instantiate(_methodPref, new Vector3(0,0,0), Quaternion.identity);
        _dump.transform.SetParent(_scrollContent.transform);
        _dump.name = "AtkMethod_" + _atkCount;
        _atkCount++;
        _atkMethodList.Add(_dump);
    }

    public void RemoveMethod()
    {
        if(1 <= _atkCount)
        {
            _dump = _atkMethodList[_atkCount - 1];
            _atkMethodList.RemoveAt(_atkCount - 1);
            Destroy(_dump);
            _atkCount--;
        } else
        {
            Debug.LogError("����ȏ�폜����List������܂���B");
        }
    }

    //ScrollView�̉�ʂ�
    public void PhaseChange(State state)
    {
        switch (state)
        {
            case State.phase1:
                _textDetail.SetActive(true);
                _inputJson.SetActive(true);
                _specialThanks.SetActive(true);
                _registAtk.SetActive(false);
                _buttonStart.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("Next");
                _textTitle.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("TRPG�̐퓬�𔲂��o���Ă݂�ver3.2");
                break;
            case State.phase2:
                _textDetail.SetActive(false);
                _inputJson.SetActive(false);
                _specialThanks.SetActive(false);
                _registAtk.SetActive(true);
                _buttonStart.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("GameStart");
                _textTitle.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("�U�����@��ǉ����Ă�");
                break;
        }
    }

    public void MovePhase1()
    {
        PhaseChange(State.phase1);
        _state = State.phase1;
    }
    //���̓G���[�́u���������v���������Ƃ�
    public void ValidateErrorPanel(bool index)
    {
        _inputJson.GetComponent<TMP_InputField>().interactable = !index;
        _buttonStart.GetComponent<Button>().interactable = !index;
        _specialThanks.SetActive(!index);
        _validateErrorPanel.SetActive(index);
    }

    public void InputErrorPanel(bool index)
    {
        foreach(GameObject obj in _atkMethodList)
        {
            obj.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().interactable = !index;
            obj.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().interactable= !index;
            obj.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().interactable = !index;
            obj.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().interactable = !index;
            obj.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>().interactable = !index;
        }
        _buttonStart.GetComponent<Button>().interactable = !index;
        _inputErrorPanel.SetActive(index);
    }

    //�Q�[���I���̊m�F��ʂ��o���B
    public void EndGamePanelView(bool index)
    {
        _inputJson.GetComponent<TMP_InputField>().interactable = !index;
        _buttonStart.GetComponent<Button>().interactable = !index;
        _endGamePanel.SetActive(index);
    }

    //�Q�[���v���C�I��
    public void EndGame()
    {
    Application.Quit();
    }
}