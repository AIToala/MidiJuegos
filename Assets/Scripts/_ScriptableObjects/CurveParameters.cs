using UnityEngine;

[CreateAssetMenu(fileName = "CurveParameters", menuName = "HandCurveParameters")]

public class CurveParameters : ScriptableObject
{
    public AnimationCurve positioning;
    public float positioningInfluence = .1f;
    public AnimationCurve rotation;
    public float rotationInfluence = 10f;
}