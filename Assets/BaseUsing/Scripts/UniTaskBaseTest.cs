using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
    }
    
    private async void OnClickLoadScene()
    {

    }

    private async void OnClickLoadText()
    {
        var loadOperation = Resources.LoadAsync<TextAsset>("test");
        var text = await loadOperation;
        TargetText.text = ((TextAsset) text).text;
    }
}
