using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro namespace for advanced text rendering
using UnityEngine;
using UnityEngine.UI;

public class Delivery : MonoBehaviour
{
    [Header("Package Tracking")]
    public bool hasPackage = false;
    [SerializeField] float destroyDelay = 0.5f;
    [SerializeField] float timeLeft = 0f;

    [Header("Delivery Tracking")]
    [SerializeField] int numberOfDeliveries = 0; // [SerializeField] makes private variables visible in Inspector while keeping them private in code
    [SerializeField] int deliveries = 0; // restart every time we start the game
    [SerializeField] int money = 0;

    [Header("UI")]
    [SerializeField] TMP_Text TimerText;
    [SerializeField] TMP_Text Countdown;
    [SerializeField] TMP_Text SmallTimerIncrease;
    [SerializeField] TMP_Text BigTimerIncrease;
    [SerializeField] TMP_Text DeliveriesText;
    [SerializeField] TMP_Text DeliveryCount;
    [SerializeField] Image addressOfHouse;
   
    //Lists for Package-Delivery pairs
    [SerializeField] List<GameObject> packages;
    [SerializeField] List<GameObject> houseDelivery;
    [SerializeField] int currentTargetIndex = -1;

    //Fisher–Yates shuffle, randomizes package-delivery pairs to create variation in every iteration of the game
    void ShuffleHouses() 
    {
        for (int i = houseDelivery.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            GameObject temp = houseDelivery[i];
            houseDelivery[i] = houseDelivery[randomIndex];
            houseDelivery[randomIndex] = temp;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (packages.Count != houseDelivery.Count)
            Debug.Log("Mistmatched number of package to houses!");
        TimerText.gameObject.SetActive(true);
        Countdown.gameObject.SetActive(false);
        SmallTimerIncrease.gameObject.SetActive(false);  // Hide at start
        BigTimerIncrease.gameObject.SetActive(false);  // Hide at start
        DeliveriesText.gameObject.SetActive(true);
        DeliveryCount.gameObject.SetActive(true);
        addressOfHouse.gameObject.SetActive(false);
        ShuffleHouses();
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


            currentTargetIndex = packages.IndexOf(collision.gameObject);

            if (currentTargetIndex == -1)
                Debug.Log("Picked pacakge is not in the list!");
            if (addressOfHouse == null) 
                Debug.LogError("addressOfHouse not assigned!");

            var house = houseDelivery[currentTargetIndex];
            var data = houseDelivery[currentTargetIndex].GetComponent<HouseData>();

            if (data == null) 
                Debug.LogError("HouseData missing on " + house.name);
            if (data.addressOfSprite == null)
                Debug.LogError("addressOfSprite not set on " + house.name);
        

            addressOfHouse.sprite = data.addressOfSprite;
            Destroy(collision.gameObject);
            addressOfHouse.gameObject.SetActive(true);
            Countdown.gameObject.SetActive(true);
            timeLeft = 60f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Deliver to house
        if (collision.collider.CompareTag("Delivery") && hasPackage && timeLeft > 0)
        {
            if (collision.collider.gameObject == houseDelivery[currentTargetIndex])
            {
     
                Debug.Log("Delivered package!");
                hasPackage = false;
                currentTargetIndex = -1;
                Countdown.gameObject.SetActive(false);
                addressOfHouse.gameObject.SetActive(false);
                timeLeft = 0f;
                deliveries++;
                DeliveryCount.text = deliveries.ToString();

                // Stop particle effect
                // packageParticles.Stop();
            } else
            {
                Debug.Log("Wrong house!");
            }
        }
    }
    
    public void AddTime(float seconds)
    {
        if (hasPackage && timeLeft > 0)
        {
            StartCoroutine(NotifyIncrease(seconds));
            timeLeft += seconds;
        }
    }

    IEnumerator NotifyIncrease(float seconds)
    {
        if (seconds == 5f)
        {
            SmallTimerIncrease.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            SmallTimerIncrease.gameObject.SetActive(false);

        }
        else if (seconds == 10f)
        {
            BigTimerIncrease.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            BigTimerIncrease.gameObject.SetActive(false);
        }

    }


    void UpdateTimer()
    {
        if (hasPackage)
        {
            int timeLeftInt;

            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                timeLeftInt = Mathf.CeilToInt(timeLeft); //converts float into int to keep whole numbers for countdown
                Countdown.text = timeLeftInt.ToString();
            }
            else
            {
                hasPackage = false;
                currentTargetIndex = -1;
                timeLeft = 0f; //clear state of timer
                Countdown.gameObject.SetActive(false);
                // cat escapes, not longer have package
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }
}
