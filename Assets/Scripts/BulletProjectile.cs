using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{

    [SerializeField] private TrailRenderer trailRenderer;

    [SerializeField] private Transform bulletHitEffect;
    private Vector3 targetPosition;

    public void SetUp(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update() 
    {    

        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float distanceBeforeMove = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 100f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float distanceAfterMove = Vector3.Distance(transform.position, targetPosition);

        if(distanceBeforeMove < distanceAfterMove)
        {
            transform.position = targetPosition;

            trailRenderer.transform.parent = null;
            Destroy(gameObject);

            Instantiate(bulletHitEffect, targetPosition, Quaternion.identity);
        }
    }
}
