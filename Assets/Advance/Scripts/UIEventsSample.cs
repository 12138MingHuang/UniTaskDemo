using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIEventsSample : MonoBehaviour
{
    public float DoubleClickCheckTime = 0.5f;

    public Button DoubleClickButton;
    public Text DoubleEventText;

    public Button CoolDownButton;
    public Text CoolDownEventText;

    public Button SphereButton;
    
    public float CoolDownTime = 3.0f;

    private void Start()
    {
        CheckDoubleClickButton(DoubleClickButton, this.GetCancellationTokenOnDestroy()).Forget();
        CheckSphereClick(SphereButton.GetCancellationTokenOnDestroy()).Forget();
        CheckCooldownClickButton(this.GetCancellationTokenOnDestroy()).Forget();
    }
    private async UniTaskVoid CheckDoubleClickButton(Button doubleClickButton, CancellationToken token)
    {
        while (true)
        {
            var clickAsync = doubleClickButton.OnClickAsync(token);
            await clickAsync;
            DoubleEventText.text = "按钮被第一次点击";
            var secondClickAsync = doubleClickButton.OnClickAsync(token);
            int resultIndex = await UniTask.WhenAny(secondClickAsync, UniTask.Delay(TimeSpan.FromSeconds(DoubleClickCheckTime), cancellationToken: token));
            if (resultIndex == 0)
            {
                DoubleEventText.text = "按钮被双击了";
            }
            else
            {
                DoubleEventText.text = "超时，按钮算作单击";
            }
        }
    }
    
    private async UniTask<int> CheckSphereClick(CancellationToken token)
    {
        var asyncEnumerable = SphereButton.OnClickAsAsyncEnumerable();
        await asyncEnumerable.Take(3).ForEachAsync((_, index) =>
        {
            if(token.IsCancellationRequested) return;
            if (index == 0)
            {
                SphereTweenScale(2, SphereButton.transform.localScale.x, 20,token).Forget();
            }
            else if (index == 1)
            {
                SphereTweenScale(2, SphereButton.transform.localScale.x, 10, token).Forget();
            }
        }, token);
        
        GameObject.Destroy(SphereButton.gameObject);
        return 0;
    }
    private async UniTaskVoid SphereTweenScale(float totalTime, float from, float to, CancellationToken token)
    {
        var trans = SphereButton.transform;
        float time = 0f;
        while (time < totalTime)
        {
            time += Time.deltaTime;
            trans.localScale = (from + (time / totalTime) * (to - from)) * Vector3.one;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private async UniTaskVoid CheckCooldownClickButton(CancellationToken token)
    {
        var asyncEnumerable = CoolDownButton.OnClickAsAsyncEnumerable();
        await asyncEnumerable.ForEachAwaitAsync(async (_) =>
        {
            CoolDownEventText.text = "被点击了，开始冷却";
            await UniTask.Delay(TimeSpan.FromSeconds(CoolDownTime), cancellationToken: token);
            CoolDownEventText.text = "冷却完毕，可以再次点击了";
        }, cancellationToken: token);
    }
}
