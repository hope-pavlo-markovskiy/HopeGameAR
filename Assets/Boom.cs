using UnityEngine;

public class Boom : MonoBehaviour
{
    public float delay = 3f;
    [SerializeField] private float _scale = 0.5f;
    [SerializeField] private float _speed = 1f;

    float countdown;

    public bool hasExploded = false;

    public GameObject explosionEffect;


    // Start is called before the first frame update
    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
    }
}
