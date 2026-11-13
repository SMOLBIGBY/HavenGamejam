using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserRaycastScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ShootLaser();
        }

    }

    void ShootLaser()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;
        float distance = 10f;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, direction, distance);

        if (raycastHit2D.collider != null)
        {
            Debug.Log("Hit " + raycastHit2D.collider.name);

            // Draw ray up to the hit point
            Debug.DrawLine(origin, raycastHit2D.point, Color.red, 0.5f);
        }
        else
        {
            // Draw full ray length if nothing was hit
            Debug.DrawRay(origin, direction * distance, Color.green, 0.5f);
        }
    }
}
