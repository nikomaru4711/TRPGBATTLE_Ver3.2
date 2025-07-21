using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    private int i;
    public static PlayerCharacter _player;
    [SerializeField] private ReadJson _readJson;
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private GameObject _inputJson;
    [SerializeField] private GameObject _buttonStart;

    //�O�̂��߁A�ŏ��Ɍ����Ȃ����̂��\���ɂ���悤�L�q
    private void Start()
    {
        _endGamePanel.SetActive(false);
    }

    //Json�N���X���쐬���ăO���[�o���ϐ��ɕێ��A�Q�[���J�n�B
    public void GameStart()
    {
        string _jsonText = _inputJson.GetComponent<TMP_InputField>().text;
        _player = _readJson.ReadJsonFile(_jsonText);

        //console�ŏ��m�F
        //Debug.LogFormat("--�ȉ��A�e��m�F���s���܂��B");
        //Debug.LogFormat("�v���C���[�l�[���F{0}", _player.data.name);
        //Debug.LogFormat("commands�F{0}", _player.data.commands);
        SceneManager.LoadScene("Battle");
        Debug.Log("�o�g���V�[���֑J��");
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