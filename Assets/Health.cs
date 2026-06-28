using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{





    public int hp = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            hp -= 50;
            Debug.Log("!!!");
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

