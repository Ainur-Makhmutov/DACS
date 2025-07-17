using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private GameObject target;                         // Персонаж, за которым будет следить камера
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);   // Камера на расстоянии
    [SerializeField] private float smoothSpeed = 5f;                    // Скорость камеры

    void LateUpdate()
    {
        if (target == null) return;

        offset.y = 4;
        Vector3 desiredPosition = target.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
