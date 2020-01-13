using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    float speed = 11;
    private Rigidbody2D rbody;

    public GameObject[] Level;

    public Sprite white;
    public Sprite black;
    //public Image circle;
    public GameObject circle;
    bool canMove = true; // if you are playing and can move
    public static bool isGrounded; // when you are touching the ground and can jump again

    GameObject winScreen;

    [Tooltip("scans per level")]
    [SerializeField]
    int scans;
    float lengthOfScan;

    Text scansText;
    [Header ("             Scans remaining")] 
    public int[] scores;
    public Sprite[] stars;
    [SerializeField]
    GameObject starsSprite;

    public Canvas myCanvas;

    public static bool overUI = false; // are you over blocakble ui
    public static bool moveLeft = false;
    public static bool moveRight = false;
    public static bool jumping = false;

    public float fallMultiplier = 2.5f; // how much we will multiply the gravity when player is falling down
    public float lowJumpMultiplier = 2f; // for when we relaese the jump button and increase the garvity

    Animator anim;
    private AudioSource killSound;
    //public static bool jumpSound;

    public List<SpriteRenderer> blackLevelsPieces;
    public List<SpriteRenderer> whiteLevelPieces;

    private void Awake()
    {
        if (myCanvas == null)
            myCanvas = GameObject.Find("Canvas, Level").GetComponent<Canvas>();
    }
    void Start()
    {
        if(myCanvas == null)
            myCanvas = GameObject.Find("Canvas, Level").GetComponent<Canvas>();
        Time.timeScale = 1; // reset timescale if it was changed from load
        circle.GetComponent<Mask>().enabled = true;
        rbody = GetComponent<Rigidbody2D> ();
        winScreen = GameObject.Find("Win Screen");
        winScreen.SetActive(false);
        killSound = GameObject.Find("KillZone").GetComponent<AudioSource>();

        anim = GetComponent<Animator>();

        if (SceneManager.GetActiveScene().buildIndex > 3) // get he scans afte the tutorials
        {
            scansText = GameObject.Find("Scans").GetComponent<Text>();
        }
        if (circle.transform.GetChild(1).gameObject.activeSelf == true)
            circle.transform.GetChild(1).gameObject.SetActive(false);

        PreGame(); // reset
    }
    void Update()
    {
        if (Input.GetKey("s"))
        {
            MoveLeft();
        }
        if (Input.GetKey("d"))
        {
            MoveRight();
        }

        if (Input.touchCount > 0 && scans > 0 && !overUI && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButton(1) && !overUI && scans > 0 && isGrounded) // scanning
        {
            canMove = false;
            rbody.velocity = new Vector2(0, 0);
            circle.gameObject.SetActive(true);

            if (Level[0].activeInHierarchy == true) // black active
            {
                for (int i = 0; i < blackLevelsPieces.Count; ++i)
                {
                    blackLevelsPieces[i].color = new Color(1, 1, 1, 0.5f);
                }
            }
            else if (Level[1].activeInHierarchy == true) // whit active
            {
                for (int i = 0; i < whiteLevelPieces.Count; ++i)
                {
                    whiteLevelPieces[i].color = new Color(1, 1, 1, 0.5f);
                }
            }            
        }
        else if(Input.touchCount == 0 && canMove == false && circle.gameObject.activeSelf == true && !overUI) // unscan
        {
            PreGame();
            scans -= 1;
            UpdateUI();

            if (Level[0].activeInHierarchy == true)
            {
                for (int i = 0; i < blackLevelsPieces.Count; ++i)
                {
                    blackLevelsPieces[i].color = new Color(1, 1, 1, 1);
                }
            }
            else if (Level[1].activeInHierarchy == true)
            {
                for (int i = 0; i < whiteLevelPieces.Count; ++i)
                {
                    whiteLevelPieces[i].color = new Color(1, 1, 1, 1);
                }
            }
        }
        if (rbody.velocity.y < 0) // if we are falling apply more gravity to fall faster
        {
            if (anim.GetBool("Jumping") == true)
                anim.SetBool("Jumping", false);

            anim.SetBool("Falling", true);

            rbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime; // changes the gravity to fall multiplier, -1 because normal gravity is already being applied
        }
        else if (rbody.velocity.y > 0) // if we are not holding jump button down
        //else if(rbody.velocity.y > 0 && !jumping) // if we are not holding jump button down 
        {
            anim.SetBool("Jumping", true);

            if (!jumping)
            {
                rbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime; // when done jumping fall faster
            }
        }
    }
    void FixedUpdate ()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
        circle.transform.position = myCanvas.transform.TransformPoint(pos); // move the circle
        //circle.transform.position = Input.mousePosition; // get the circle to follow mouse

        if (moveRight)
        {
            rbody.velocity += Vector2.right * speed * Time.deltaTime;
        }
        if (moveLeft)
        {
            rbody.velocity -= Vector2.right * speed * Time.deltaTime;
        }
    }
    void PreGame()
    {
        canMove = true;
        circle.gameObject.SetActive(false);
        UpdateUI();
    }
    public void MoveRight()
    {
        moveRight = true;
        moveLeft = false;
        if (transform.rotation != Quaternion.Euler(0, 0, 0))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        anim.SetBool("Moving", true);
    }
    public void MoveLeft()
    {
        moveLeft = true;
        moveRight = false;

        if (transform.rotation != Quaternion.Euler(0, 180, 0))
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        anim.SetBool("Moving", true);
    }
    public void NotMoving()
    {
        if(moveRight == true)
            moveRight = false;
        if(moveLeft == true)
            moveLeft = false;

        if (anim.GetBool("Moving") == true)
            anim.SetBool("Moving", false);
    }
    public void TouchAction()
    {
        if(Level[0].activeInHierarchy == true)
        //if (this.GetComponent<SpriteRenderer>().sprite == white) // if white change
        {
            //this.GetComponent<SpriteRenderer>().sprite = black;
            Level[0].SetActive(false); // turn off white gameobjects
            Level[1].SetActive(true);
            circle.transform.GetChild(0).gameObject.SetActive(false);
            circle.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (Level[1].activeInHierarchy == true)
            //else if (this.GetComponent<SpriteRenderer>().sprite == black) // if black change
        {
                //this.GetComponent<SpriteRenderer>().sprite = white;
            Level[0].SetActive(true);
            Level[1].SetActive(false);
            circle.transform.GetChild(0).gameObject.SetActive(true);
            circle.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.name == "Goal")
        {
            collider.GetComponent<AudioSource>().Play();
            collider.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Time.timeScale = 0;
            winScreen.SetActive(true);
            winScreen.transform.GetChild(3).gameObject.SetActive(false); // turn off lose text
            winScreen.transform.GetChild(0).gameObject.SetActive(false); // turn off restart button

            // if certain number of moves
            //if (SceneManager.GetActiveScene().buildIndex != 2 || SceneManager.GetActiveScene().buildIndex != 1 || SceneManager.GetActiveScene().buildIndex != 3)
            if (SceneManager.GetActiveScene().buildIndex > 3)
            {
                if (scans <= scores[0])
                {
                    starsSprite.GetComponent<Image>().sprite = stars[0]; // 1
                    DontDestroy.controller.TempStars(SceneManager.GetActiveScene().buildIndex, 1);
                }
                else if (scans == scores[1])
                {
                    starsSprite.GetComponent<Image>().sprite = stars[1]; // 2
                    DontDestroy.controller.TempStars(SceneManager.GetActiveScene().buildIndex, 2);
                }
                else if (scans >= scores[2])
                {
                    starsSprite.GetComponent<Image>().sprite = stars[2]; // 3 perfect
                    DontDestroy.controller.TempStars(SceneManager.GetActiveScene().buildIndex, 3);
                }
            }
            else // 3 stars on tutorial levels
            {
                starsSprite.GetComponent<Image>().sprite = stars[2]; // 3 perfect
            }
        }
        if (collider.name == "KillZone" || collider.tag == "Spike" || collider.name == "Spike")
        {
            if(killSound == null)
            {
                killSound = GameObject.Find("KillZone").GetComponent<AudioSource>();
            }

            killSound.Play();
            Time.timeScale = 0;
            winScreen.SetActive(true);
            winScreen.transform.GetChild(4).gameObject.SetActive(false); // turn off win text
            winScreen.transform.GetChild(1).gameObject.SetActive(false); // turn off next button
            winScreen.transform.GetChild(5).gameObject.SetActive(false); // turn off stars
        }
    }
    void UpdateUI()
    {
        if (SceneManager.GetActiveScene().buildIndex > 3)
            scansText.text = "Scans: " + scans.ToString();
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1));
        else
            SceneManager.LoadScene(0);
        //Debug.Log(SceneManager.GetActiveScene().buildIndex);
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene(0);
    }
}