using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Asteroid : MonoBehaviour
{
    // inspector settings
    public Rigidbody rigidBody;
    public GameObject asteroidFragmentPrefab;
    public int numberOfParticles = 1;
    //
    // Use this for initialization
    void Start()
    {

        rigidBody = GetComponent<Rigidbody>();
        transform.localScale = new Vector3(Random.Range(0.06f, 0.09f), Random.Range(0.06f, 0.09f), Random.Range
        (0.06f, 0.09f));


        rigidBody.mass = transform.localScale.x * transform.localScale.y * transform.localScale.z;
        // randomise velocity
        rigidBody.linearVelocity = new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f));
        rigidBody.angularVelocity = new Vector3(Random.Range(-2f, 2f), Random.Range(-
        2f, 2f), Random.Range(-2f, 2f));
        // start periodically checking for being off-scree
        InvokeRepeating("CheckScreenEdges", 0.2f, 0.2f);

    }

    public void OnCollisionEnter(Collision collision)
    {
        // Only spawn particles if not colliding with another particle

        if (collision.gameObject.tag != "Asteroid Fragment")
        {
            SpawnParticleEffect(collision.contacts[0].point);
        }

        if (collision.gameObject.tag == "bullet")
        {
            fragmentAsteroid();
            Destroy(collision.gameObject);

        }



    }
   
    
    private void SpawnParticleEffect(Vector3 collisionPoint)
    {
        if (asteroidFragmentPrefab == null) return;
        for(int i = 0; i < numberOfParticles; i++)
        {
            GameObject particle = Instantiate(asteroidFragmentPrefab, collisionPoint, Quaternion.identity);
            Rigidbody particleRb = particle.GetComponent<Rigidbody>();
            if (particleRb != null)
            {
                particleRb.linearVelocity = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                particleRb.angularVelocity = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            }
            Destroy(particle, 1f); // destroy particles after 2 seconds to clean up
        }
    }

    private void fragmentAsteroid()
    {
        // To be implemented in future versions
    }
    private void CheckScreenEdges()
    {

        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 vel = rigidBody.linearVelocity;
        Vector3 newPos = transform.position;
        float buffer = 0.1f; // in viewport coords


        if (viewPos.x < -buffer && vel.x <= 0f) // velocity check as sanity test
        {
            newPos.x = GetRightEdgeX();
            transform.position = newPos;
        }

        else if (viewPos.x > 1 + buffer && vel.x >= 0f)
        {
            newPos.x = GetLeftEdgeX();
            transform.position = newPos;
        }

        if (viewPos.y < -buffer && vel.z <= 0f)
        {
            newPos.z = GetTopEdgeZ();
            transform.position = newPos;
        }

        else if (viewPos.y > 1 + buffer && vel.z >= 0f)
        {
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