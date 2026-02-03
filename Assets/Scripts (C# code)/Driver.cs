using UnityEngine;

// A MonoBehavior script is a bridge between you custom C# code and the underlying Unity engine, allowing you to attach the sript to a GameObject in the scene as a component
public class Driver : MonoBehaviour 
{
    // class-level variables
    float moveSpeed = 0.01f;
    float turnSpeed = 0.1f;
    int numberOfDeliveries = 0;
    bool hasPackage = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game has Started!");
    }

    // Update is called once per frame (60+ times per second)
    void Update()
    {
        // Debug.Log("Update is running...");
        transform.Translate(moveSpeed, 0.01f, 0); // Translate(): Moves an object by adding to its current position in (x,y,z)
                                          // Unity's transform.Translate() expects floats(32-bit), not doubles(64-bit). In C# decimal numbers are treated as double by default.


    }
}
