using UnityEngine;

public class GUN : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpid = 1f;
    [SerializeField] private float bulletLifeTIME = 3f;

    [SerializeField] private float fireRate = 0.15f;

    private float nextFireTime;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPref, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpid;
        }
        Destroy(bullet, bulletLifeTIME);
    }
}
