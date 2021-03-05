using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    

    private Rigidbody rb;
    private float currentTime;
    private bool smash, invincible; 
    public int speed;
    public bool isGameOn = true;

    private int CurrentBrokenStacks, totalStacks;

    public GameObject InvincibleOBJ;
    public GameObject FireEffect, winEffect, splashEffect;
    public Image InvincibleFill;

    public enum PlayerState
    {
        Prepare,
        Playing,
        Died,
        Finish
    }

    [HideInInspector]
    public PlayerState playerState = PlayerState.Prepare;

    public AudioClip bounceClip, deadClip, winClip, smashClip, invincibleClip;

    void Awake()
    {
        CurrentBrokenStacks = 0;
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        totalStacks = FindObjectsOfType<StackController>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerState == PlayerState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
                smash = true;

            if (Input.GetMouseButtonUp(0))
                smash = false;

            if (invincible)
            {
                currentTime -= Time.deltaTime * 0.35f;
                if (!FireEffect.activeInHierarchy)
                    FireEffect.SetActive(true);
            }
            else
            {
                if (FireEffect.activeInHierarchy)
                {
                    FireEffect.SetActive(false);
                }

                if (smash)
                    currentTime += Time.deltaTime * 0.8f;
                else
                    currentTime -= Time.deltaTime * 0.5f;
            }

            if (currentTime >= 0.3f || InvincibleFill.color == Color.red)
            {
                InvincibleOBJ.SetActive(true);
            }
            else
            {
                InvincibleOBJ.SetActive(false);
            }

            //UI Check
            if (currentTime >= 1)
            {
                currentTime = 1;
                invincible = true;
                InvincibleFill.color = Color.red;
            }
            else if (currentTime <= 0)
            {
                currentTime = 0;
                invincible = false;
                InvincibleFill.color = Color.white;
            }

            if (InvincibleOBJ.activeInHierarchy)
                InvincibleFill.fillAmount = currentTime / 1;
        }

        if (playerState == PlayerState.Finish)
        {
            if (Input.GetMouseButtonDown(0))
                FindObjectOfType<LevelSpawnner>().nextLevel();
        }
    }
    
    void FixedUpdate()
    {
        if (playerState == PlayerState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                smash = true;

                rb.velocity = new Vector3(0, -speed * Time.fixedDeltaTime * 7, 0);

            }

            if (rb.velocity.y > 5)
            {
                rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);
            }
        }

        
    }

    public void IncreaseBrokenStacks()
    {
        CurrentBrokenStacks++;
        if (!invincible)
        {
            ScoreManager.instance.AddScore(1);
            SoundManager.instance.PlaySoundFx(smashClip, 0.5f);
        }
        else
        {
            ScoreManager.instance.AddScore(2);
            SoundManager.instance.PlaySoundFx(invincibleClip, 0.5f);
        }

    }

    void OnTriggerEnter(Collider target)
    {
        if (isGameOn == true)
        {
            if (!smash)
            {
                if (target.gameObject.tag != "Finish")
                {
                    GameObject splash = Instantiate(splashEffect);

                    Destroy(splash, 1.5f);
                    splash.transform.SetParent(target.transform);
                    splash.transform.localEulerAngles = new Vector3(90, Random.Range(0, 369), 0);
                    float randomScale = Random.Range(0.18f, 0.25f);
                    splash.transform.localScale = new Vector3(randomScale, randomScale, 1);
                    splash.transform.position = new Vector3(transform.position.x, transform.position.y - 0.22f, transform.position.z);
                    splash.GetComponent<SpriteRenderer>().color = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
                }

                SoundManager.instance.PlaySoundFx(bounceClip, 0.5f);
                rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
            }
            else
            {

                if (invincible)
                {
                    if (target.gameObject.tag == "enemy" || target.gameObject.tag == "plane")
                        target.transform.parent.GetComponent<StackController>().ShatterAllParts();
                }
                else
                {
                    if (target.gameObject.tag == "enemy")
                    {
                        target.transform.parent.GetComponent<StackController>().ShatterAllParts();
                    }
                    if (target.gameObject.tag == "plane")
                    {
                        isGameOn = false;
                        playerState = PlayerState.Died;
                        rb.isKinematic = true;
                        transform.GetChild(0).gameObject.SetActive(false);
                        Debug.Log("Game Over");
                        ScoreManager.instance.ResetScore();
                        SoundManager.instance.PlaySoundFx(deadClip, 0.5f);
                    }
                }
            }

            FindObjectOfType<GameUI>().levelSliderFill(CurrentBrokenStacks / (float)totalStacks);

            if (target.gameObject.tag == "Finish" && playerState == PlayerState.Playing)
            {
                playerState = PlayerState.Finish;
                SoundManager.instance.PlaySoundFx(winClip, 0.7f);
                InvincibleOBJ.SetActive(false);


                GameObject win = Instantiate(winEffect);
                win.transform.SetParent(Camera.main.transform);
                win.transform.localPosition = Vector3.up * 1.5f;
                win.transform.eulerAngles = Vector3.zero;
            }
        }


    }

    void OnTriggerStay(Collider target)
    {
        if (!smash || target.gameObject.tag == "Finish")
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);

        }
    }



}

