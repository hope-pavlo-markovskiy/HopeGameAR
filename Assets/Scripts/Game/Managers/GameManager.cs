using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class GameManager : MonoBehaviour
{
    //[SerializeField] private LevelManager levelManager;

    //ACtivaTE TIMELINE if bool is selected
    //potential for SO to plugin intro/outro set
    //private bool hasTimeline = false;


    protected void Awake()
    {
        
    }

    protected void Start()
    {
        
    }

    protected void OnEnable()
    {
        
    }

    protected void OnDisable()
    {
        
    }

    protected void Update()
    {
        
    }





}

#if UNITY_EDITOR

/*[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        GameManager gameManager = (GameManager)target;

        gameManager.has
    }
}
*/
#endif