using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BezierPathFollower : MonoBehaviour
{
    [SerializeField] private Transform particleEmitter;
    [SerializeField] private List<ParticleSystem> particles;
    [SerializeField] private List<ParticleSystem> leadParticles;

    [SerializeField] private GameObject currentShape;

    //[SerializeField] private List<BezierPath> bezierPaths;
    [SerializeField] private BezierPath[] _bezierPaths;

    [SerializeField] private BezierPath _bezierPath;
    [SerializeField] float _speed = 1;
    [SerializeField] bool _followOnEnable;

    [SerializeField] bool _loop;

    [SerializeField] bool _applyRotation = true;

    BezierWaypoint _currWaypoint;
    BezierWaypoint _nextWaypoint;

    bool _isMoving;
    IEnumerator _following;
    float _initialSpeed;


    public float Speed { get => _speed; set => _speed = value; }
    public bool IsMoving => _isMoving;

    public event Action OnFinishLine;



    void Awake()
    {
        _initialSpeed = _speed;

        if (currentShape != null)
            currentShape = null;

        Array.Clear(_bezierPaths, 0, _bezierPaths.Length);

    }

    private void Start()
    {
        //if (_followOnEnable)
        //    StartFollowing();
    }

    void OnEnable()
    {
        //if (_followOnEnable)
        //    StartFollowing();

        if(currentShape != null) 
        {
            //bezierPaths = new List<BezierPath>();

            
        }
    }

    public bool isBusy = false;

    public void SetShape(GameObject shape, BezierPath[] paths)
    {
        isBusy = true;

        currentShape = shape;

        _bezierPaths = paths;//currentShape.GetComponentsInChildren<BezierPath>();

        if (_followOnEnable)
            StartFollowing();
    }

    private GameObject currentShape2;

    public void SetShapes(GameObject shape1, GameObject shape2, BezierPath[] paths)
    {
        isBusy = true;

        currentShape = shape1;
        currentShape2 = shape2;

        _bezierPaths = paths; //currentShape.GetComponentsInChildren<BezierPath>();

        if (_followOnEnable)
            StartFollowing();
    }

    public void SetPathAndFollowIt(BezierPath bezierCurve, bool applyRotation = true)
    {
        _applyRotation = applyRotation;
        _bezierPath = bezierCurve;

        StartFollowing(applyRotation);
    }

    public void StartFollowing(bool applyRotation = true)
    {
        _applyRotation = applyRotation;

        _isMoving = true;

        if (_following != null)
            StopCoroutine(_following);

        StartCoroutine(_following = Following());
    }

    public void PauseFollowing()
    {
        _isMoving = false;
    }

    public void ResumeFollowing()
    {
        _isMoving = true;
    }

    public void RestartFollowing()
    {
        StartFollowing();
    }

    public void StopFollowing()
    {
        StopCoroutine(_following);

        OnFinishLine?.Invoke();
    }

    IEnumerator Following()
    {
        int pathIndex = 0;
        int numPaths = _bezierPaths.Length;
        float _moveValue = 0;

        bool drawing = true;

        bool complete = true;

        foreach (ParticleSystem particle in particles)
        {
            var emission = particle.emission;
            emission.rateOverDistance = 0;
        }

        particleEmitter.transform.position = _bezierPaths[pathIndex].GetPointAt(_moveValue); //_bezierPath.GetPointAt(_moveValue);


        while (drawing)
        {
            //particleEmitter.transform.position = _bezierPaths[pathIndex].GetPointAt(_moveValue);

            if (!_isMoving || complete)
            {
                yield return new WaitForSeconds(0.1f);


                foreach (ParticleSystem particle in particles)
                {
                    var emission = particle.emission;
                    emission.rateOverDistance = 200;
                }

                foreach(ParticleSystem particle in leadParticles)
                {
                    var emission = particle.emission;
                    emission.enabled = true;
                }

                _isMoving = true;
                complete = false;

                yield return null;
                continue;
            }

            _moveValue = Mathf.MoveTowards(_moveValue, 0.99f, _speed * 0.01f * Time.deltaTime);

            particleEmitter.transform.position = _bezierPaths[pathIndex].GetPointAt(_moveValue); //_bezierPath.GetPointAt(_moveValue);

            if (_currWaypoint && _applyRotation)
            {
                var distanceDiffrence = particleEmitter.transform.position - _currWaypoint.position;
                Quaternion quaternion = Quaternion.LookRotation(distanceDiffrence, Vector3.up);

                particleEmitter.transform.rotation = Quaternion.Lerp(particleEmitter.transform.rotation, quaternion, 5 * Time.deltaTime);
            }


            if (_currWaypoint != _bezierPaths[pathIndex].GetCurrentWaypoint(_moveValue))
            {
                _currWaypoint = _bezierPaths[pathIndex].GetCurrentWaypoint(_moveValue);

                if (_currWaypoint.TryGetComponent(out BezierWaypointEvents events))
                    events.OverrideSpeed(this);
                else
                    _speed = _initialSpeed;


            }

            if (_nextWaypoint != _bezierPaths[pathIndex].GetNextWaypoint(_moveValue))
            {
                _nextWaypoint = _bezierPaths[pathIndex].GetNextWaypoint(_moveValue);
            }


            yield return null;

            if (_moveValue >= 0.99f)
            {
                pathIndex++;

                if (!complete && pathIndex < numPaths)
                {
                    foreach (ParticleSystem particle in particles)
                    {
                        var emission = particle.emission;
                        emission.rateOverDistance = 0;
                    }

                    particleEmitter.transform.position = _bezierPaths[pathIndex].GetPoints()[0].position;

                    yield return new WaitForSeconds(0.1f);
                    _isMoving = true;

                    //particleEmitter.transform.position = _bezierPaths[pathIndex].GetPoints()[0].position;

                    complete = true;
                }


                if (pathIndex >= _bezierPaths.Length)
                {
                    drawing = false;
                    isBusy = false;

                    foreach (ParticleSystem particle in leadParticles)
                    {
                        var emission = particle.emission;
                        emission.enabled = false;
                    }

                    break;
                }

                _moveValue = 0;
            }


        }

        //if (_loop)
        //RestartFollowing();

        OnFinishLine?.Invoke();
    }

    //WORKING
    /*IEnumerator Following()
    {
        int pathIndex = 0;
        int numPaths = _bezierPaths.Length;
        float _moveValue = 0;


        while (pathIndex < numPaths)
        {
            if (!_isMoving)
            {
                yield return null;
                continue;
            }

            Debug.Log($"BezierPathFollower: Number of paths: {numPaths} :: {pathIndex}");


            _moveValue = Mathf.MoveTowards(_moveValue, 0.99f, _speed * 0.01f * Time.deltaTime);

            particleEmitter.transform.position = _bezierPaths[pathIndex].GetPointAt(_moveValue); //_bezierPath.GetPointAt(_moveValue);



            if (_currWaypoint && _applyRotation)
            {
                var distanceDiffrence = particleEmitter.transform.position - _currWaypoint.position;
                Quaternion quaternion = Quaternion.LookRotation(distanceDiffrence, Vector3.up);

                particleEmitter.transform.rotation = Quaternion.Lerp(particleEmitter.transform.rotation, quaternion, 5 * Time.deltaTime);
            }


            if (_currWaypoint != _bezierPaths[pathIndex].GetCurrentWaypoint(_moveValue))
            {
                _currWaypoint = _bezierPaths[pathIndex].GetCurrentWaypoint(_moveValue);

                if (_currWaypoint.TryGetComponent(out BezierWaypointEvents events))
                    events.OverrideSpeed(this);
                else
                    _speed = _initialSpeed;


            }

            if (_nextWaypoint != _bezierPaths[pathIndex].GetNextWaypoint(_moveValue))
            {
                _nextWaypoint = _bezierPaths[pathIndex].GetNextWaypoint(_moveValue);
            }


            yield return null;

            if (_moveValue >= 0.99f)
            {
                pathIndex++;
                _moveValue = 0;
            }

        }

        Debug.Log("BezierPathFollower: While loop broken");

        //if (_loop)
            //RestartFollowing();

        OnFinishLine?.Invoke();
    }*/


    /*[SerializeField] private List<BezierPath> paths = new List<BezierPath>();
    [SerializeField] private float speed = 1f;

    [SerializeField] private bool loop;

    private Queue<BezierPath> pathQueue = new Queue<BezierPath>();
    private BezierWaypoint currentWaypoint;
    private BezierWaypoint nextWaypoint;

    bool _isMoving;

    IEnumerator following;

    float initialSpeed;

    public float Speed { get => speed; set => speed = value;}
    public bool IsMoving => _isMoving;

    public event Action OnFinishedLine;

    private void Awake()
    {
        initialSpeed = speed;

        foreach(BezierPath path in paths)
        {
            pathQueue.Enqueue(path);
        }
    }

    private void Start()
    {
        StartFollowing();
    }

    public void StartFollowing()
    {
        if(following != null)
        {
            StopCoroutine(following);
        }

        following = Following();

        StartCoroutine(following);
    }

    IEnumerator Following()
    {
        while(pathQueue.Count > 0) 
        {
            BezierPath currentPath = pathQueue.Dequeue();
            float moveValue = 0;

            while(moveValue < 0.99f)
            {
                if(!IsMoving)
                {
                    yield return null;
                    continue;
                }

                moveValue = Mathf.MoveTowards(moveValue, 0.99f, speed * 0.01f * Time.deltaTime);
                transform.position = currentPath.GetPointAt(moveValue);

                if (currentWaypoint != currentPath.GetCurrentWaypoint(moveValue))
                {
                    currentWaypoint = currentPath.GetCurrentWaypoint(moveValue);

                    if (currentWaypoint.TryGetComponent(out BezierWaypointEvents events))
                        events.OverrideSpeed(this);
                    else
                        speed = initialSpeed;
                }

                if (nextWaypoint != currentPath.GetNextWaypoint(moveValue))
                {
                    nextWaypoint = currentPath.GetNextWaypoint(moveValue);
                }

                yield return null;
            }

            if(loop)
            {
                pathQueue.Enqueue(currentPath);
            }

            OnFinishedLine?.Invoke();
        }
    }*/

}

/*public class BezierPathFollower : MonoBehaviour
{
    [SerializeField] private BezierPath _bezierPath;
    [SerializeField] float _speed = 1;
    [SerializeField] bool _followOnEnable;

    [SerializeField] bool _loop;

    [SerializeField] bool _applyRotation = true;

    BezierWaypoint _currWaypoint;
    BezierWaypoint _nextWaypoint;


    bool _isMoving;
    IEnumerator _following;
    float _initialSpeed;


    public float Speed { get => _speed; set => _speed = value; }
    public bool IsMoving => _isMoving;

    public event Action OnFinishLine;



    void Awake()
    {
        _initialSpeed = _speed;
    }

    private void Start()
    {
        //if (_followOnEnable)
        //    StartFollowing();
    }

    void OnEnable()
    {
        //if (_followOnEnable)
        //    StartFollowing();
    }


    public void SetPathAndFollowIt(BezierPath bezierCurve, bool applyRotation = true)
    {
        _applyRotation = applyRotation;
        _bezierPath = bezierCurve;

        StartFollowing(applyRotation);
    }

    public void StartFollowing(bool applyRotation = true)
    {
        _applyRotation = applyRotation;

        _isMoving = true;

        if (_following != null)
            StopCoroutine(_following);

        StartCoroutine(_following = Following());
    }

    public void PauseFollowing()
    {
        _isMoving = false;
    }

    public void ResumeFollowing()
    {
        _isMoving = true;
    }

    public void RestartFollowing()
    {
        StartFollowing();
    }

    public void StopFollowing()
    {
        StopCoroutine(_following);

        OnFinishLine?.Invoke();
    }

    IEnumerator Following()
    {
        float _moveValue = 0;

        while (_moveValue < 0.99f)
        {
            if (!_isMoving)
            {
                yield return null;
                continue;
            }
                

            _moveValue = Mathf.MoveTowards(_moveValue, 0.99f, _speed * 0.01f * Time.deltaTime);

            transform.position = _bezierPath.GetPointAt(_moveValue);


            if (_currWaypoint && _applyRotation)
            {
                var distanceDiffrence = transform.position - _currWaypoint.position;
                Quaternion quaternion = Quaternion.LookRotation(distanceDiffrence, Vector3.up);

                transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 5 * Time.deltaTime);
            }


            if (_currWaypoint != _bezierPath.GetCurrentWaypoint(_moveValue))
            {
                _currWaypoint = _bezierPath.GetCurrentWaypoint(_moveValue);

                if (_currWaypoint.TryGetComponent(out BezierWaypointEvents events))
                    events.OverrideSpeed(this);
                else
                    _speed = _initialSpeed;


            }

            if (_nextWaypoint != _bezierPath.GetNextWaypoint(_moveValue))
            {
                _nextWaypoint = _bezierPath.GetNextWaypoint(_moveValue);
            }


            yield return null;
        }

        if (_loop)
            RestartFollowing();

        OnFinishLine?.Invoke();
    }

}
*/