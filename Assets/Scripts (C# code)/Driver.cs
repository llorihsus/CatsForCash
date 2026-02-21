using UnityEngine;
using UnityEngine.InputSystem; // Handles player input from keyboard, mouse, gamepad, etc.
using System.Collections;
using UnityEngine.SceneManagement; // For using IEnumerator and Coroutines

// Current Challenges:
//   List<GameObject> -> keep track of packages and the index (customer who requrested that package)
//   Game AI -> pathfinding 
//   Timer comoponent -> increase time when picking up package, decrease time when colliding with objects, display time on UI
//   count currency earned
//   change appearance of player sprite when package is picked up 

// A MonoBehavior script is a bridge between you custom C# code and the underlying Unity engine, allowing you to attach the sript to a GameObject in the scene as a component
public class Driver : MonoBehaviour
{
    // class-level variables
    [Header("Movement Settings")]
    [SerializeField] float currentSpeed = 7f; // Visible in Inspector
    [SerializeField] float regularSpeed = 7f; //Use [SerializeField] for values you want to adjust in the Inspector while keeping proper encapsulation.
    [SerializeField] float slowSpeed = 0.7f;
    [SerializeField] float steerSpeed = 25f;
    [SerializeField] float move = 0f;
    [SerializeField] float steer = 0f;

    // Efficient - Get once, store, reuse
    [Header("Components")]
    [SerializeField] ParticleSystem playerParticles;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color originalColor;

    [Header("Reference")]
    [SerializeField] Delivery delivery;

    // Get Rigidbody2D component
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game has Started!");
        playerParticles = GetComponent<ParticleSystem>();
        playerParticles.Play();
    }

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
        currentSpeed = regularSpeed;
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red; // Change color of player temporarily
        yield return new WaitForSeconds(0.5f);
        sr.color = originalColor;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null) // safety check
        {
            if (collision.CompareTag("Package") && !delivery.hasPackage)
            {
                currentSpeed = regularSpeed; // Restore speed after picking up package
            }

            if (delivery.hasPackage) {
                if (collision.CompareTag("RareBoost"))
                {
                    delivery.AddTime(10f); // adds 10 seconds to timer
                    Destroy(collision.gameObject);
                }

                if (collision.CompareTag("CommonBoost"))
                {
                    delivery.AddTime(5f); // adds 5 seconds to timer
                    Destroy(collision.gameObject);
                }
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

        // Resets the game
        if (Keyboard.current.rKey.isPressed)
        {
            SceneManager.LoadScene("Main");
        }


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
