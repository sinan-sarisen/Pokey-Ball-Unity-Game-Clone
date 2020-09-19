using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Ball : MonoBehaviour {

	public Rigidbody2D rb;
	public Rigidbody2D hook;

	public Text levelT;
	public Text coinT;


	private int levelint=0;
	private int hardness=1;
	private int levelLong=5;
	private int maxLevel=15;
	private int coinCount=0;
	private int i;



	public List<GameObject> walls = new List<GameObject>();
	GameObject wallHolder;
	GameObject coinHolder;



	public float releaseTime = .15f;
	public float maxDragDistance = 0.5f;
	public float distance = 8f;
	public float wallsXpos = 3.35f;


	public GameObject Hook,ball;

	private bool isPressed = false;
	private bool isStopped = false;

	


	public Transform Wallprefab,EmptyWallprefab,RedWallprefab,Finishprefab,Coinprefab;

	public LayerMask wallLayer;
	RaycastHit2D hit;

    void Start()
	{
		wallHolder = (Instantiate(Wallprefab, new Vector3(wallsXpos, 0, 0), Quaternion.identity) as Transform).gameObject;  //first wall must be normal.
		walls.Add(wallHolder);


		levelLong = PlayerPrefs.GetInt("level");

		for (i=1; i < levelLong; i++)
		{
		
			if (Random.value > 0.7) // % 30 posibility normal wall  
			{
              
					wallHolder = (Instantiate(Wallprefab, new Vector3(wallsXpos, i * 4.6f, 0), Quaternion.identity) as Transform).gameObject;
					walls.Add(wallHolder);

			}
			
            else 
            {
					if (walls[walls.Count - 1].tag != "RedWall")
					{

						wallHolder = (Instantiate(RedWallprefab, new Vector3(wallsXpos, i * 4.6f, 0), Quaternion.identity) as Transform).gameObject;
						walls.Add(wallHolder);

						for (int j = 0; j < Random.Range(1, 6); j++)
						{
							coinHolder = (Instantiate(Coinprefab, new Vector3(0, i * (4.6f )+(j * 1.5f), 0), Quaternion.identity) as Transform).gameObject;  //Coin maker
						}
				}
					else if (walls[walls.Count - 1].tag != "EmptyWall")
					{
						wallHolder = (Instantiate(EmptyWallprefab, new Vector3(wallsXpos, i * 4.6f, 0), Quaternion.identity) as Transform).gameObject;
						walls.Add(wallHolder);

						i++;
						wallHolder = (Instantiate(Wallprefab, new Vector3(wallsXpos, i * 4.6f, 0), Quaternion.identity) as Transform).gameObject; //Must be normal not red wall after every empty walls.
						walls.Add(wallHolder);
					}

					

			}
		}
		wallHolder = (Instantiate(Finishprefab, new Vector3(0.95f, i * 4.45f, -1), Quaternion.identity) as Transform).gameObject;

	}

	void Update ()
	{
		levelint = PlayerPrefs.GetInt("level");
		coinCount = PlayerPrefs.GetInt("Coin");


		levelT.text = "Level: " + (levelint).ToString();
		coinT.text = "Coins: " + (coinCount).ToString();

		if (Input.GetKeyDown("space")
)
		{
			OnMouseDown();

		}
		if (Input.GetKeyUp("space")
)
		{
			OnMouseUp();

		}

		

		if (isPressed)
		{
			rb.velocity = Vector3.zero;
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			if (Vector3.Distance(mousePos, hook.position) > maxDragDistance)
            {
                float normalized = Mathf.Sign(mousePos.y - hook.position.y);
                rb.position = new Vector2(0, hook.position.y + normalized * maxDragDistance);
            }
            else
				rb.position = new Vector2(0,mousePos.y);


		}


	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Finish")
		{
			hardness = PlayerPrefs.GetInt("hardness");

			hardness++;
            if (levelLong<maxLevel) //if not reach max level number
            {
				levelLong++;
			}
            

			PlayerPrefs.SetInt("level", levelLong);
			PlayerPrefs.SetInt("hardness", hardness);

			
		
			Destroy(gameObject);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

			

		}
		if (other.name == "Bottom")
		{
			
			Destroy(gameObject);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);



		}

		if (other.tag == "Coin")
		{
			hardness = PlayerPrefs.GetInt("Coin");


			Destroy(other.gameObject);
			coinCount++;
			print(coinCount);
			PlayerPrefs.SetInt("Coin", coinCount);


		}
	}
	bool IsWall()
	{


		

		
		if (hit.collider != null && hit.collider.tag=="Wall")
		{
			return true;
        }
        else if (hit.collider != null && hit.collider.tag == "RedWall")
        {
			Destroy(gameObject);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
        

	


		return false;
	}

	void OnMouseDown ()
	{
		if (Vector3.Distance(ball.transform.position, hook.position) > maxDragDistance)
        {
			Hook.transform.position = ball.transform.position;
        }


		hit = Physics2D.Raycast(transform.position, Vector2.right, 6f, wallLayer);
		UnityEngine.Debug.DrawRay(transform.position, Vector2.right, Color.red, 6f);


		if (IsWall())
        {
			
			GetComponent<SpringJoint2D>().enabled = true;
			isPressed = true;
			rb.isKinematic = true;

        } 

        if (!isStopped)
        {
			isStopped = true;
		}



	}

	void OnMouseUp ()
	{
		
		isPressed = false;
		rb.isKinematic = false;

		

		if ( Vector3.Distance(ball.transform.position, hook.position) > 0.5f)
		{

			isStopped = false;

			StartCoroutine(Release());

			rb.AddTorque(-5);
			rb.AddTorque(rb.angularVelocity * -.05f, (ForceMode2D)ForceMode.Force);

		}



	}

	IEnumerator Release ()
	{

		yield return new WaitForSeconds(releaseTime);

		GetComponent<SpringJoint2D>().enabled = false;

	
		yield return new WaitForSeconds(2f);

		
	
	}

}
