using UnityEngine;
using UnityEngine.InputSystem; // Handles player input from keyboard, mouse, gamepad, etc.

// A MonoBehavior script is a bridge between you custom C# code and the underlying Unity engine, allowing you to attach the sript to a GameObject in the scene as a component
public class Driver : MonoBehaviour
{
    // class-level variables
    [Header("Movement Settings")]
    public float currentSpeed = 7f; // Visible in Inspector
    private float steerSpeed = 300f; // Hidden in Inspector, private is default when no access modifier is specified
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float regularSpeed = 5f;
    [SerializeField] float slowSpeed = 2f;

    [Header("Counters")]
    [SerializeField] int numberOfDeliveries = 0; // [SerializeField] makes private variables visible in Inspector while keeping them private in code
    [SerializeField] bool hasPackage = false; //Use [SerializeField] for values you want to adjust in the Inspector while keeping proper encapsulation.

    // Collision Test
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Wall")
        {
            Debug.Log("Hit a wall!");
            currentSpeed = slowSpeed; // Slow down when hitting a wall
        }
        else if (collision.gameObject.name == "Rock")
        {
            Debug.Log("Hit a rock!");
            currentSpeed = slowSpeed; // Slow down when hitting a rock
        }

       /* if (collision.collider.CompareTag("WorldCollision"))
        {
            currentSpeed = regularSpeed;
            boostText.gameObject.setActive(false);
        Challenge:
         Arraylist of hte packages and showing the path to the requester 

        } */
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

        /* if (collision.CompareTag("Boost"))
        {
            currentSpeed = boostSpeed;
            boostText.gameObject.SetActive(true);
            Destroy(collision.gameObject);
        } */
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game has Started!");
    }

    // Update is called once per frame (60+ times per second)
    void Update()
    {
        /* transform.Translate(moveSpeed, 0.01f, 0);    Translate(): Moves an object by adding to its current position in (x,y,z)
                                                        Unity's transform.Translate() expects floats(32-bit), not doubles(64-bit). In C# decimal numbers are treated as double by default. */

        float move = 0f;
        float steer = 0f;

        // Forward/Backward movement
        if (Keyboard.current.wKey.isPressed)
        {
            move = 1f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            move = -1f;
        }

        // Left/Right rotation
        if (Keyboard.current.aKey.isPressed)
        {
            steer = 1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            steer = -1f;
        }

        // Apply movement
        float moveAmount = move * currentSpeed * Time.deltaTime;    // Time.deltaTime makes movement frame rate independent
        float steerAmount = steer * steerSpeed * Time.deltaTime; // Speed is now units per second rather than units pers frame

        transform.Translate(0, moveAmount, 0);
        transform.Rotate(0, 0, steerAmount);

    }
}
