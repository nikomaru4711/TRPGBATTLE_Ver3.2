using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class ImageToSpriteLoader : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // 2DゲームのSpriteRenderer用

    IEnumerator LoadImage(GameObject obj, string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        { Debug.LogError("画像のダウンロードに失敗しました: " + www.error);}
        else
        {
            // ダウンロードしたテクスチャを取得
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            // Texture2DからSpriteを作成
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            obj.GetComponent<Image>().sprite = sprite;
        }
        yield return null;
    }
}