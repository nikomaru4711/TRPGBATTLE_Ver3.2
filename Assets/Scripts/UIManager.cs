using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //�X�N���v�g�̃C���|�[�g
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioManager _audioManager;

    //����^private
    [SerializeField] private GameObject _logContent;
    [SerializeField] private GameObject _logPref_1Line;
    [SerializeField] private GameObject _logPref_2Line;
    [SerializeField] private GameObject _logPref_3Line;
    [SerializeField] private GameObject _logPref_4Line;
    [SerializeField] private GameObject _fightPanel;
    [SerializeField] private GameObject _actPanel;
    [SerializeField] private GameObject _logScroll;
    [SerializeField] private GameObject _iconBasePref;
    [SerializeField] private GameObject _enemyBasePref;
    [SerializeField] private GameObject _fightContent;
    [SerializeField] private GameObject _actContent;
    [SerializeField] private GameObject _buttonPref;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _contentLog;
    [SerializeField] private GameObject _buttonFight;
    [SerializeField] private GameObject _buttonAct;
    [SerializeField] private GameObject _whitePanel;
    [SerializeField] private GameObject _blackPanel;

    //���̑�
    private RectTransform _rectTransform;
    private int _id = 1;
    public ObjectInfo _index;
    [System.NonSerialized] public GameObject _playerIcon;
    private GameObject _enemyAppearance;
    private GameObject _canvas;
    private GameObject _hpText;
    private GameObject _image;
    private GameObject _text;
    private GameObject _character;
    private int _screenvariable = 80;
    private int i;
    public List<ObjectInfo> _displayOrder = new List<ObjectInfo>();

    void Start()
    {
        //���IUI�̏�����
        _fightPanel.SetActive(false);
        _actPanel.SetActive(false);
        _whitePanel.transform.GetChild(0).gameObject.SetActive(false);
        _whitePanel.SetActive(false);
        _blackPanel.transform.GetChild(0).gameObject.SetActive(false);
        _blackPanel.SetActive(false);
    }

    //���O�̐���
    public void CreateLog(string message, Line line, int textsize = 40)
    {
        GameObject _text;
        switch (line)
        {
            case Line.Line1:
                _text = Instantiate(_logPref_1Line);
                break;
            case Line.Line2:
                _text = Instantiate(_logPref_2Line);
                break;
            case Line.Line3:
                _text = Instantiate(_logPref_3Line);
                break;
            case Line.Line4:
                _text = Instantiate(_logPref_4Line);
                break;
            default:
                CreateLog("<color=red>Error!</color>���O�o�͂��ł��܂���ł����B", Line.Line1);
                return;
        }
        _text.name = "Log_" + _id;
        _text.transform.SetParent(_logContent.transform);
        _text.GetComponent<TMP_Text>().SetText(message);
        _text.GetComponent<TMP_Text>().fontSize = textsize;
        StartCoroutine("AutoScroll");
        _id++;
    }
    //�X�N���[���o�[����ԉ��ɍX�V����
    //���X�N���[��rect���X�V����̂�1�t���[��������炵���̂�1�t���[���҂������p�̃R���[�`��
    private IEnumerator AutoScroll()
    {
        yield return new WaitForEndOfFrame();
        _scrollRect.verticalNormalizedPosition = 0;

    }

    //����ɕ\�������L�����N�^�[�𐶐�
    public void CreateIcon(Character character)
    {
        //�eGameObject�̐ݒ�
        _canvas = GameObject.Find("Canvas");
        _playerIcon = Instantiate(_iconBasePref, new Vector2(0, 0), Quaternion.identity);
        _playerIcon.name = "Character_" + character.id;
        _playerIcon.transform.SetParent(_canvas.transform);
        _playerIcon.transform.localPosition = new Vector2(0, 0);

        //�摜�ݒ�
        _image = GameObject.Find("Icon");
        _image.name = "Icon_" + character.id;
        if (character.imagePath != null)
        {
            _image.GetComponent<Image>().sprite = Resources.Load<Sprite>(character.imagePath);
        }
        _text = GameObject.Find("Text_DEX");
        _text.name = "Text_DEX_" + character.id;
        _text.GetComponent<TMP_Text>().SetText(character.dex.ToString());

        //HP�Q�[�W�̐ݒ�
        _hpText = GameObject.Find("Text_HP");
        _hpText.name = "Text_HP_" + character.id;
        _hpText.GetComponent<TMP_Text>().SetText("  HP      " + character.currentHP.ToString() + "/" + character.maxHP.ToString());
        _image = GameObject.Find("HP_Gauge");
        _image.name = "HPGauge_" + character.id;

        //DisplayOrder�p�̃��X�g�ɓ���邽�߂ɐ�p�̍\���̂��g��

        _index = new ObjectInfo(_playerIcon, character.dex, character.id);
        _displayOrder.Add(_index);
        _displayOrder.Sort((x, y) => y._dex - x._dex);
        for (i = 0; i < _displayOrder.Count; i++)
        {
            if (i == 0)
            {
                _displayOrder[i]._gameObject.transform.localPosition = new Vector2(0, 0);
            }
            else
            {
                _displayOrder[i]._gameObject.transform.localPosition = new Vector2(0, -200);
            }
        }

    }

    public void CreateEnemyAppearance(Character character)
    {
        _enemyAppearance = Instantiate(_enemyBasePref);
        _enemyAppearance.transform.SetParent(_canvas.transform);
        _enemyAppearance.transform.localPosition = new Vector2(-10.0f,-50.0f);
        _enemyAppearance.name = "Enemy_" + character.id;
        if(character.imagePath != null)
        {
            _enemyAppearance.GetComponent<Image>().sprite = Resources.Load<Sprite>(character.imagePath);
        }
    }

    //_fightPanel�ɕ\�������{�^���̐���
    public void CreateButton(Weapon weapon)
    {
        if(weapon == null)
        {
            Debug.LogError("weapon[" + i + "] �� null �ł�");
        }
        GameObject button = Instantiate(_buttonPref);
        //�e��ݒ�A�I�u�W�F�N�g���̕ύX�A�e�L�X�g�ҏW�A�C�x���g�ݒ�
        button.transform.SetParent(_fightContent.transform);
        button.name = "button_" + weapon.actionName;
        if(weapon.successNum > 99)
        {
            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  <size=34>CCB<=" + weapon.successNum + "�y" + weapon.actionName + "�z</size>");
        } else
        {
            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  CCB<=" + weapon.successNum + "�y" + weapon.actionName + "�z");
        }
        button.GetComponent<Button>().onClick.AddListener(() => 
        {
            _gameManager.StartCoroutine("AttackManage",weapon);
            IsInteractable(false);
        });
    }

    //_actPanel�ɕ\�������{�^���̐���
    public void CreateButton(Skill skill)
    {
        if(skill.name != "���")
        {
            GameObject button = Instantiate(_buttonPref);
            //�e��ݒ�A�I�u�W�F�N�g���̕ύX�A�e�L�X�g�ҏW�A�C�x���g�ݒ�
            button.transform.SetParent(_actContent.transform);
            button.name = "button_" + skill.actionName;
            if (skill.successNum > 99)
            {
                button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  <size=32>CCB<=" + skill.successNum + "�y" + skill.actionName + "�z</size>");
            } else
            {
                button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  <size=34>CCB<=" + skill.successNum + "�y" + skill.actionName + "�z</size>");
            }

            button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _gameManager.StartCoroutine("MoveManage", skill);
                    IsInteractable(false);
                });
        }
    }
    //updateindex�ɂ͂��̂܂܂̒l�u+2�v��u-3�v�Ȃǂ������đ�����
    public void UpdateCharacterHP(Character character, int updateindex)
    {
        int oldHP = character.currentHP;

        //�̗͂��ő�l�E�ŏ��l�𒴂��Ȃ��悤�ɁB
        character.currentHP += updateindex;
        if (character.currentHP > character.maxHP)
        {
            character.currentHP = character.maxHP;
        }
        else if (character.currentHP <= 0)
        {
            character.currentHP = 0;
        }

        Debug.LogFormat("�󂯎����index: {0}", updateindex);
        //�L�����N�^�[�ɑΉ�����UI�̎擾
        _character = GameObject.Find("Character_" + character.id);
        //Log�Ƀ_���[�W�̏o��
        CreateLog("�y" + character.name + "�zHP : " +  oldHP + "��" + character.currentHP , Line.Line1);

        //�Q�[�W�̍X�V
        //���񂾂�����
        if (character.currentHP == 0)
        {
            _character.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().SetText("  HP      <color=red>0</color>" + "/" + character.maxHP);
            _character.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = ((float)character.currentHP / (float)character.maxHP);
            //�G��������
            if (character.kind == GameManager.CharacterKind.Enemy)
            {
                _character = GameObject.Find("Enemy_" + character.id);
                _character.GetComponent<Image>().sprite = Resources.Load<Sprite>("Enemy_Dead");
                character.isDead = false;
                DeadProcess(character);
                //��
                Debug.Log("�G�����񂾂� ggwp");
                //�Q�[���N���A
            }
            else if(character.kind == GameManager.CharacterKind.Player)//Player��������
            {
                //�Ó]
                //�Q�[���I�[�o�[�̕\�L
                //��
                Debug.Log("Player�����񂾂� ggwp");
                character.isDead = false;
                DeadProcess(character);
            }
        } else //�܂������c���Ă���
        {
            _character.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = ((float)character.currentHP / (float)character.maxHP);
            if (character.currentHP / character.maxHP <= 0.8)
            {
                _character.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().SetText("  HP      <color=red>" + character.currentHP + "</color>/" + character.maxHP);
            } else
            {
                _character.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().SetText("  HP      " + character.currentHP + "/" + character.maxHP);
            }
                
            Debug.Log("�����c���Ă񂶂�� nc");
        }
    }

    //�L�����N�^�[�̎��S����
    public void DeadProcess(Character character)
    {
        //Player��������
        if(character.kind == GameManager.CharacterKind.Player)
        {
            //�Q�[���I�[�o�[����
            StartCoroutine("GameOverProcces");
        } 
        else
        {//�G��������
            //�Q�[���N���A����
            //  ����͓G����l�������Ȃ��̂ŏ�ɏo�Ă�G�̐��𒲂ׂ鏈���͏����Ȃ��B
            _audioManager.EnemyDeadSound();
            StartCoroutine("ClearProcess");
        }
    }

    public void onClickBackTitle()
    {
        SceneManager.LoadScene("Title");
    }

    //_fightButton���������Ƃ��̏���
    public void onClickFIGHT()
    {
        if (_actPanel.activeSelf)
        {
            _actPanel.SetActive(false);
        }
        else
        {
            ShrinkLogPanel();
        }
        _fightPanel.SetActive(true);
    }

    //_actButton���������Ƃ��̏���
    public void onClickACT()
    {
        if (_fightPanel.activeSelf)
        {
            _fightPanel.SetActive(false);
        }
        else
        {
            ShrinkLogPanel();
        }
        _actPanel.SetActive(true);
    }

    //_fightButton�A_actButton�́uBack�v���������Ƃ��̏���
    public void onClickBack()
    {
        if (_fightPanel.activeSelf)
        {
            _fightPanel.SetActive(false);
        }
        else if (_actPanel.activeSelf)
        {
            _actPanel.SetActive(false);
        }
        StretchLogPanel();
    }

    //_logScroll��L�΂�
    public void StretchLogPanel()
    {
        _rectTransform = _logScroll.GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, 0);
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, 1080);
    }

    //_logScroll���k�߂�
    public void ShrinkLogPanel()
    {
        _rectTransform = _logScroll.GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, -240);
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, 600);
    }

    //�{�^���̗L����/��L�����̏���
    public void  IsInteractable(bool index)
    {
        if(index)
        {
            _buttonFight.GetComponent<Button>().interactable = true;
            _buttonAct.GetComponent<Button>().interactable = true;
        }
        else
        {
            _buttonFight.GetComponent<Button>().interactable = false;
            _buttonAct.GetComponent<Button>().interactable = false;
            onClickBack();
        }
    }

    //�N���A���̏���
    public IEnumerator ClearProcess()
    {
        yield return new WaitForSeconds(1.0f);
        //�t�F�[�h�C��
        _whitePanel.SetActive(true);
        _whitePanel.transform.SetAsLastSibling();
        for (i = 0;i < 256;i++)
        {
            _whitePanel.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, (float)i / (float)255);
            if(i < 80) {
                yield return new WaitForSeconds(0.02f);
            } 
            else
            {
                yield return new WaitForSeconds(0.015f);
            }
                
        }
        //UI�X�V
        GameObject.Find("BattleBackground_0").GetComponent<Image>().sprite = Resources.Load<Sprite>("ClearBackground");
        _fightPanel.SetActive(false);
        _actPanel.SetActive(false);
        //GameObject.Find("").SetActive(false);
        GameObject.Find("Log").SetActive(false);
        GameObject.Find("Button_Fight").SetActive(false);
        GameObject.Find("Button_Act").SetActive(false);
        for (i = 0; _gameManager._characterArray[i] != null;i++)
        {
            if(_gameManager._characterArray[i].kind == GameManager.CharacterKind.Enemy)
            {
                GameObject.Find("Enemy_" + _gameManager._characterArray[i].id).SetActive(false);
            }
            GameObject.Find("Character_" + _gameManager._characterArray[i].id).SetActive(false);
        }
        //���̍Đ�
        _audioManager.ChangeBGM(AudioManager.BGMType.Clear);
        //�t�F�[�h�A�E�g
        for (i = 255; i > 0; i--)
        {
            _whitePanel.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, (float)i / (float)255);
            if (i < 100)
            {
                yield return new WaitForSeconds(0.025f);
            }
            else
            {
                yield return new WaitForSeconds(0.005f);
            }

        }
        yield return new WaitForSeconds(1.5f);
        _whitePanel.transform.GetChild(0).gameObject.SetActive(true);
        yield break;
    }

    //�Q�[���I�[�o�[����
    public IEnumerator GameOverProcces()
    {
        _blackPanel.SetActive(true);
        _blackPanel.transform.SetAsLastSibling();
        for (i = 0; i < 256; i++)
        {
            _blackPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, (float)i / (float)255);
            if (i < _screenvariable)
            {
                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                yield return new WaitForSeconds(0.015f);
            }
        }
        //���̍Đ�
        _audioManager.ChangeBGM(AudioManager.BGMType.GameOver);
        yield return new WaitForSeconds(1.25f);
        _blackPanel.transform.GetChild(0).gameObject.SetActive(true);
    }

    //Log���o�͂��鎞�ɉ��s�g�p���邩������
    public enum Line
    {
        Line1,
        Line2,
        Line3,
        Line4,
    }
}