using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [SerializeField] private Transform fishingRodEdge;
    [SerializeField] private Transform hookTransform;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private float lineWidth = 0.03f;
    [SerializeField] private float rotationOffset = -90f;
    [SerializeField] private float minDistanceToRotate = 0.1f;

    [Header("Rotation Limits")]
    [SerializeField] private float maxAngleOffset = 1f; 

    [Header("Breathing")]
    [SerializeField] private float breatheAmplitude = 0.3f; // сила дыхания (в градусах)
    [SerializeField] private float breatheSpeed = 1.5f;     // скорость дыхания

    private float baseAngle;

    private void Awake()
    {
        baseAngle = transform.eulerAngles.z;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
    }

    private void LateUpdate()
    {
        if (hookTransform == null || fishingRodEdge == null || lineRenderer == null)
            return;

        // Леска
        lineRenderer.SetPosition(0, fishingRodEdge.position);
        lineRenderer.SetPosition(1, hookTransform.position);

        float distance = Vector3.Distance(hookTransform.position, fishingRodEdge.position);

        float delta = 0f;

        if (distance > minDistanceToRotate)
        {
            Vector2 direction = (hookTransform.position - fishingRodEdge.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
            delta = Mathf.DeltaAngle(baseAngle, targetAngle);
        }

        // Ограничение поворота
        delta = Mathf.Clamp(delta, -maxAngleOffset, maxAngleOffset);

        // 🌬️ ДЫХАНИЕ
        float breathe = Mathf.Sin(Time.time * breatheSpeed) * breatheAmplitude;

        // Суммируем, но не выходим за пределы
        float finalDelta = Mathf.Clamp(delta + breathe, -maxAngleOffset, maxAngleOffset);

        transform.rotation = Quaternion.Euler(0f, 0f, baseAngle + finalDelta);
    }
}
