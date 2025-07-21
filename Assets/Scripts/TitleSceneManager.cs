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

    //念のため、最初に見せないものを非表示にするよう記述
    private void Start()
    {
        _endGamePanel.SetActive(false);
    }

    //Jsonクラスを作成してグローバル変数に保持、ゲーム開始。
    public void GameStart()
    {
        string _jsonText = _inputJson.GetComponent<TMP_InputField>().text;
        _player = _readJson.ReadJsonFile(_jsonText);

        //consoleで情報確認
        //Debug.LogFormat("--以下、各種確認を行います。");
        //Debug.LogFormat("プレイヤーネーム：{0}", _player.data.name);
        //Debug.LogFormat("commands：{0}", _player.data.commands);
        SceneManager.LoadScene("Battle");
        Debug.Log("バトルシーンへ遷移");
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