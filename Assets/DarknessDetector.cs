using Unity.Netcode;
using UnityEngine;

public class DarknessDetector : MonoBehaviour
{
    // Ссылка на систему рассудка
    public SanitySystem sanitySystem;

    // Параметры для проверки темноты
    public float raycastDistance = 5f;  // Дистанция для луча
    public float checkRadius = 10f;     // Радиус для проверки освещенности
    public LayerMask lightLayer;        // Слой, на котором находятся источники света
    public LayerMask obstacleLayer;     // Слой для объектов, которые могут заблокировать свет (например, стены)

    private bool isInDarkness = false;

    private void Update()
    {
        bool isCurrentlyInDarkness = true;

        // Проверка с помощью нескольких Raycast
        for (int i = 0; i < 360; i += 45) // Проверяем в разных направлениях (каждые 45 градусов)
        {
            Vector3 direction = Quaternion.Euler(0, i, 0) * transform.forward;  // Направление луча
            Ray ray = new Ray(transform.position, direction);

            // Проверяем, есть ли источник света в этом направлении
            if (Physics.Raycast(ray, raycastDistance, lightLayer))
            {
                isCurrentlyInDarkness = false; // Если нашли свет, игрок не в темноте
                break;
            }
        }

        // Теперь проверяем на наличие света в радиусе вокруг игрока
        Collider[] nearbyLights = Physics.OverlapSphere(transform.position, checkRadius, lightLayer);
        if (nearbyLights.Length > 0)
        {
            isCurrentlyInDarkness = false; // Если в радиусе есть свет, игрок не в темноте
        }

        // Если в темноте — уменьшаем рассудок
        if (isCurrentlyInDarkness != isInDarkness)
        {
            isInDarkness = isCurrentlyInDarkness;
            if (sanitySystem != null)
            {
                sanitySystem.SetInDarkness(isInDarkness); // Передаем информацию о темноте в систему рассудка
            }
        }
    }
}
