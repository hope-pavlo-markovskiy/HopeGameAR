

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
    /// ��������
    /// </summary>
    private PhraseRecognizer phraseRecognizer;
    //��������
    private ConfidenceLevel confidenceLevel = ConfidenceLevel.Low;

    void Start()
    {
        if (phraseRecognizer == null)
        {
            //����ʶ����
            phraseRecognizer = new KeywordRecognizer(keywords, confidenceLevel);
            //ע���⵽������¼�
            phraseRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            print("��ʼ������������");
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
    /// ��⵽��Ӧ������¼�ʵ������
    /// </summary>
    /// <param name="args"></param>
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        //�˴����ʶ�𵽶����ؼ��ֵĲ���
        //Debug.Log(args.text);
        spawnerController.input = true;
        spawnerController.inputString = args.text;
    }

}


