using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class UniTaskWaitTest : MonoBehaviour
{
    public PlayerLoopTiming TestYieldTiming = PlayerLoopTiming.PreUpdate;
    
    public Button TestDelayButton;
    public Button TestDelayFrameButton;
    public Button TestYieldButton;
    public Button TestNextFrameButton;
    public Button TestEndOfFrameButton;
    public Button ClearButton;
    public Text ShowLogText;
    
    private List<PlayerLoopSystem.UpdateFunction> _injectUpdateFunctions = new List<PlayerLoopSystem.UpdateFunction>();
    
    private bool _showUpdateLog = false;

    private void Start()
    {
        TestDelayButton.onClick.AddListener(OnClickTestDelay);
        TestDelayFrameButton.onClick.AddListener(OnClickTestDelayFrame);
        TestYieldButton.onClick.AddListener(OnClickTestYield);
        TestNextFrameButton.onClick.AddListener(OnClickTestNextFrame);
        TestEndOfFrameButton.onClick.AddListener(OnClickTestEndOfFrame);
        ClearButton.onClick.AddListener(OnClickClear);
        
        InjectFunction();
    }

    private async void OnClickTestDelay()
    {
        Debug.Log($"执行Delay开始，当前时间{Time.time}");
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        Debug.Log($"执行Delay结束，当前时间{Time.time}");
    }
    
    private async void OnClickTestDelayFrame()
    {
        Debug.Log($"执行DelayFrame开始，当前帧{Time.frameCount}");
        await UniTask.DelayFrame(10);
        Debug.Log($"执行DelayFrame结束，当前帧{Time.frameCount}");
    }
    
    private async void OnClickTestYield()
    {
        _showUpdateLog = true;
        Debug.Log($"执行yield开始{TestYieldTiming}");
        await UniTask.Yield(TestYieldTiming);
        Debug.Log($"执行yield结束{TestYieldTiming}");
        _showUpdateLog = false;
    }

    private void OnClickClear()
    {
        ShowLogText.text = "Log:";
    }
    
    private async void OnClickTestNextFrame()
    {
        _showUpdateLog = true;
        Debug.Log($"执行NextFrame开始");
        await UniTask.NextFrame();
        Debug.Log($"执行NextFrame结束");
        _showUpdateLog = false;
    }
    
    private async void OnClickTestEndOfFrame()
    {
        _showUpdateLog = true;
        Debug.Log($"执行EndOfFrame开始");
        await UniTask.WaitForEndOfFrame(this);
        Debug.Log($"执行EndOfFrame结束");
        _showUpdateLog = false;
    }

    private void InjectFunction()
    {
        PlayerLoopSystem playerLoop = PlayerLoop.GetDefaultPlayerLoop();
        PlayerLoopSystem[] subSystems = playerLoop.subSystemList;
        playerLoop.updateDelegate += OnUpdate;
        for (int i = 0; i < subSystems.Length; i++)
        {
            int index = i;
            PlayerLoopSystem.UpdateFunction injectFunction = () =>
            {
                if(!_showUpdateLog) return;
                Debug.Log($"执行子系统 {_showUpdateLog} {subSystems[index]} 当前帧 {Time.frameCount}");
            };
            _injectUpdateFunctions.Add(injectFunction);
            subSystems[i].updateDelegate += injectFunction;
        }
        
        PlayerLoop.SetPlayerLoop(playerLoop);
    }
    private void OnUpdate()
    {
        Debug.Log($"当前帧{Time.frameCount}");
    }
    private void UnInjectFunction()
    {
        PlayerLoopSystem playerLoop = PlayerLoop.GetDefaultPlayerLoop();
        playerLoop.updateDelegate -= OnUpdate;
        for (int i = 0; i < _injectUpdateFunctions.Count; i++)
        {
            playerLoop.subSystemList[i].updateDelegate -= _injectUpdateFunctions[i];
        }
        
        PlayerLoop.SetPlayerLoop(playerLoop);
        _injectUpdateFunctions.Clear();
    } 
    
    private void ShowLog(string condition, string stackTrace, LogType type)
    {
        ShowLogText.text = $"{ShowLogText.text}\n{condition}";
    }

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += ShowLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= ShowLog;
    }

    private void OnDestroy()
    {
        UnInjectFunction();
    }
}
