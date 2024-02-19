using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame.AsteroidManagers;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ShipController : MonoBehaviour
{
    MovesManager movesManager;

    [SerializeField] private MoveAsset leftHand;
    [SerializeField] private MoveAsset rightHand;

    MoveAssetConditionRotation leftHandCondRot, rightHandCondRot;

    [SerializeField] private Animator anim;

    public float speed = 5f;
    public float maxDistance = 5f;

    [SerializeField] Vector3 nearDetectOffset;
    [SerializeField] Vector3 nearDetectSize;

    [SerializeField] float aheadDetectRadius;
    [SerializeField] float aheadDetectDistance;

    [System.Serializable]
    public class SoundSet
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip[] clips;

        public void PlayRandom()
        {
            audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.Play();
        }
    }
    [SerializeField] SoundSet[] characterSoundSets;
    [SerializeField] SoundSet spaceshipSoundSet;
    Collider detectedAsteroid;

    [SerializeField] Vector3 hitBoxOffset;
    [SerializeField] Vector3 hitBoxSize;
    [SerializeField] LayerMask astroidLayerMask;

    [SerializeField] GameObject forceField;
    [SerializeField] Material forceFieldMat;
    [SerializeField] float forceFieldFlashTime;
    [SerializeField] float forceFieldPowerOn = 6.95f;
    [SerializeField] float forceFieldPowerOff = 30f;

    [SerializeField] float knockbackDist;
    [SerializeField] float knockbackTime;
    bool canMove = true;

    [SerializeField] float returnTime;

    private void Awake()
    {
        leftHandCondRot = (MoveAssetConditionRotation)leftHand.GetCondition(typeof(MoveAssetConditionRotation));
        rightHandCondRot = (MoveAssetConditionRotation)rightHand.GetCondition(typeof(MoveAssetConditionRotation));
    }


    private void Start()
    {
        movesManager = MovesManager.Instance;

        movesManager.Add(leftHand);
        movesManager.Add(rightHand);
    }

    [SerializeField] private float horizontalInputRaw = 0f;
    [SerializeField] private float horizontalInput = 0f;
    float horizontalInputVelocity;
    [SerializeField] float horizontalInputSmoothTime;

    [SerializeField] private bool conflict = false;

    [SerializeField] private bool leftHandenabled = false;
    [SerializeField] private bool rightHandenabled = false;

    private void OnEnable()
    {
        leftHand.OnEnterUpdate += LeftHand;
        leftHand.OnExit += LeftHandExit;

        rightHand.OnEnterUpdate += RightHand;
        rightHand.OnExit += RightHandExit;
    }

    private void OnDisable()
    {
        leftHand.OnEnterUpdate -= LeftHand;
        leftHand.OnExit -= LeftHandExit;

        rightHand.OnEnterUpdate -= RightHand;
        rightHand.OnExit -= RightHandExit;
    }

    void Update()
    {
        if (!canMove)
            return;

        HandleMovement();
        HandleCollisionDetection();


        void HandleMovement()
        {
            horizontalInput = Mathf.SmoothDamp(horizontalInput, horizontalInputRaw, ref horizontalInputVelocity, horizontalInputSmoothTime);

            Vector3 newPosition = transform.position + new Vector3(horizontalInput, 0f, 0f) * speed * Time.deltaTime;

            if(!leftHandenabled && !rightHandenabled)
            {
                horizontalInputRaw = 0;
            }
            newPosition.x = Mathf.Clamp(newPosition.x, -maxDistance, maxDistance);

            transform.position = newPosition;

            anim.SetFloat("Horizontal", horizontalInputRaw);
        }

        async void HandleCollisionDetection()
        {
            if (GetBoxCollision(transform.position + nearDetectOffset, nearDetectSize, out Collider astroidNear))
            {
                if (astroidNear != detectedAsteroid)
                {
                    SoundSet charSoundSet = characterSoundSets[Random.Range(0, characterSoundSets.Length)];
                    charSoundSet.PlayRandom();
                    detectedAsteroid = astroidNear;
                }
            }

            if (GetSphereCollision(transform.position + (Vector3.forward * aheadDetectDistance), aheadDetectRadius, out Collider astroidAhead))
            {
                if (astroidAhead != detectedAsteroid)
                {
                    spaceshipSoundSet.PlayRandom();
                    detectedAsteroid = astroidAhead;
                }
            }

            if (GetBoxCollision(transform.position + hitBoxOffset, hitBoxSize, out Collider astroidHit))
            {
                canMove = false;

                anim.Play("Hit");
                horizontalInputRaw = 0;
                ForceFieldFlashAnim();

                astroidHit.GetComponent<AsteroidController>().Hit(); // Explode asteroid

                // Knockback
                Vector3 hitPos = transform.position;
                Vector3 knockBackDir = (hitPos - astroidHit.transform.position).normalized;
                Vector3 knockbackPos = hitPos + (knockBackDir * knockbackDist);
                knockbackPos.y = hitPos.y;
                knockbackPos.z = Mathf.Clamp(knockbackPos.z, -knockbackDist, hitPos.z);
                await AnimateMove(hitPos, knockbackPos, knockbackTime);

                // Recovery
                Vector3 recoveryPos = new (knockbackPos.x, hitPos.y, hitPos.z);
                await AnimateMove(knockbackPos, recoveryPos, returnTime);

                canMove = true;
            }
        }

        bool GetBoxCollision(Vector3 pos, Vector3 size, out Collider astroidCollider)
        {
            Collider[] c = new Collider[1];
            if (Physics.OverlapBoxNonAlloc(pos, size * 0.5f, c, Quaternion.identity, astroidLayerMask) == 1)
            {
                astroidCollider = c[0];
                return true;
            }
            else
            {
                astroidCollider = null;
                return false;
            }
        }

        bool GetSphereCollision(Vector3 pos, float radius, out Collider astroidCollider)
        {
            Collider[] c = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(pos, radius, c, astroidLayerMask) == 1)
            {
                astroidCollider = c[0];
                return true;
            }
            else
            {
                astroidCollider = null;
                return false;
            }
        }
    }

    async Task ForceFieldFlashAnim()
    {
        forceField.SetActive(true);

        float halfTime = forceFieldFlashTime / 2f;

        await ForceFieldTransition(forceFieldPowerOff, forceFieldPowerOn, halfTime);

        await ForceFieldTransition(forceFieldPowerOn, forceFieldPowerOff, halfTime);

        forceField.SetActive(false);
    }

    async Task ForceFieldTransition(float startPower, float endPower, float time)
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += 1f / time * Time.deltaTime;
            forceFieldMat.SetFloat("_Power", Mathf.Lerp(startPower, endPower, time));

            await Task.Yield();
        }
        forceFieldMat.SetFloat("_Power", endPower);
    }

    async Task AnimateMove(Vector3 startPos, Vector3 endPos, float time)
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += 1f / time * Time.deltaTime;
            if (gameObject.transform.position != null)
            {
                gameObject.transform.position = Vector3.Lerp(startPos, endPos, percent);
            }

            await Task.Yield();
        }
        gameObject.transform.position = endPos;
    }

    void LeftHand(HandReader.Hand hand)
    {
        if (!canMove)
            return;

        leftHandenabled = true;

        horizontalInputRaw = -1f;
    }

    void LeftHandExit() 
    {
        leftHandenabled = false;
    }

    void RightHand(HandReader.Hand hand) 
    {
        if (!canMove)
            return;

        rightHandenabled = true;

        horizontalInputRaw = 1f;
    }

    void RightHandExit()
    {
        rightHandenabled = false;        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.transform.tag);

        if (other.tag == "Respawn")
        {
            CameraSwitcher.Instance.CockpitView();
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Ahead
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3.forward * aheadDetectDistance), aheadDetectRadius);

        // Near
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + nearDetectOffset, nearDetectSize);

        // Hit
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + hitBoxOffset, hitBoxSize);
    }
    
}