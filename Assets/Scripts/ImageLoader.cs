using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class ImageToSpriteLoader : MonoBehaviour
{
    /// <summary>
    /// テスト用
    /// </summary>
    //[SerializeField] private GameObject img;
    //private void Start()
    //{
    //    Debug.Log("LoadImage実行");
    //    StartCoroutine(LoadImage(img, "https://image.iaproject.app/040927e6-f2ba-4c05-a104-b7c5e5d6f0c7"));
    //    Debug.Log("DONE.");
    //}

    private SpriteRenderer spriteRenderer; // 2DゲームのSpriteRenderer用
    IEnumerator LoadImage(GameObject obj, string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        { Debug.LogError("画像のダウンロードに失敗しました: " + request.error);}
        else
        {
            // ダウンロードしたテクスチャを取得
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            // Texture2DからSpriteを作成
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            obj.GetComponent<Image>().sprite = sprite;
        }
        yield return null;
    }
}