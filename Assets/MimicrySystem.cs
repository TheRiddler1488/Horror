using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class MimicrySystem : NetworkBehaviour
{
    public float mimicDuration = 30f;
    public float mimicCooldown = 120f;
    private bool isMimicking = false;
    private float lastMimicTime = -999f;
    private GameObject originalPlayerModel;
    private GameObject mimicModelInstance;
    private Camera playerCamera;
    private float originalCameraHeight;

    private void Start()
    {
        if (IsOwner)
        {
            originalPlayerModel = transform.GetChild(0).gameObject;
            playerCamera = Camera.main;
            originalCameraHeight = playerCamera.transform.position.y; // Сохраняем исходную высоту камеры
        }
    }

    void Update()
    {
        if (!IsOwner || isMimicking) return;

        if (Input.GetKeyDown(KeyCode.F) && Time.time - lastMimicTime >= mimicCooldown)
        {
            TryStartMimic();
        }
    }

    private void TryStartMimic()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Mimicable"))
            {
                StartCoroutine(MimicRoutine(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator MimicRoutine(GameObject target)
    {
        isMimicking = true;
        lastMimicTime = Time.time;

        // Скрываем модель игрока
        if (originalPlayerModel != null)
            originalPlayerModel.SetActive(false);

        // Копируем внешний вид мимикрируемого объекта
        mimicModelInstance = Instantiate(target, transform.position, transform.rotation, transform);
        DestroyUnneededComponents(mimicModelInstance);

        // Получаем высоту копируемого объекта (например, если объект — куб)
        float targetHeight = mimicModelInstance.GetComponent<Renderer>().bounds.extents.y;

        // Корректируем позицию объекта на уровне земли
        mimicModelInstance.transform.position = new Vector3(mimicModelInstance.transform.position.x, mimicModelInstance.transform.position.y - targetHeight, mimicModelInstance.transform.position.z);

        // Корректируем позицию камеры
        AdjustCameraPosition(mimicModelInstance);

        // Передаем владельца если нужно (опционально)
        var netObj = mimicModelInstance.GetComponent<NetworkObject>();
        if (IsServer && netObj != null && !netObj.IsSpawned)
        {
            netObj.Spawn();
            netObj.ChangeOwnership(OwnerClientId);
        }

        yield return new WaitForSeconds(mimicDuration);

        // Возврат в нормальное состояние
        if (mimicModelInstance != null) Destroy(mimicModelInstance);

        if (originalPlayerModel != null)
            originalPlayerModel.SetActive(true);

        // Восстановление позиции камеры
        ResetCameraPosition();

        isMimicking = false;
    }

    private void DestroyUnneededComponents(GameObject obj)
    {
        // Удаляем компоненты, мешающие мимикрии (например, лишние компоненты)
        foreach (var col in obj.GetComponentsInChildren<Collider>())
        {
            Destroy(col);
        }

        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
        {
            Destroy(rb);
        }

        foreach (var net in obj.GetComponentsInChildren<NetworkObject>())
        {
            if (net != obj.GetComponent<NetworkObject>())
                Destroy(net);
        }
    }

    private void AdjustCameraPosition(GameObject target)
    {
        // Получаем высоту объекта
        var targetHeight = target.GetComponent<Renderer>().bounds.size.y;
        
        // Рассчитываем новый уровень камеры
        var cameraHeight = targetHeight / 2;
        playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, cameraHeight, playerCamera.transform.position.z);
    }

    private void ResetCameraPosition()
    {
        // Восстанавливаем исходную высоту камеры
        playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, originalCameraHeight, playerCamera.transform.position.z);
    }
}
