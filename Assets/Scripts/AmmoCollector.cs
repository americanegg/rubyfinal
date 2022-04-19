using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCollector : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.changeAmmo(3);
            Destroy(gameObject);

            controller.PlaySound(collectedClip);
        }
    }
}
