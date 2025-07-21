using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    //0�Ԃ���A�oHP, DEX, ���}�蓖, ���Ԃ�, ����, ����p�������Ă�
    [SerializeField] private TMP_InputField[] _input = new TMP_InputField[6];
    private int i;
    [SerializeField] private GameObject _warningText_01;
    [SerializeField] private GameObject _warningText_02;
    public static int[] _playerStatus = new int[6];

    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private GameObject _buttonStart;

    private void Start()
    {
        _warningText_01.SetActive(false);
        _warningText_02.SetActive(false);
        _endGamePanel.SetActive(false);
    }
    //�l�̑��(�O���[�o��)�ƃV�[���J��
    public void GameStart()
    {
        for (i = 0; i < 6; i++)
        {
            if (!Validate(_input[i].text))
            {
                return;
            }
            _playerStatus[i] = int.Parse(_input[i].text);
        }

        SceneManager.LoadScene("Battle");
    }

    //�o���f�[�V����
    private bool Validate(string content)
    {
        _warningText_01.SetActive(false);
        _warningText_02.SetActive(false);

        if (string.Compare(content, "") == 0)
        {
            _warningText_01.SetActive(true);
            return false;
        }
        else if (string.Compare(content, "-") == 0)
        {
            _warningText_02.SetActive(true);
            return false;
        }

        int num = int.Parse(content);

        if (num < 0 || num >= 1000)
        {
            _warningText_02.SetActive(true);
            return false;
        }
        return true;
    }


    public void EndGamePanelView(bool index)
    {//if���򂵂ăQ�[���X�^�[�g�Ƃ���interactable = false�ɂ��Ȃ���...!
        for (i = 0; i < _input.Length; i++)
        {
            _input[i].GetComponent<TMP_InputField>().interactable = !index;
        }
        _buttonStart.GetComponent<Button>().interactable = !index;
        _endGamePanel.SetActive(index);
    }
    public void EndGame()
    {
    Application.Quit();//�Q�[���v���C�I��
    }
}