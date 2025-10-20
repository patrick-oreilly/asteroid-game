using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rigidBody;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        InvokeRepeating("CheckScreenEdges", 0.2f, 0.2f);

    }

    // Update is called once per frame
    void Update()
    {


    }
    

    private void CheckScreenEdges()
    {

        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 vel = rigidBody.linearVelocity;
        Vector3 newPos = transform.position;
        float buffer = 0.1f; // in viewport coords


        if (viewPos.x < -buffer && vel.x <= 0f) // velocity check as sanity test
        {
            Destroy(gameObject);

        }

        else if (viewPos.x > 1 + buffer && vel.x >= 0f)
        {
            Destroy(gameObject);

        }

        if (viewPos.y < -buffer && vel.z <= 0f)
        {
            Destroy(gameObject);
        }

        else if (viewPos.y > 1 + buffer && vel.z >= 0f)
            Destroy(gameObject);

        }

    }