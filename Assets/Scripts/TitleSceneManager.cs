using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.Timeline;
public class TitleSceneManager : MonoBehaviour
{
    public enum State
    {
        phase1,
        phase2
    }
    //アクセス修飾子の後にstaticでグローバル化
    public PlayerCharacter _playerJson;
    public static Character _player;
    public static GameManager.PieceType pieceType;


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
    

    //念のため、最初に見せないものを非表示にするよう記述
    private void Start()
    {
        _endGamePanel.SetActive(false);
        _atkList.Add("こぶしで殴る");
        _atkList.Add("刃物で切りかかる");
        _atkList.Add("物を投げる");
    }

    private string _dumpText;
    private string _dumpText_dp1;
    private string _dumpText_dp2;
    private int _dumpInt;
    private string[] _dumpString_dice = new string[2];
    //プレイヤーを作成してゲーム開始。
    public void GameStart()
    {
        switch (_state)
        {
            case State.phase1:
                //////////////////
                //JSONの読み込み//
                //////////////////
                try
                {
                    string _jsonText = _inputJson.GetComponent<TMP_InputField>().text.Replace("\"params\":", "\"param\":");
                    _playerJson = _readJson.ReadJsonFile(_jsonText);
                    if (_playerJson == null) { return; }
                    Debug.Log("JSONの読み込み完了");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSONの読み込みに失敗");
                    Debug.Log(e.ToString());
                    ValidateErrorPanel(true);
                    return;
                }
                //////////////////////
                //NewCharacterの作成//
                //////////////////////
                _player = new Character(0, _playerJson.data.name, _playerJson.data.status[0].value, int.Parse(_playerJson.data.param[3].value), _playerJson.data.iconUrl, GameManager.CharacterKind.Player);
                ///////////////////
                //NewSkillsの作成//
                ///////////////////
                commands = _playerJson.data.commands.Split('\n');

                ///luckOutput
                ///幸運ダイスを出力：true
                ///幸運ダイスを出力しない：false
                ///
                ///CCorCCB
                ///CCB出力：true
                ///CC出力：false
                bool luckOutput = (_playerJson.data.status[3].label == "幸運");
                bool CCorCCB = commands[1].Contains("CCB");
                if (_playerJson.data.status[3].label == "幸運")
                {
                    if (commands[1].Contains("CCB")) { pieceType = GameManager.PieceType.luck_CCB; } else { pieceType = GameManager.PieceType.luck_CC; }
                }
                else
                {
                    if (commands[1].Contains("CCB")) { pieceType = GameManager.PieceType.Noluck_CCB; } else { pieceType = GameManager.PieceType.Noluck_CC; }
                }

                Regex regex = new Regex(@"(^$|STR|CON|POW|DEX|APP|SIZ|INT|EDU|正気度ロール|ダメージ判定)");
                string marker;
                if (CCorCCB) { marker = "CCB<="; } else { marker = "CC<="; }
                string dump_text;
                int startIndex;
                int endIndex;
                foreach (string command in commands)
                {
                    //特定のcommandは処理しない
                    if (regex.IsMatch(command.ToString())) { continue; }
                    Debug.LogFormat("make command：{0}", command);
                    //数値の抜き出し
                    startIndex = command.IndexOf(marker);
                    endIndex = command.IndexOf(" 【");

                    if (startIndex != -1 && endIndex != -1)
                    {
                        startIndex += marker.Length;
                        dump_text = command.Substring(startIndex, endIndex - startIndex);
                    }
                    else
                    {//エラー
                        Debug.LogError("NewSkillの作成に失敗しました！");
                        Debug.LogErrorFormat("コマンド名：{0}",command);
                        return;
                    }
                    if(luckOutput){ dump_text = _playerJson.data.status[3].value.ToString(); }
                    Debug.LogFormat("dump_text = {0}", dump_text);
                    string[] parts = command.Split(new char[] { '【', '】' });
                    if (parts[1] == "応急手当") { 
                        _player.skills.Add(new Heal(command, "応急手当", int.Parse(dump_text), AudioManager.Move.FirstAid, 3)); 
                    } else
                    {
                        _player.skills.Add(new Skill(command, "", int.Parse(dump_text), AudioManager.Move.None));
                    }
                }

                ///////////////////////
                //１．オプション追加 //
                ///////////////////////
                PhaseChange(State.phase2);
                _optionList.Clear();
                foreach (Skill skill in _player.skills){ _optionList.Add(skill.diceText); }
                _dropdown = _methodPref.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Dropdown>();
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
                        //各要素の取り出し//
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
                        Debug.LogError("エラー。入力ミス？");
                        Debug.Log(e.ToString());
                        InputErrorPanel(true);
                        return;
                    }
                    ////////////////
                    //Weaponの作成//
                    ////////////////
                    _dumpInt = _player.skills.FindIndex(skill => skill.diceText == _dumpText_dp1);
                    switch (_dumpText_dp2)
                    {
                        case"こぶしで殴る":
                            _player.weapons.Add(new Weapon(_dumpText, _dumpText_dp1, _player.skills[_dumpInt].successNum, int.Parse(_dumpString_dice[0]), int.Parse(_dumpString_dice[1]), AudioManager.Move.Panch));
                            break;
                        case"刃物で切りかかる":
                            _player.weapons.Add(new Weapon(_dumpText, _dumpText_dp1, _player.skills[_dumpInt].successNum, int.Parse(_dumpString_dice[0]), int.Parse(_dumpString_dice[1]), AudioManager.Move.Knife));
                            break;
                        case "物を投げる":
                            _player.weapons.Add(new Weapon(_dumpText, _dumpText_dp1, _player.skills[_dumpInt].successNum, int.Parse(_dumpString_dice[0]), int.Parse(_dumpString_dice[1]), AudioManager.Move.Throw));
                            break;
                    }
                }
                Debug.Log("シーン遷移します。");
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
            Debug.LogError("これ以上削除するListがありません。");
        }
    }

    //ScrollViewの画面へ
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
                _textTitle.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("TRPGの戦闘を抜き出してみたver3.2");
                break;
            case State.phase2:
                _textDetail.SetActive(false);
                _inputJson.SetActive(false);
                _specialThanks.SetActive(false);
                _registAtk.SetActive(true);
                _buttonStart.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("GameStart");
                _textTitle.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("攻撃方法を追加してね");
                break;
        }
    }

    public void MovePhase1()
    {
        PhaseChange(State.phase1);
        _state = State.phase1;
    }
    //入力エラーの「分かった」を押したとき
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
    //ゲーム終了の確認画面を出す。
    public void EndGamePanelView(bool index)
    {
        _inputJson.GetComponent<TMP_InputField>().interactable = !index;
        _buttonStart.GetComponent<Button>().interactable = !index;
        _endGamePanel.SetActive(index);
    }
    //ゲームプレイ終了
    public void EndGame() { Application.Quit(); }
}