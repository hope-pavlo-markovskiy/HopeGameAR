

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

/// <summary>
/// Voice recognition
/// </summary>
public class SpeechRecognition : MonoBehaviour
{
    public SpawnerController spawnerController;
    /// <summary>
    /// key words
    /// </summary>
    public string[] keywords = { "one", "two" };
    /// <summary>
    /// 语音监听
    /// </summary>
    private PhraseRecognizer phraseRecognizer;
    //监听级别
    private ConfidenceLevel confidenceLevel = ConfidenceLevel.Low;

    void Start()
    {
        if (phraseRecognizer == null)
        {
            //语音识别器
            phraseRecognizer = new KeywordRecognizer(keywords, confidenceLevel);
            //注册检测到短语的事件
            phraseRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            print("开始监听语音输入");
            phraseRecognizer.Start();
        }
    }

    private void OnDestroy()
    {
        if (phraseRecognizer != null)
        {
            phraseRecognizer.Dispose();
        }
    }


    /// <summary>
    /// 检测到对应短语的事件实例函数
    /// </summary>
    /// <param name="args"></param>
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        //此处添加识别到对饮关键字的操作
        //Debug.Log(args.text);
        spawnerController.input = true;
        spawnerController.inputString = args.text;
    }

}


