using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpaceshipController : MonoBehaviour
{
    public float movementRadius = 10.0f; // 飞船上方10米的球体半径
    public float maxAngle = 3.0f; // 最大角度
    public float positionUpdateTime = 4.0f; // 每4秒更新位置
    public float turbulenceIntensity = 0.5f; // 震动强度

    private Rigidbody rb;
    private Vector3 originPoint;
    private Vector3 targetPosition;
    private float journeyLength;
    private float startTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 设置飞船的初始位置和球心
        originPoint = transform.position + Vector3.up * movementRadius;
        SetNewRandomTarget();

        // 开始协程
        StartCoroutine(TargetPositionUpdater());
    }

    void Update()
    {
        // 计算已经过去的时间所占总时间的比例
        float distCovered = (Time.time - startTime) / positionUpdateTime;

        // 根据时间比例移动飞船
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
            // 等待下一个位置更新
            yield return new WaitForSeconds(positionUpdateTime);
        }
    }

    void SetNewRandomTarget()
    {
        // 在球面上确定一个新的随机目标位置
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        randomDirection.y = Mathf.Abs(randomDirection.y); // 确保它在飞船的上方

        // 限制最大角度
        float angle = Mathf.Acos(Vector3.Dot(randomDirection, Vector3.up)) * Mathf.Rad2Deg;
        if (angle > maxAngle)
        {
            randomDirection = Vector3.RotateTowards(randomDirection, Vector3.up, (angle - maxAngle) * Mathf.Deg2Rad, 0.0f).normalized;
        }

        targetPosition = originPoint + randomDirection * movementRadius;
        startTime = Time.time; // 更新开始时间
        journeyLength = Vector3.Distance(originPoint, targetPosition); // 计算新的路径长度
    }
}
