using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UniRx;
public class TitleSceneManager : MonoBehaviour
{
    //アクセス修飾子の後にstaticでグローバル化
    public PlayerCharacter _playerJson;
    public NewSkill[] _skills = new NewSkill[50];
    public static NewCharacter _player;

[SerializeField] private ReadJson _readJson;
    [SerializeField] private GameObject _textTitle;
    [SerializeField] private GameObject _textDetail;
    [SerializeField] private GameObject _specialThanks;
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private GameObject _inputJson;
    [SerializeField] private GameObject _buttonStart;
    [SerializeField] private GameObject _validateErrorPanel;
    private string[] commands = new string[64];

    //念のため、最初に見せないものを非表示にするよう記述
    private void Start()
    {
        _endGamePanel.SetActive(false);
    }

    //プレイヤーを作成してゲーム開始。
    public void GameStart()
    {
        //////////////////
        //JSONの読み込み//
        //////////////////
        try
        {
            string _jsonText = _inputJson.GetComponent<TMP_InputField>().text.Replace("\"params\":", "\"param\":");
            _playerJson = _readJson.ReadJsonFile(_jsonText);
            if (_playerJson == null){ return; }
            Debug.Log("JSONの読み込み完了");
        }
        catch (System.Exception e)
        {
            Debug.Log("JSONの読み込みに失敗");
            Debug.Log(e.ToString());
            ValidateErrorPanel(true);
            return;
        }


        ///////////////////
        //NewSkillsの作成//
        ///////////////////
        for (int i = 0;i < commands.Length;i++)
        {
            commands = _playerJson.data.commands.Split('\n');
        }
        NewSkill dump_newskill;
        string marker = "CC<=";
        string dump_text;
        int startIndex;
        int endIndex;
        int index = 0;
        for (int i = 0; i < commands.Length -12; i++)
        {
            if (i == 1 || (2 < i && 50 < i) ) 
            {
                startIndex = commands[i].IndexOf(marker);
                endIndex = commands[i].IndexOf(" 【");

                if (startIndex != -1 && endIndex != -1)
                {
                    startIndex += marker.Length;
                    dump_text = commands[i].Substring(startIndex, endIndex - startIndex);
                }
                else
                {//エラー
                    Debug.LogError("NewSkillの作成に失敗しました！");
                    Debug.LogErrorFormat("i = {0}\ncommand[{1}] = {2}", i, i, commands[i]);
                    return;
                }
                Debug.LogFormat("i = {0}を作成します。", i);
                Debug.LogFormat("dump_text = {0}", dump_text);
                dump_newskill = new NewSkill(commands[i], int.Parse(dump_text));
                _skills[index] = dump_newskill;
                index++;
            } else 
            {
                
            }
            
        }
        //////////////////////
        //NewCharacterの作成//
        //////////////////////
        _player = new NewCharacter(0, _playerJson.data.name, _playerJson.data.status[0].value, int.Parse(_playerJson.data.param[3].value), _playerJson.data.iconUrl, _skills, GameManager.CharacterKind.Player);
        //////////////////
        //攻撃方法の記入//
        //////////////////
        //IEnumerator Atk = AddAtk();

        Observable.FromCoroutine(AddAtk)
            .Subscribe(_ =>
            {
                Debug.Log("バトルシーンへ遷移");
                //SceneManager.LoadScene("Battle");
            }).AddTo(this);

        
    }
    //攻撃方法に関して記載する。
    private IEnumerator AddAtk()
    {
        _textTitle.SetActive(false);
        _textDetail.SetActive(false);
        _inputJson.SetActive(false);
        Debug.Log("２秒待ち");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("完了！");
    }

    //入力エラーの「分かった」を押したとき
    public void ValidateErrorPanel(bool index)
    {
        _inputJson.GetComponent<TMP_InputField>().interactable = !index;
        _buttonStart.GetComponent<Button>().interactable = !index;
        _specialThanks.SetActive(!index);
        _validateErrorPanel.SetActive(index);
    }

    //ゲーム終了の確認画面を出す。
    public void EndGamePanelView(bool index)
    {
        _inputJson.GetComponent<TMP_InputField>().interactable = !index;
        _buttonStart.GetComponent<Button>().interactable = !index;
        _endGamePanel.SetActive(index);
    }

    //ゲームプレイ終了
    public void EndGame()
    {
    Application.Quit();
    }
}