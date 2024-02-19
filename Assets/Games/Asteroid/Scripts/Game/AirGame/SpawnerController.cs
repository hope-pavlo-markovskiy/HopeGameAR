using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public GameObject prefab; 
    public Camera mainCamera;
    public float spawnDistance = 20.0f;
    public float spawnRate = 4.0f;
    //public int hp = 100;
    public float time = 300f;
    //public Slider hpBar;
   //public Slider timeBar;
    public string inputString = "";
    public bool input = false;
    private float spawnAreaRadius = 5f;

    private float timer;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        timer = spawnRate; 
        StartCoroutine(SpawnGameObjects());
    }


    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0)
        {
            StopAllCoroutines();
        }
    }

    void SetHp()
    {
        //hpBar.value = (float)hp / 20;
    }
    void SetTime()
    {
        //timeBar.value = time / 30;
    }

    IEnumerator SpawnGameObjects()
    {
        while (true)
        {
            if (timer >= spawnRate)
            {
                //Vector3 spawnDirection = Random.insideUnitSphere.normalized;
                //Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance + spawnDirection * spawnAreaRadius;

                Vector3 spawnDirection = Random.onUnitSphere.normalized;

                // Ensure the Y coordinate is above half of the screen
                float spawnY = mainCamera.transform.position.y + Random.Range(Screen.height * 0.5f, Screen.height);
                Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(Random.value, spawnY / Screen.height, spawnDistance)) + spawnDirection * spawnAreaRadius;


                GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity);



                /*char randomChar = GetRandomChar();
                // јЩЙиДъµДІгґОЅб№№КЗ GameObject -> Canvas -> Text
                Text textComponent = newObject.transform.Find("Canvas").GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    textComponent.text = randomChar.ToString();
                    newObject.GetComponent<PrefabController>().SetCharacter(textComponent.text.ToLower()[0]);
                }
                else
                {
                    Debug.LogError("Text component not found on prefab!");
                }*/


                timer = 0;
            }

            timer += Time.deltaTime;
            yield return null; 
        }
    }

    char GetRandomChar()
    {
        return (char)('A' + Random.Range(0, 3));
    }
}


