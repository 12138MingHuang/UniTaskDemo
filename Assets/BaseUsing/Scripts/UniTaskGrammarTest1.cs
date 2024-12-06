using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class UniTaskGrammarTest1 : MonoBehaviour
{
    async UniTask<string> DemoAsync()
    {
        //可以await Unity async对象
        var asset = await Resources.LoadAsync<TextAsset>("test");
        var text = (await UnityWebRequest.Get("https://....").SendWebRequest()).downloadHandler.text;
        await SceneManager.LoadSceneAsync("SampleScene");
        
        //WithCancellation 启用取消方法，GetCancellationTokenOnDestroy 与 GameObject 的生命周期同步
        var asset2 = await Resources.LoadAsync<TextAsset>("test").WithCancellation(this.GetCancellationTokenOnDestroy());
        
        //ToUniTask 接收进度回调(和完整的参数) Progress.Create是IProgress<T>的轻量级替代品
        var asset3 = await Resources.LoadAsync<TextAsset>("test").ToUniTask(Progress.Create<float>(x => Debug.Log("进度: " + x)));
        
        //像协程一样，是await基于帧的操作
        await UniTask.DelayFrame(10);
        
        //替换yield return new WaitForSeconds / WaitForSecondsRealtime
        await UniTask.Delay(TimeSpan.FromSeconds(10));
        
        //产生任何游戏循环时间（PreUpdate，Update，LateUpdate...）
        await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);
        
        //替换yield return null
        await UniTask.Yield();
        await UniTask.NextFrame();
        
        //替换WaitForEndOfFrame(需要 MonoBehaviour(CoroutineRunner))
        await UniTask.WaitForEndOfFrame(this); //this就是MonoBehaviour
        
        //替换yield return new WaitForFixedUpdate() 同UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        await UniTask.WaitForFixedUpdate();
        
        //替换WaitUntil
        bool isActive = true;
        await UniTask.WaitUntil(() => isActive == false);
        
        //WaitUntil的帮助方法
        //await UniTask.WaitUntilValueChanged(this, x => {});
        
        //可以await IEnumerator
        // await FooCoroutineEnumerator();
        
        //await C# 标准Task
        await Task.Run(() => 100);
        
        //多线程，此代码运行在ThreadPool上
        await UniTask.SwitchToTaskPool();
        
        //在线程池上工作  返回主线程
        await UniTask.SwitchToMainThread();
        
        //获取async网络请求
        async UniTask<string> GetTextAsync(UnityWebRequest req)
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        
        var task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
        var task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
        var task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));
        
        //并发async await并且通过元组来获取结果
        var (goole, bing, yahoo) = await UniTask.WhenAll(task1, task2, task3);
        
        //whenAll的简写
        var (goole2, bing2, yahoo2) = await (task1, task2, task3);
        
        return goole + bing + yahoo;
    }
    
    /// <summary>
    /// WithCancellation 启用取消方法，GetCancellationTokenOnDestroy 与 GameObject 的生命周期同步
    /// 正常加载：如果 GameObject 存活，资源成功加载并打印内容。
    /// 取消加载：如果在加载过程中销毁了 GameObject，任务会自动取消，并捕获到 OperationCanceledException。
    /// </summary>
    private async UniTaskVoid Start()
    {
        try
        {
            // 异步加载资源，同时绑定取消令牌到当前 GameObject 生命周期
            var asset = await Resources.LoadAsync<TextAsset>("bar")
                .WithCancellation(this.GetCancellationTokenOnDestroy());
            
            if (asset != null)
            {
                Debug.Log("资源加载完成: " + ((TextAsset)asset).text);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("资源加载任务被取消，因为 GameObject 已销毁！");
        }
    }

}
