using UnityEngine;

public class SceneViewCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f; // Увеличено для работы с Input.GetAxis
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float panSpeed = 20f;

    private void Update()
    {
        if (Input.GetMouseButton(1)) // Правая кнопка мыши
        {
            HandleRotation();
        }
        
        if (Input.GetMouseButton(2)) // Средняя кнопка мыши
        {
            HandlePanning();
        }

        HandleZoom();
        HandleKeyboardMovement();
    }

    private void HandleRotation()
    {
        // Используем GetAxis вместо изменения позиции мыши
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Поворот камеры
        Vector3 euler = transform.eulerAngles;
        euler.y += mouseX * rotationSpeed * Time.deltaTime;
        euler.x -= mouseY * rotationSpeed * Time.deltaTime;
        
        // Ограничение вращения по оси X
        if (euler.x > 180f)
            euler.x -= 360f;
        euler.x = Mathf.Clamp(euler.x, -89f, 89f);
        
        transform.eulerAngles = euler;
    }

    private void HandlePanning()
    {
        // Используем GetAxis для плавного стрейфа
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 movement = transform.right * (-mouseX) + transform.up * (-mouseY);
        transform.position += movement * panSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 forward = transform.forward;
            transform.position += forward * scroll * zoomSpeed;
        }
    }

    private void HandleKeyboardMovement()
    {
        Vector3 movement = Vector3.zero;

        // Используем GetAxisRaw для более отзывчивого управления с клавиатуры
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 right = transform.right * horizontal;
        Vector3 forward = transform.forward * vertical;
        
        // Удаляем вертикальную составляющую для движения только в горизонтальной плоскости
        forward.y = 0;
        forward.Normalize();

        movement += right + forward;

        // Поддержка подъема/спуска с Q/E
        if (Input.GetKey(KeyCode.Q))
            movement += Vector3.down;
        if (Input.GetKey(KeyCode.E))
            movement += Vector3.up;

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}