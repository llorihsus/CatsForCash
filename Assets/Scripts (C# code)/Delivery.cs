using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] bool hasPackage = false;
    [SerializeField] float destroyDelay = 0.5f;

    ParticleSystem packageParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        packageParticles = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

        // Deliver to house
        if (collision.CompareTag("Delivery") && hasPackage)
        {
            Debug.Log("Delivered package!");
            hasPackage = false;

            // Stop particle effect
            // packageParticles.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
