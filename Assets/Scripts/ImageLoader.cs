using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // 2DゲームのSpriteRenderer用
    public IEnumerator LoadImage(GameObject obj, string url)
    {
        Debug.LogFormat("url:{0}",url);
        if (url.Contains("https"))
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            { Debug.LogError("画像のダウンロードに失敗しました: " + request.error); }
            else
            {
                // ダウンロードしたテクスチャを取得
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                // Texture2DからSpriteを作成
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                obj.GetComponent<Image>().sprite = sprite;
            }
        } else
        {
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(url);
        }
            yield return null;
    }
}