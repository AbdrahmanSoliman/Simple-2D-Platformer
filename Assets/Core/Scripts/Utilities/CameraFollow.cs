using UnityEngine;

namespace Platformer.Utilities
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 1f, -10f);

        [Header("Smoothing")]
        [SerializeField] private float smoothTime = 0.15f;

        [Header("Axis Locks")]
        [SerializeField] private bool lockX;
        [SerializeField] private bool lockY;

        [Header("Camera Bounds")]
        [SerializeField] private bool useBounds = true;
        [SerializeField] private float leftBound;
        [SerializeField] private float rightBound;
        [SerializeField] private float bottomBound;
        [SerializeField] private float topBound;

        private Vector3 velocity;

        private void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 targetPosition = GetTargetPosition();

            targetPosition = ApplyAxisLocks(targetPosition);

            if (useBounds)
                targetPosition = ClampToBounds(targetPosition);

            MoveCamera(targetPosition);
        }

        private Vector3 GetTargetPosition()
        {
            return target.position + offset;
        }

        private Vector3 ApplyAxisLocks(Vector3 targetPosition)
        {
            if (lockX)
                targetPosition.x = transform.position.x;

            if (lockY)
                targetPosition.y = transform.position.y;

            return targetPosition;
        }

        private Vector3 ClampToBounds(Vector3 targetPosition)
        {
            float verticalExtent = Camera.main.orthographicSize;
            float horizontalExtent = verticalExtent * Camera.main.aspect;

            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                leftBound + horizontalExtent,
                rightBound - horizontalExtent
            );

            targetPosition.y = Mathf.Clamp(
                targetPosition.y,
                bottomBound + verticalExtent,
                topBound - verticalExtent
            );

            return targetPosition;
        }

        private void MoveCamera(Vector3 targetPosition)
        {
            targetPosition.z = transform.position.z;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );
        }

        private void OnDrawGizmosSelected()
        {
            if (!useBounds)
                return;

            Gizmos.color = Color.yellow;

            Vector3 center = new(
                (leftBound + rightBound) * 0.5f,
                (bottomBound + topBound) * 0.5f,
                0f
            );

            Vector3 size = new(
                rightBound - leftBound,
                topBound - bottomBound,
                0f
            );

            Gizmos.DrawWireCube(center, size);
        }
    }
}