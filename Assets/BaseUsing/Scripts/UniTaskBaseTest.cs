using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniTaskBaseTest : MonoBehaviour
{
    public Button LoadTextButton;
    public Text TargetText;

    public Button LoadSceneButton;
    public Slider LoadSceneSlider;
    public Text ProgressText;

    public Button WebRequestButton;
    public Image DownloadImage;

    private void Start()
    {
        LoadTextButton.onClick.AddListener(OnClickLoadText);
        LoadSceneButton.onClick.AddListener(OnClickLoadScene);
        WebRequestButton.onClick.AddListener(OnClickWebRequest);
    }

    private async void OnClickWebRequest()
    {
        var webRequest = UnityWebRequestTexture.GetTexture("https://s1.hdslb.com/bfs/static/jinkela/video/asserts/33-coin-ani.png");
        var result = await webRequest.SendWebRequest();
        var texture = ((DownloadHandlerTexture)result.downloadHandler).texture;
        int totalSpriteCount = 24;
        int perSpriteWidth = texture.width / totalSpriteCount;
        Sprite[] sprites = new Sprite[totalSpriteCount];
        for (int i = 0; i < totalSpriteCount; i++)
        {
            sprites[i] = Sprite.Create(texture,
                new Rect(new Vector2(perSpriteWidth * i, 0), new Vector2(perSpriteWidth, texture.height)), new Vector2(0.5f, 0.5f));
        }
        
        float perFrameTime = 0.1f;
        while (true)
        {
            for (int i = 0; i < totalSpriteCount; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(perFrameTime));
                var sprite = sprites[i];
                DownloadImage.sprite = sprite;
            }
        }
    }

    private void OnClickLoadScene()
    {
        SceneManager.LoadSceneAsync("Scenes/SampleScene").ToUniTask((Progress.Create<float>(
            (p) =>
            {
                LoadSceneSlider.value = p;
                if (ProgressText != null)
                {
                    ProgressText.text = $"读取进度{p * 100:F2}%";
                }
            })));
    }

    private async void OnClickLoadText()
    {
        var loadOperation = Resources.LoadAsync<TextAsset>("test");
        var text = await loadOperation;
        TargetText.text = ((TextAsset)text).text;
    }
}
