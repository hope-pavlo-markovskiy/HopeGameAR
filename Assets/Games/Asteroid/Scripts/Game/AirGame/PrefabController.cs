using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabController : MonoBehaviour
{
    [SerializeField] Transform parentObject;
    [SerializeField] Transform destination;

    private Camera mainCamera;
    public float speed = 5f;
    private char character;
    private SpawnerController spawnerController;

    public float rotationSpeedMin = 10f;
    public float rotationSpeedMax = 50f;

    public bool spawn = true;

    public AudioSource sound;
    public bool played = false;
    public float customDist = 10f;

    [SerializeField] Boom explosion;
    [SerializeField] GameObject hologram;

    void Start()
    {
        mainCamera = Camera.main;
        spawnerController = FindObjectOfType<SpawnerController>();

        if (spawnerController == null)
        {
            Debug.LogError("SpawnerController was not found in the scene.");
        }

        int rand = Random.Range(0, 1);

/*        if(rand == 0)
        {
            destination = dest1;
        }
        else
        {
            destination = dest2;
        }*/

        //destination = dests[rand];

        // Generate a random rotation axis
        Vector3 randomRotationAxis = Random.onUnitSphere;

        // Generate a random rotation speed
        float randomRotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

        // Apply initial rotation to the asteroid
        transform.rotation = Random.rotation;

        // Start continuous rotation using a coroutine
        StartCoroutine(RotateAsteroid(randomRotationAxis, randomRotationSpeed));
        
    }

    IEnumerator RotateAsteroid(Vector3 rotationAxis, float rotationSpeed)
    {
        while (true)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

            yield return null;
        }
    }

    public void SetDestination(Transform position)
    {
        destination = position;
        spawn = true;
    }

    void Update()
    {
        if(spawn)
        {
            float dist = Vector3.Distance(transform.position, destination.position);

            //Debug.Log("DISTANCE: " + dist);

            if (dist < 1.5f)
            {
                AsteroidGame.Instance.asteroidsComplete += 1;
                Destroy(parentObject.gameObject);
            }
            else
            {

                //parentObject.position = parentObject.position + new Vector3(0f, 0f, -1f) * speed * Time.deltaTime;
                parentObject.position = Vector3.MoveTowards(parentObject.position, destination.position, speed * Time.deltaTime);
                if (dist < customDist && !played)
                {
                    played = true;
                }
                //Vector3.MoveTowards(parentObject.position, destination.position, speed * Time.deltaTime);
            }

            CheckKeyboardInput();
        }   
        else
        {
            parentObject.position = parentObject.position + new Vector3(0f, 0f, -1f) * speed * Time.deltaTime;

            if (this.transform.position.z < -7f)
                Destroy(parentObject.gameObject);
        }
    }

    public void Hit()
    {
        hologram.SetActive(false);
        explosion.gameObject.SetActive(true);
        Destroy(gameObject);

        //Destroy(gameObject);
    }

    public void SetCharacter(char c)
    {
        this.character = c;
    }

    private void CheckKeyboardInput()
    {
        if (spawnerController.input)
        {
            char inputChar = (spawnerController.inputString).ToLower()[0];

            Debug.Log(inputChar);

            if (inputChar == char.ToLower(character))
            {
                Destroy(gameObject);
            }

            spawnerController.input = false;
        }
    }
}

