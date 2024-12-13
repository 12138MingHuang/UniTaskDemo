using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ThreadSample : MonoBehaviour
{
    public Button StandardRun;
    public Button YieldRun;

    public Text Text;
    
    private void Start()
    {
        StandardRun.onClick.AddListener(UniTask.UnityAction(OnClickStandardRun));
        YieldRun.onClick.AddListener(UniTask.UnityAction(OnClickYieldRun));
    }
    
    private async UniTaskVoid OnClickStandardRun()
    {
        int result = 0;
        await UniTask.RunOnThreadPool(() =>
        {
            result = 10;
        });
        await UniTask.SwitchToMainThread();
        Text.text = result.ToString();
    }
    
    private async UniTaskVoid OnClickYieldRun()
    {
        string fileName = Application.dataPath + "/BaseUsingNext/test.txt";
        await UniTask.SwitchToThreadPool();
        string fileContent = await File.ReadAllTextAsync(fileName);
        await UniTask.Yield(PlayerLoopTiming.Update);
        Text.text = fileContent;
    }
}
