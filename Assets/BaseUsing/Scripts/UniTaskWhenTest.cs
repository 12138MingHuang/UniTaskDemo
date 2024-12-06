using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniTaskWhenTest : MonoBehaviour
{
    public Runner FirstRunner;
    public Runner SecondRunner;
    
    public Button FirstRunButton;
    public Button SecondRunButton;
    public Button WhenAllButton;
    public Button WhenAnyButton;
    public Button ResetButton;
    public Text WinnerText;
    public Text CompleteText;

    public float TotalDistance = 15;

    private void Start()
    {
        FirstRunButton.onClick.AddListener(OnClickFirstRun);
        SecondRunButton.onClick.AddListener(OnClickSecondRun);
        WhenAllButton.onClick.AddListener(OnClickWhenAll);
        WhenAnyButton.onClick.AddListener(OnClickWhenAny);
        ResetButton.onClick.AddListener(OnClickReset);
    }

    private void OnClickReset()
    {
        FirstRunner.Reset();
        SecondRunner.Reset();
        CompleteText.text = "";
        WinnerText.text = "";
    }

    private async UniTask RunSomeOne(Runner runner)
    {
        runner.Reset();
        float totalTime = TotalDistance / runner.Speed;
        float timeElapsed = 0;
        while (timeElapsed < totalTime)
        {
            timeElapsed += Time.deltaTime;
            await UniTask.NextFrame();
            float runDistance = Mathf.Min(timeElapsed, totalTime) * runner.Speed;
            runner.Target.position = runner.StartPos + Vector3.right * runDistance;
        }
        
        runner.ReachGoal = true;
    }


    private async void OnClickFirstRun()
    {
        await RunSomeOne(FirstRunner);
    }

    private async void OnClickSecondRun()
    {
        await RunSomeOne(SecondRunner);
    }

    private async void OnClickWhenAll()
    {
        
    }

    private async void OnClickWhenAny()
    {
        
    }
}
