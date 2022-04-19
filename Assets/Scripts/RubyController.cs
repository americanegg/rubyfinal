using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;

    public Text score;
    public Text ammo;
    private int scoreValue = 0;
    private int ammoValue = 4;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject stageTextObject;
    public GameObject BGMusic;

    public GameObject projectilePrefab;

    int currentHealth;
    public int health { get { return currentHealth; } }

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rb2d;
    float horizontal;
    float vertical;

    Animator anim;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;

    public ParticleSystem hitEffect;
    public ParticleSystem healEffect;

    bool gameOver;

    public static int level = 1;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        score.text = scoreValue.ToString();
        ammo.text = ammoValue.ToString();

        scoreValue = 0;
        ammoValue = 4;
        changeScore(0);
        changeAmmo(0);
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        stageTextObject.SetActive(false);
    }

    public void changeScore(int number)
    {
        scoreValue = scoreValue + number;
        score.text = "Robots Fixed: " + scoreValue.ToString();

        if (scoreValue == 4)
        {
            stageTextObject.SetActive(true);
            gameOver = false;
        }

        else if (scoreValue == 10)
        {
            winTextObject.SetActive(true);
            gameOver = true;
        }
    }

    public void changeAmmo(int number)
    {
        ammoValue = ammoValue + number;
        ammo.text = "Cogs: " + ammoValue.ToString();
    }


    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        anim.SetFloat("Look X", lookDirection.x);
        anim.SetFloat("Look Y", lookDirection.y);
        anim.SetFloat("Speed", move.magnitude);


        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (health <= 0)
        {
            loseTextObject.SetActive(true);
            gameOver = true;
            speed = 0;
            BGMusic.SetActive(false);
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            if (ammoValue == 0)
            {
                return;
            }
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rb2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (scoreValue == 4)
                {
                    SceneManager.LoadScene("Level2");
                }

                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rb2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rb2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            anim.SetTrigger("Hit");

            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);
            Instantiate(hitEffect, rb2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        if (amount > 0)
        {
            Instantiate(healEffect, rb2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }



        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb2d.position + Vector2.up * 0.5f, Quaternion.identity);

        changeAmmo(-1);

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        projectile.Launch(lookDirection, 300);

        anim.SetTrigger("Launch");
        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }


}
