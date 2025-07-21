using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //スクリプトのインポート
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioManager _audioManager;

    //代入型private
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

    //その他
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
        //動的UIの初期化
        _fightPanel.SetActive(false);
        _actPanel.SetActive(false);
        _whitePanel.transform.GetChild(0).gameObject.SetActive(false);
        _whitePanel.SetActive(false);
        _blackPanel.transform.GetChild(0).gameObject.SetActive(false);
        _blackPanel.SetActive(false);
    }

    //ログの生成
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
                CreateLog("<color=red>Error!</color>ログ出力ができませんでした。", Line.Line1);
                return;
        }
        _text.name = "Log_" + _id;
        _text.transform.SetParent(_logContent.transform);
        _text.GetComponent<TMP_Text>().SetText(message);
        _text.GetComponent<TMP_Text>().fontSize = textsize;
        StartCoroutine("AutoScroll");
        _id++;
    }
    //スクロールバーを一番下に更新する
    //※スクロールrectを更新するのに1フレームかかるらしいので1フレーム待たせる専用のコルーチン
    private IEnumerator AutoScroll()
    {
        yield return new WaitForEndOfFrame();
        _scrollRect.verticalNormalizedPosition = 0;

    }

    //左上に表示されるキャラクターを生成
    public void CreateIcon(Character character)
    {
        //親GameObjectの設定
        _canvas = GameObject.Find("Canvas");
        _playerIcon = Instantiate(_iconBasePref, new Vector2(0, 0), Quaternion.identity);
        _playerIcon.name = "Character_" + character.id;
        _playerIcon.transform.SetParent(_canvas.transform);
        _playerIcon.transform.localPosition = new Vector2(0, 0);

        //画像設定
        _image = GameObject.Find("Icon");
        _image.name = "Icon_" + character.id;
        if (character.imagePath != null)
        {
            _image.GetComponent<Image>().sprite = Resources.Load<Sprite>(character.imagePath);
        }
        _text = GameObject.Find("Text_DEX");
        _text.name = "Text_DEX_" + character.id;
        _text.GetComponent<TMP_Text>().SetText(character.dex.ToString());

        //HPゲージの設定
        _hpText = GameObject.Find("Text_HP");
        _hpText.name = "Text_HP_" + character.id;
        _hpText.GetComponent<TMP_Text>().SetText("  HP      " + character.currentHP.ToString() + "/" + character.maxHP.ToString());
        _image = GameObject.Find("HP_Gauge");
        _image.name = "HPGauge_" + character.id;

        //DisplayOrder用のリストに入れるために専用の構造体を使う

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

    //_fightPanelに表示されるボタンの生成
    public void CreateButton(Weapon weapon)
    {
        if(weapon == null)
        {
            Debug.LogError("weapon[" + i + "] が null です");
        }
        GameObject button = Instantiate(_buttonPref);
        //親を設定、オブジェクト名の変更、テキスト編集、イベント設定
        button.transform.SetParent(_fightContent.transform);
        button.name = "button_" + weapon.actionName;
        if(weapon.successNum > 99)
        {
            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  <size=34>CCB<=" + weapon.successNum + "【" + weapon.actionName + "】</size>");
        } else
        {
            button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  CCB<=" + weapon.successNum + "【" + weapon.actionName + "】");
        }
        button.GetComponent<Button>().onClick.AddListener(() => 
        {
            _gameManager.StartCoroutine("AttackManage",weapon);
            IsInteractable(false);
        });
    }

    //_actPanelに表示されるボタンの生成
    public void CreateButton(Skill skill)
    {
        if(skill.name != "回避")
        {
            GameObject button = Instantiate(_buttonPref);
            //親を設定、オブジェクト名の変更、テキスト編集、イベント設定
            button.transform.SetParent(_actContent.transform);
            button.name = "button_" + skill.actionName;
            if (skill.successNum > 99)
            {
                button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  <size=32>CCB<=" + skill.successNum + "【" + skill.actionName + "】</size>");
            } else
            {
                button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText("  <size=34>CCB<=" + skill.successNum + "【" + skill.actionName + "】</size>");
            }

            button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _gameManager.StartCoroutine("MoveManage", skill);
                    IsInteractable(false);
                });
        }
    }
    //updateindexにはそのままの値「+2」や「-3」などが入って送られる
    public void UpdateCharacterHP(Character character, int updateindex)
    {
        int oldHP = character.currentHP;

        //体力が最大値・最小値を超えないように。
        character.currentHP += updateindex;
        if (character.currentHP > character.maxHP)
        {
            character.currentHP = character.maxHP;
        }
        else if (character.currentHP <= 0)
        {
            character.currentHP = 0;
        }

        Debug.LogFormat("受け取ったindex: {0}", updateindex);
        //キャラクターに対応するUIの取得
        _character = GameObject.Find("Character_" + character.id);
        //Logにダメージの出力
        CreateLog("【" + character.name + "】HP : " +  oldHP + "→" + character.currentHP , Line.Line1);

        //ゲージの更新
        //死んだか判定
        if (character.currentHP == 0)
        {
            _character.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().SetText("  HP      <color=red>0</color>" + "/" + character.maxHP);
            _character.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = ((float)character.currentHP / (float)character.maxHP);
            //敵だったら
            if (character.kind == GameManager.CharacterKind.Enemy)
            {
                _character = GameObject.Find("Enemy_" + character.id);
                _character.GetComponent<Image>().sprite = Resources.Load<Sprite>("Enemy_Dead");
                character.isDead = false;
                DeadProcess(character);
                //音
                Debug.Log("敵が死んだわ ggwp");
                //ゲームクリア
            }
            else if(character.kind == GameManager.CharacterKind.Player)//Playerだったら
            {
                //暗転
                //ゲームオーバーの表記
                //音
                Debug.Log("Playerが死んだわ ggwp");
                character.isDead = false;
                DeadProcess(character);
            }
        } else //まだ生き残ってたら
        {
            _character.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = ((float)character.currentHP / (float)character.maxHP);
            if (character.currentHP / character.maxHP <= 0.8)
            {
                _character.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().SetText("  HP      <color=red>" + character.currentHP + "</color>/" + character.maxHP);
            } else
            {
                _character.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().SetText("  HP      " + character.currentHP + "/" + character.maxHP);
            }
                
            Debug.Log("生き残ってんじゃん nc");
        }
    }

    //キャラクターの死亡処理
    public void DeadProcess(Character character)
    {
        //Playerだったら
        if(character.kind == GameManager.CharacterKind.Player)
        {
            //ゲームオーバー処理
            StartCoroutine("GameOverProcces");
        } 
        else
        {//敵だったら
            //ゲームクリア処理
            //  今回は敵が一人しかいないので場に出てる敵の数を調べる処理は書かない。
            _audioManager.EnemyDeadSound();
            StartCoroutine("ClearProcess");
        }
    }

    public void onClickBackTitle()
    {
        SceneManager.LoadScene("Title");
    }

    //_fightButtonを押したときの処理
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

    //_actButtonを押したときの処理
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

    //_fightButton、_actButtonの「Back」を押したときの処理
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

    //_logScrollを伸ばす
    public void StretchLogPanel()
    {
        _rectTransform = _logScroll.GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, 0);
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, 1080);
    }

    //_logScrollを縮める
    public void ShrinkLogPanel()
    {
        _rectTransform = _logScroll.GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, -240);
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, 600);
    }

    //ボタンの有効化/非有効化の処理
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

    //クリア時の処理
    public IEnumerator ClearProcess()
    {
        yield return new WaitForSeconds(1.0f);
        //フェードイン
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
        //UI更新
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
        //音の再生
        _audioManager.ChangeBGM(AudioManager.BGMType.Clear);
        //フェードアウト
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

    //ゲームオーバー処理
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
        //音の再生
        _audioManager.ChangeBGM(AudioManager.BGMType.GameOver);
        yield return new WaitForSeconds(1.25f);
        _blackPanel.transform.GetChild(0).gameObject.SetActive(true);
    }

    //Logを出力する時に何行使用するかを示す
    public enum Line
    {
        Line1,
        Line2,
        Line3,
        Line4,
    }
}