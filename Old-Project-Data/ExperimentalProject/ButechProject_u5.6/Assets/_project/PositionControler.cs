using System;
using UnityEngine;
public class PositionControler : MonoBehaviour
{
    [Tooltip("Offset to be applied")]
    public Vector3 Offset = new Vector3();
    [Tooltip("Speed used for animation")]
    public float MovementSpeed = 0.1f;

    public bool IsMoving { get; private set; }

    public Vector3 CurrentPosition {
        get { return transform.localPosition - Offset; }
        private set { transform.localPosition = value + Offset; }
    }
    private Vector3 targetPosition;

    private void Start()
    {
        IsMoving = false;
        targetPosition = CurrentPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMoving)
        {
            float maxStep = MovementSpeed * Time.deltaTime;
            CurrentPosition = Vector3.MoveTowards(CurrentPosition, targetPosition, maxStep);
            if (CurrentPosition == targetPosition)
                IsMoving = false;
        }
    }

    public void SetTargetPosition(Vector3 pos)
    {
        if (pos == targetPosition)
            return;
        targetPosition = pos;
        IsMoving = true;
    }
    public void SetTargetPosition(Nullable<float> x = null, Nullable<float> y = null, Nullable<float> z = null)
    {
        Vector3 pos = new Vector3(x ?? targetPosition.x, y ?? targetPosition.y, z ?? targetPosition.z);
        SetTargetPosition(pos);
    }

    public void ModifyTargetPosition(Vector3 deltaPos)
    {
        SetTargetPosition(targetPosition + deltaPos);
    }
    public void ModifyTargetPosition(float dx = 0, float dy = 0, float dz = 0)
    {
        Vector3 deltaPos = new Vector3(dx, dy, dz);
        ModifyTargetPosition(deltaPos);
    }
}