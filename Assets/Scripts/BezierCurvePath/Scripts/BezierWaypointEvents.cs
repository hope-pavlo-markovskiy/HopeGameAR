using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierWaypointEvents : MonoBehaviour
{
    [SerializeField] bool _overrideSpeed;

    [SerializeField] float _speed;

    public void OverrideSpeed(BezierPathFollower follower)
    {
        if (!_overrideSpeed)
            return;
        follower.Speed = _speed;
    }

    public void OverrideSpeed(OriginalBezierPathFollower follower)
    {
        if (!_overrideSpeed)
            return;
        follower.Speed = _speed;
    }
}
