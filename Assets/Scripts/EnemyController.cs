using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;


    Rigidbody2D rb2d;
    float timer;
    int direction = 1;
    bool broken = true;

    Animator anim;

    public ParticleSystem smokeEffect;

    private RubyController rubyController;

    void Start()
    {
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");

        if (rubyControllerObject != null)

        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();

            print("Found the RubyConroller Script!");
        }

        if (rubyController == null)

        {
            print("Cannot find GameController Script!");
        }

        rb2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
        if (!broken)
        {
            return;
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }
        Vector2 position = rb2d.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;

            anim.SetFloat("Move X", 0);
            anim.SetFloat("Move Y", direction);
        }

        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;

            anim.SetFloat("Move X", direction);
            anim.SetFloat("Move Y", 0);
        }


        rb2d.MovePosition(position);
    }

    public void Fix()
    {
        if (rubyController != null)
        {
            rubyController.changeScore(1);
        }

        broken = false;
        rb2d.simulated = false;

        anim.SetTrigger("Fixed");

        smokeEffect.Stop();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController controller = other.gameObject.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }
}
