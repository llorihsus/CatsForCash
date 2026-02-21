using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro namespace for advanced text rendering
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Delivery : MonoBehaviour
{
    [Header("Package Tracking")]
    public bool hasPackage = false;
    [SerializeField] float destroyDelay = 0.5f;
    [SerializeField] float timeLeft = 0f;

    [Header("Delivery Tracking")]
    [SerializeField] int deliveries = 0; // [SerializeField] makes private variables visible in Inspector while keeping them private in code
    [SerializeField] int money = 0;
    [SerializeField] int LostCats = 0;

    [Header("UI")]
    [SerializeField] TMP_Text TimerText;
    [SerializeField] TMP_Text Countdown;
    [SerializeField] TMP_Text SmallTimerIncrease;
    [SerializeField] TMP_Text BigTimerIncrease;
    [SerializeField] Image Money;
    [SerializeField] TMP_Text MoneyCount;
    [SerializeField] TMP_Text DeliveriesText;
    [SerializeField] TMP_Text DeliveryCount;
    [SerializeField] Image addressOfHouse;
    [SerializeField] TMP_Text GameStatus;

    [Header("Pathfinding")]
    [SerializeField] Tilemap roadTilemap;
    [SerializeField] Tilemap houseTilemap;
    private Vector3Int startCell;
    private Vector3Int goalCell;
    private List<Vector3Int> pathCells;
    [SerializeField] private LineRenderer lineRenderer;


    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite originalBiker;

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
        Time.timeScale = 1f;
        if (packages.Count != houseDelivery.Count)
            Debug.Log("Mistmatched number of package to houses!");
        TimerText.gameObject.SetActive(true);
        Countdown.gameObject.SetActive(false);
        SmallTimerIncrease.gameObject.SetActive(false);  // Hide at start
        BigTimerIncrease.gameObject.SetActive(false);  // Hide at start
        Money.gameObject.SetActive(true);
        MoneyCount.gameObject.SetActive(true);
        DeliveriesText.gameObject.SetActive(true);
        DeliveryCount.gameObject.SetActive(true);
        addressOfHouse.gameObject.SetActive(false);
        GameStatus.gameObject.SetActive(false);
        ShuffleHouses();
        sr = GetComponent<SpriteRenderer>();
        originalBiker = sr.sprite;
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

            //if (currentTargetIndex == -1)
            //    Debug.Log("Picked pacakge is not in the list!");
            //if (addressOfHouse == null) 
            //    Debug.LogError("addressOfHouse not assigned!");

            var houseData = houseDelivery[currentTargetIndex].GetComponent<HouseData>();
            var catData = packages[currentTargetIndex].GetComponent<CatData>();

            //if (houseData == null) 
            //    Debug.LogError("HouseData missing on " + houseData.name);
            //if (houseData.addressOfSprite == null)
            //    Debug.LogError("addressOfSprite not set on " + houseData.name);

            sr.sprite = catData.CatInBike; // Change sprite to cat in bike when package is picked up
            addressOfHouse.sprite = houseData.addressOfSprite;
            
            Destroy(collision.gameObject);
            addressOfHouse.gameObject.SetActive(true);
            Countdown.gameObject.SetActive(true);

            startCell = roadTilemap.WorldToCell(transform.position);
            Transform deliveryPoint = houseDelivery[currentTargetIndex].transform.Find("deliveryPoint");
            goalCell = roadTilemap.WorldToCell(deliveryPoint.position);

            pathCells = FindPathAStar(startCell, goalCell);
            DrawPath(pathCells);

            timeLeft = 20f;
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
                lineRenderer.positionCount = 0;
                Countdown.gameObject.SetActive(false);
                addressOfHouse.gameObject.SetActive(false);
                timeLeft = 0f;
                deliveries++;
                money += 50;
                MoneyCount.text = "$" + money.ToString();
                DeliveryCount.text = deliveries.ToString();

                // Stop particle effect
                // packageParticles.Stop();

                sr.sprite = originalBiker; // Change sprite back to original biker after delivery
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

    private bool IsWalkable(Vector3Int cell)
    {
        return roadTilemap.HasTile(cell);
    }

    private List<Vector3Int> FindPathAStar(Vector3Int start, Vector3Int goal)
    {
  
        List<Vector3Int> openSet = new List<Vector3Int>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, int> gScore = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, int> fScore = new Dictionary<Vector3Int, int>();

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            // Get node with lowest fScore
            Vector3Int current = openSet[0];
            foreach (var node in openSet)
            {
                if (fScore.ContainsKey(node) && fScore[node] < fScore[current])
                    current = node;
            }

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (!IsWalkable(neighbor) || closedSet.Contains(neighbor))
                    continue;

                int tentativeG = gScore[current] + 1;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (gScore.ContainsKey(neighbor) && tentativeG >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);
            }
        }

        return new List<Vector3Int>(); // no path found
    }

    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        return new List<Vector3Int>
    {
        cell + Vector3Int.up,
        cell + Vector3Int.down,
        cell + Vector3Int.left,
        cell + Vector3Int.right
    };
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return path;
    }

    private void DrawPath(List<Vector3Int> path)
    {
        if (path == null || path.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 worldPos = roadTilemap.GetCellCenterWorld(path[i]);
            lineRenderer.SetPosition(i, worldPos);
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
                lineRenderer.positionCount = 0;
                LostCats += 1;
                timeLeft = 0f; //clear state of timer
                Countdown.gameObject.SetActive(false);
                sr.sprite = originalBiker;
                // cat escapes, not longer have package
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        if (money >= 200)
        {
            GameStatus.gameObject.SetActive(true);
            GameStatus.color = Color.darkGreen;
            GameStatus.text = "YOU WIN!";
            Time.timeScale = 0f;
        } else if (LostCats == 6 && money < 200)
        {
            GameStatus.gameObject.SetActive(true);
            GameStatus.color = Color.darkRed;
            GameStatus.text = "GAME OVER";
            Time.timeScale = 0f;
        }
    }
}
