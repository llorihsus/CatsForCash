using TMPro;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [Header("Package Tracking")]
    [SerializeField] bool hasPackage = false;
    [SerializeField] float destroyDelay = 0.5f;

    [Header("Delivery Tracking")]
    [SerializeField] int numberOfDeliveries = 0; // [SerializeField] makes private variables visible in Inspector while keeping them private in code
    [SerializeField] int deliveries = 0; // restart every time we start the game

    [Header("UI")]
    [SerializeField] TMP_Text TimerText;
    [SerializeField] TMP_Text SmallTimerIncrease;
    [SerializeField] TMP_Text BigTimerIncrease;
    [SerializeField] TMP_Text DeliveriesCompleted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimerText.gameObject.SetActive(true);
        SmallTimerIncrease.gameObject.SetActive(false);  // Hide at start
        BigTimerIncrease.gameObject.SetActive(false);  // Hide at start
        UpdateDeliveriesUI();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Pick up package
        if (collision.CompareTag("Package") && !hasPackage)
        {
            // Will only trigger if we DON'T have a package
            // Prevents picking up multiple packages
            Debug.Log("Picked up package!");
            hasPackage = true;

            /* Play particle effect
            if (packageParticles != null)
            {
                packageParticles.Play();
            } */

            Destroy(collision.gameObject);
        }
    }

    void UpdateDeliveriesUI()
    {
        DeliveriesCompleted.text = "Deliveries Completed: " + deliveries;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Deliver to house
        if (collision.collider.CompareTag("Delivery") && hasPackage)
        {
            Debug.Log("Delivered package!");
            hasPackage = false;

            deliveries++;
            // updateDeliveriesUI();
            // Stop particle effect
            // packageParticles.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
