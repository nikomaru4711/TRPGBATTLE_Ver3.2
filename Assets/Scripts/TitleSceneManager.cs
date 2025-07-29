using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UniRx;
public class TitleSceneManager : MonoBehaviour
{
    //�A�N�Z�X�C���q�̌��static�ŃO���[�o����
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

    //�O�̂��߁A�ŏ��Ɍ����Ȃ����̂��\���ɂ���悤�L�q
    private void Start()
    {
        _endGamePanel.SetActive(false);
    }

    //�v���C���[���쐬���ăQ�[���J�n�B
    public void GameStart()
    {
        //////////////////
        //JSON�̓ǂݍ���//
        //////////////////
        try
        {
            string _jsonText = _inputJson.GetComponent<TMP_InputField>().text.Replace("\"params\":", "\"param\":");
            _playerJson = _readJson.ReadJsonFile(_jsonText);
            if (_playerJson == null){ return; }
            Debug.Log("JSON�̓ǂݍ��݊���");
        }
        catch (System.Exception e)
        {
            Debug.Log("JSON�̓ǂݍ��݂Ɏ��s");
            Debug.Log(e.ToString());
            ValidateErrorPanel(true);
            return;
        }


        ///////////////////
        //NewSkills�̍쐬//
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
                dump_newskill = new NewSkill(commands[i], int.Parse(dump_text));
                _skills[index] = dump_newskill;
                index++;
            } else 
            {
                
            }
            
        }
        //////////////////////
        //NewCharacter�̍쐬//
        //////////////////////
        _player = new NewCharacter(0, _playerJson.data.name, _playerJson.data.status[0].value, int.Parse(_playerJson.data.param[3].value), _playerJson.data.iconUrl, _skills, GameManager.CharacterKind.Player);
        //////////////////
        //�U�����@�̋L��//
        //////////////////
        //IEnumerator Atk = AddAtk();

        Observable.FromCoroutine(AddAtk)
            .Subscribe(_ =>
            {
                Debug.Log("�o�g���V�[���֑J��");
                //SceneManager.LoadScene("Battle");
            }).AddTo(this);

        
    }
    //�U�����@�Ɋւ��ċL�ڂ���B
    private IEnumerator AddAtk()
    {
        _textTitle.SetActive(false);
        _textDetail.SetActive(false);
        _inputJson.SetActive(false);
        Debug.Log("�Q�b�҂�");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("�����I");
    }

    //���̓G���[�́u���������v���������Ƃ�
    public void ValidateErrorPanel(bool index)
    {
        _inputJson.GetComponent<TMP_InputField>().interactable = !index;
        _buttonStart.GetComponent<Button>().interactable = !index;
        _specialThanks.SetActive(!index);
        _validateErrorPanel.SetActive(index);
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