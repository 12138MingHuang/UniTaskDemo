using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UniTaskCancelTest : MonoBehaviour
{
    public Runner FirstRunner;
    public Runner SecondRunner;
    
    public Button FirstRunButton;
    public Button SecondRunButton;
    public Button ResetButton;
    public Button FirstCancelButton;
    public Button SecondCancelButton;
    public Text FirstText;
    public Text SecondText;

    public float TotalDistance = 15;
    
    private CancellationTokenSource _firstCancelToken;
    private CancellationTokenSource _secondCancelToken;
    private CancellationTokenSource _linkCancelToken;
    
    private void Start()
    {
        FirstRunButton.onClick.AddListener(OnClickFirstRun);
        SecondRunButton.onClick.AddListener(OnClickSecondRun);
        FirstCancelButton.onClick.AddListener(OnClickFirstCancel);
        SecondCancelButton.onClick.AddListener(OnClickSecondCancel);
        ResetButton.onClick.AddListener(OnClickReset);
        
        _firstCancelToken = new CancellationTokenSource();
        // 注意这里可以直接先行设置多久以后取消
        // _firstCancelToken = new CancellationTokenSource(TimeSpan.FromSeconds(1.5f));
        _secondCancelToken = new CancellationTokenSource();
        _linkCancelToken = CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
    }

    private async UniTask<int> RunSomeOne(Runner runner, CancellationToken cancelToken)
    {
        runner.Reset();
        float totalTime = TotalDistance / runner.Speed;
        float timeElapsed = 0;
        while (timeElapsed <= totalTime)
        {
            timeElapsed += Time.deltaTime;
            await UniTask.NextFrame(cancelToken);
            
            float runDistance = Mathf.Min(timeElapsed, totalTime) * runner.Speed;
            runner.Target.position = runner.StartPos + Vector3.right * runDistance;
        }
        
        runner.ReachGoal = true;
        return 0;
    }
    
    private async void OnClickFirstRun()
    {
        try
        {
            await RunSomeOne(FirstRunner, _firstCancelToken.Token);
        }
        catch (OperationCanceledException e)
        {
            FirstText.text = "一号跑已经被取消";
        }
    }
    
    private async void OnClickSecondRun()
    {
        var (cancelled, _) = await RunSomeOne(SecondRunner, _linkCancelToken.Token).SuppressCancellationThrow();
        if (cancelled)
        {
            SecondText.text = "二号跑已经被取消";
        }
    }
    
    private void OnClickFirstCancel()
    {
        _firstCancelToken.Cancel();
        _firstCancelToken.Dispose();
        _firstCancelToken = new CancellationTokenSource();
        _linkCancelToken = CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
    }
    
    private void OnClickSecondCancel()
    {
        _secondCancelToken.Cancel();
        _secondCancelToken.Dispose();
        _secondCancelToken = new CancellationTokenSource();
        _linkCancelToken = CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
    }
    
    private void OnClickReset()
    {
        _firstCancelToken.Cancel();
        _firstCancelToken = new CancellationTokenSource();
        _secondCancelToken = new CancellationTokenSource();
        _linkCancelToken = CancellationTokenSource.CreateLinkedTokenSource(_firstCancelToken.Token, _secondCancelToken.Token);
        FirstRunner.Reset();
        SecondRunner.Reset();
        FirstText.text = string.Empty;
        SecondText.text = string.Empty;
    }

    private void OnDestroy()
    {
        _firstCancelToken.Dispose();
        _secondCancelToken.Dispose();
        _linkCancelToken.Dispose();
    }
}
