using UnityEngine;
using UnityEngine.InputSystem; // Handles player input from keyboard, mouse, gamepad, etc.
using System.Collections;

// A MonoBehavior script is a bridge between you custom C# code and the underlying Unity engine, allowing you to attach the sript to a GameObject in the scene as a component
public class Driver : MonoBehaviour
{
    // class-level variables
    [Header("Movement Settings")]
    [SerializeField] float currentSpeed = 1f; // Visible in Inspector
    [SerializeField] float regularSpeed = 1f;
    [SerializeField] float slowSpeed = 0.7f;
    [SerializeField] float steerSpeed = 25f;

    [Header("Counters")]
    [SerializeField] int numberOfDeliveries = 0; // [SerializeField] makes private variables visible in Inspector while keeping them private in code
    [SerializeField] bool hasPackage = false; //Use [SerializeField] for values you want to adjust in the Inspector while keeping proper encapsulation.

    private Rigidbody2D rb;
    private float move = 0f;
    private float steer = 0f;

    // Efficient - Get once, store, reuse
    SpriteRenderer sr;
    Color originalColor;

    // Get Rigidbody2D component
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    // Collision Test
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Collider"))
        {
            StartCoroutine(BumpEffect(collision));
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator BumpEffect(Collision2D collision)
    {
        // Slow down
        float originalSpeed = currentSpeed;
        currentSpeed = slowSpeed;

        Vector2 away = collision.GetContact(0).normal;
        float distance = 0.2f;
        float time = 0.08f;

        float moved = 0f;
        float speed = distance / time;

        while (moved < distance)
        {
            float step = speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + away * step);
            moved += step;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1f);
        currentSpeed = originalSpeed;
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red; // Change color of player temporarily
        yield return new WaitForSeconds(0.5f);
        sr.color = originalColor;
    }

    //Trigger Test
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null) // safety check
        {
            if (collision.CompareTag("Package") && !hasPackage)
            {
                hasPackage = true;
                currentSpeed = regularSpeed; // Restore speed after picking up package
            }

            if (collision.CompareTag("RareBoost"))
            {
                //increase timer + 5s
                //boostText.gameObject.SetActive(true);
                Destroy(collision.gameObject);
            }

            if (collision.CompareTag("CommonBoost"))
            {
                //increase timer + 10s
                //boostText.gameObject.SetActive(true);
                Destroy(collision.gameObject);
            }

            if (collision.CompareTag("Trigger"))
            {
                currentSpeed = slowSpeed; //slow speed while crossing object
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Trigger"))
        {
            currentSpeed = regularSpeed; //return speed to normal
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
        move = 0f;
        steer = 0f;

        if (Keyboard.current == null) return;

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

        /* Apply movement
        float moveAmount = move * currentSpeed * Time.deltaTime; // Time.deltaTime makes movement frame rate independent
        float steerAmount = steer * steerSpeed * Time.deltaTime; // Speed is now units per second rather than units pers frame

        transform.Translate(moveAmount, 0, 0);
        transform.Rotate(0, 0, steerAmount); */

    }
    void FixedUpdate()
    {
        if (rb == null) return;

        if (move == 0f && steer == 0f) return;

        // Use transform.right because your original Translate(x,0,0) moved along local X axis
        Vector2 forward = transform.right;

        // Use MovePosition instead of transform.Translate
        Vector2 newPos = rb.position + forward * (move * currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Use MoveRotation instead of transform.Rotate
        float newRot = rb.rotation + (steer * steerSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newRot);
    }

}
