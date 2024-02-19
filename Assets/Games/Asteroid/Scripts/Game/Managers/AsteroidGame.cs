using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class AsteroidGame : GameManager
{
    public static AsteroidGame Instance;

    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private Transform[] destLocations;

    [SerializeField] private Transform[] decoyLocations;

    [SerializeField] private GameObject asteroidBasePrefab;
    [SerializeField] private Transform asteroidBaseContainer;

    [SerializeField] private ShipController shipController;

    [SerializeField] private GameObject asterBG;

    [SerializeField] private GameObject dustParticle;

    public float spawnRate = 4.0f, spawnRate2 = 4.0f;
    public float gameDuration = 60f;
    private float timer, timer2;

    [SerializeField] AudioSource playerSpawnAS;

    public int asteroidsComplete = 0;
    [SerializeField] private int asteroidsRequired = 0;

    [SerializeField] private PlayableDirector timeline;

    public bool spawn = false;

    public float delayTime = 3f;

    [SerializeField] private GameObject playerQuad;
    [SerializeField] private GameObject teleporter;

    [SerializeField] private GameObject lookAudio;

    [SerializeField] private GameObject ziggyWoo;
    [SerializeField] private GameObject spaceshipWoosh;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        timer = spawnRate;
    }

    void Start()
    {
        timeline.stopped += TimelineEnd;
        PythonProcessor.Instance.StartProgram();
    }

    void Update()
    {
        if(asteroidsComplete > asteroidsRequired)
        {
            StopAllCoroutines();

            Debug.Log("Game Complete");
        }
    }
    

    async void TimelineEnd(PlayableDirector dir)
    {
        Debug.LogError("TimelineEnd");
        //StartCoroutine(Delay(delayTime));
        await TeleporterDelay(1);
       //playerSpawnAS.Play();

       await Delay(delayTime);
    }

    async Task TeleporterDelay(float delay)
    {
        
        await Task.Delay((int)(delay * 1000));
        
        teleporter.SetActive(true);

        await Task.Delay((int)(1 * 1000));
       
        playerQuad.SetActive(true);

        lookAudio.SetActive(true);
    }

    async Task Delay(float delay)
    {
        
        await Task.Delay((int)(delay * 1000));

        CameraSwitcher.Instance.ToggleCamera();

        ziggyWoo.SetActive(true);
        spaceshipWoosh.SetActive(true);

        ToggleShipControl();

        asterBG.SetActive(true);

        dustParticle.SetActive(true);

        if (spawn)
        {
            StartCoroutine(SpawnAsteroids());

            StartCoroutine(SpawnDecoy());
        }
    }

    public void ToggleShipControl()
    {
        Debug.LogError("ToggleShipControl");
        shipController.enabled = true;
    }

    IEnumerator SpawnAsteroids()
    {
        Debug.LogError("SpawnAsteroids");
        while (asteroidsComplete < asteroidsRequired)
        {
            if (timer >= spawnRate)
            {
                int rand = Random.Range(0, 2);

                GameObject newObject = Instantiate(asteroidBasePrefab, spawnLocations[rand].position, Quaternion.identity);
                newObject.SetActive(true);
                newObject.transform.parent = asteroidBaseContainer;
                newObject.transform.GetComponentInChildren<PrefabController>().SetDestination(destLocations[rand]);

                timer = 0;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpawnDecoy()
    {
        Debug.LogError("SpawnDecoy");
        while(true)
        {
            if(timer2 >= spawnRate2)
            {
                int rand = Random.Range(0, 4);

                GameObject newObject = Instantiate(asteroidBasePrefab, decoyLocations[rand].position, Quaternion.identity);
                newObject.SetActive(true);
                newObject.transform.parent = asteroidBaseContainer;
                newObject.transform.GetComponentInChildren<PrefabController>().spawn = false;

                timer2 = 0;

            }

            timer2 += Time.deltaTime;
            yield return null;
        }
    }

   /* IEnumerator SpawnObjectsByCamera()
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



                char randomChar = GetRandomChar();
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
                }


                timer = 0;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }*/
}
