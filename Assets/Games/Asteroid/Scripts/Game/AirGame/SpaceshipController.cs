using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpaceshipController : MonoBehaviour
{
    public float movementRadius = 10.0f; // �ɴ��Ϸ�10�׵�����뾶
    public float maxAngle = 3.0f; // ���Ƕ�
    public float positionUpdateTime = 4.0f; // ÿ4�����λ��
    public float turbulenceIntensity = 0.5f; // ��ǿ��

    private Rigidbody rb;
    private Vector3 originPoint;
    private Vector3 targetPosition;
    private float journeyLength;
    private float startTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ���÷ɴ��ĳ�ʼλ�ú�����
        originPoint = transform.position + Vector3.up * movementRadius;
        SetNewRandomTarget();

        // ��ʼЭ��
        StartCoroutine(TargetPositionUpdater());
    }

    void Update()
    {
        // �����Ѿ���ȥ��ʱ����ռ��ʱ��ı���
        float distCovered = (Time.time - startTime) / positionUpdateTime;

        // ����ʱ������ƶ��ɴ�
        if (distCovered <= 1)
        {
            rb.MovePosition(Vector3.Lerp(originPoint, targetPosition, distCovered));
        }
    }

    IEnumerator TargetPositionUpdater()
    {
        while (true)
        {
            SetNewRandomTarget();
            // �ȴ���һ��λ�ø���
            yield return new WaitForSeconds(positionUpdateTime);
        }
    }

    void SetNewRandomTarget()
    {
        // ��������ȷ��һ���µ����Ŀ��λ��
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        randomDirection.y = Mathf.Abs(randomDirection.y); // ȷ�����ڷɴ����Ϸ�

        // �������Ƕ�
        float angle = Mathf.Acos(Vector3.Dot(randomDirection, Vector3.up)) * Mathf.Rad2Deg;
        if (angle > maxAngle)
        {
            randomDirection = Vector3.RotateTowards(randomDirection, Vector3.up, (angle - maxAngle) * Mathf.Deg2Rad, 0.0f).normalized;
        }

        targetPosition = originPoint + randomDirection * movementRadius;
        startTime = Time.time; // ���¿�ʼʱ��
        journeyLength = Vector3.Distance(originPoint, targetPosition); // �����µ�·������
    }
}
