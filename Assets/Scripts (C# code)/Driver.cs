using UnityEngine;
using UnityEngine.InputSystem; // Handles player input from keyboard, mouse, gamepad, etc.

// A MonoBehavior script is a bridge between you custom C# code and the underlying Unity engine, allowing you to attach the sript to a GameObject in the scene as a component
public class Driver : MonoBehaviour
{
    // class-level variables
    [Header("Movement Settings")]
    [SerializeField] float currentSpeed = 1f; // Visible in Inspector
    [SerializeField] float regularSpeed = 1f;
    [SerializeField] float slowSpeed = 0.5f;
    [SerializeField] float steerSpeed = 25f;

    [Header("Counters")]
    [SerializeField] int numberOfDeliveries = 0; // [SerializeField] makes private variables visible in Inspector while keeping them private in code
    [SerializeField] bool hasPackage = false; //Use [SerializeField] for values you want to adjust in the Inspector while keeping proper encapsulation.

    // Collision Test
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("WorldCollision"))
        {
            currentSpeed = regularSpeed;
            //boostText.gameObject.setActive(false);
        }
    }

    //Trigger Test
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Package")
        {
            Debug.Log("Picked up package!");
            Destroy(collision.gameObject);
            currentSpeed = regularSpeed; // Restore speed after picking up package
        }

        if (collision.CompareTag("Boost"))
        {
            //increase timer 
            //boostText.gameObject.SetActive(true);
            Destroy(collision.gameObject);
        } 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game has Started!");
    }

    // Update is called once per frame (60+ times per second)
    void Update()
    {
        float move = 0f;
        float steer = 0f;

        // Forward/Backward movement
        if (Keyboard.current.wKey.isPressed)
        {
            move = -0.15f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            move = 0.15f;
        }

        // Left/Right rotation
        if (Keyboard.current.aKey.isPressed)
        {
            steer = 0.75f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            steer = -0.75f;
        }

        // Apply movement
        float moveAmount = move * currentSpeed * Time.deltaTime;    // Time.deltaTime makes movement frame rate independent
        float steerAmount = steer * steerSpeed * Time.deltaTime; // Speed is now units per second rather than units pers frame

        transform.Translate(moveAmount, 0, 0);
        transform.Rotate(0, 0, steerAmount);

    }
}
