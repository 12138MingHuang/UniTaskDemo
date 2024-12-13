using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TimeoutTest : MonoBehaviour
{
    public Button TestButton;
    public Text[] Texts;
    
    public string SearchWord = "Unity";
    public string[] SearchURLs = new string[]
    {
        "https://www.baidu.com/s?wd=",
        "https://www.bing.com/search?q=",
        "https://www.google.com/search?q=",
    };

    private void Start()
    {
        TestButton.onClick.AddListener(UniTask.UnityAction(OnClickTest));
    }
    
    private async UniTaskVoid OnClickTest()
    {
        UniTask<string>[] waitTasks = new UniTask<string>[SearchURLs.Length];
        for (int i = 0; i < SearchURLs.Length; i++)
        {
            waitTasks[i] = GetRequest(SearchURLs[i], 2f);
        }
        
        var tasks = await UniTask.WhenAll(waitTasks);
        for (int i = 0; i < tasks.Length; i++)
        {
            Texts[i].text = tasks[i];
        }
    }

    private async UniTask<string> GetRequest(string url, float timeOut)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(timeOut));

        var (cancelOrFailed, result) = await UnityWebRequest.Get(url).SendWebRequest().WithCancellation(cts.Token).SuppressCancellationThrow();
        if(!cancelOrFailed)
        {
            return result.downloadHandler.text.Substring(0, 100);
        }
        
        return "请求超时";
    }
}
