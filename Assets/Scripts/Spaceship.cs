using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Spaceship : MonoBehaviour
{
    private Rigidbody rigidBody;
    public GameObject bulletPrefab;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        // Start checking screen edges on a repeating interval once
        InvokeRepeating("CheckScreenEdges", 0.2f, 0.2f);
    }

    void Update()
    {
        HandleMovement();

    }

    public void HandleMovement()
    {
        // New Input System usage - wouldnt compile using the old one

        if (Keyboard.current.upArrowKey.isPressed)
        {
            Debug.Log("Up Arrow Pressed");
            rigidBody.AddForce(transform.up * 500f * Time.deltaTime);
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            Debug.Log("Down Arrow Pressed");
            rigidBody.AddForce(-transform.up * 500f * Time.deltaTime);
        }

        // Since the spaceship model was created with y as the up axis, we rotate around z
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            Debug.Log("Left Arrow Pressed");
            transform.Rotate(0f, 0f, -2f * Time.deltaTime * 60f);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            Debug.Log("Right Arrow Pressed");
            transform.Rotate(0f, 0f, 2f * Time.deltaTime * 60f);
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(Shoot());
        }
    }
        
    public IEnumerator Shoot()
    {
        if (bulletPrefab == null) yield break;

        // Instantiate the bullet slightly in front of the ship --  so it doesn't collide with the ship

        Vector3 spawnPos = transform.position + transform.up * 1.5f;

        // Use the ship's rotation so the bullet faces the same direction
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            float bulletSpeed = 50f;
            if (rigidBody != null)
            {
                bulletRb.linearVelocity = rigidBody.linearVelocity + transform.up * bulletSpeed;
            }
            else
            {
                bulletRb.linearVelocity = transform.up * bulletSpeed;
            }
        }
        yield return new WaitForSeconds(0.25f);


    }
    
    private void CheckScreenEdges()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 vel = rigidBody.linearVelocity;
        Vector3 newPos = transform.position;
        float buffer = 0.1f; // in viewport coords


        if (viewPos.x < -buffer && vel.x <= 0f) // velocity check as sanity test
        {
            if(CompareTag("Bullet")){
                Destroy(gameObject);
                return; 
            }
            newPos.x = GetRightEdgeX();
            transform.position = newPos;
        }

        else if (viewPos.x > 1 + buffer && vel.x >= 0f)
        {
            if(CompareTag("Bullet")){
                Destroy(gameObject);
                return; 

            }
            newPos.x = GetLeftEdgeX();
            transform.position = newPos;
        }

        if (viewPos.y < -buffer && vel.z <= 0f)
        {
            if(CompareTag("Bullet")){
                Destroy(gameObject);
                return;
            }
            newPos.z = GetTopEdgeZ();
            transform.position = newPos;
        }

        else if (viewPos.y > 1 + buffer && vel.z >= 0f)
        {
            if(CompareTag("Bullet")){
                Destroy(gameObject);
                return;
            }
            newPos.z = GetBottomEdgeZ();
            transform.position = newPos;
        }

        float GetRightEdgeX()
        {
            // When leaving the left side, wrap to right side
            Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1.08f, 0.5f, Camera.main.transform.position.y));
            return rightEdge.x;
        }
        float GetLeftEdgeX()
        {
            // When leaving right side, wrap to left
            Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(-0.08f, 0.5f, Camera.main.transform.position.y));
            return leftEdge.x;
        }
        float GetTopEdgeZ()
        {
            // When leaving bottom, wrap arount to the top
            Vector3 topEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.08f, Camera.main.transform.position.y));
            return topEdge.z;
        }
        float GetBottomEdgeZ()
        {
            // When leaving top, wrap to bottom
            Vector3 bottomEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, -0.08f,
            Camera.main.transform.position.y));
            return bottomEdge.z;
        }
    }
}
