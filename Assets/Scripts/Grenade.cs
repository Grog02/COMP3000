    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Assertions.Must;
    using UnityEngine.UIElements;

    public class Grenade : MonoBehaviour
    {
        private Vector3 targetPosition;

        private Action OnGrenadeImpact; 

        // Prefab for Grenade Explosion with Particle System
        [SerializeField] private Transform grenadeExpolosion;
        
        // Trail Renderer for when grenade is thrown
        [SerializeField] private TrailRenderer trailRenderer;

        // Curve for trajectory of grenade
        [SerializeField] private AnimationCurve AnimationCurve;
        public static event EventHandler OnAnyGrenadeExplode;

        private float totalDistance;
        private Vector3 positionXZ;


        private void Update() 
        {
            // Calculate movement direction and speed
            Vector3 moveDirection = (targetPosition - positionXZ).normalized;
            float moveSpeed = 15f;
            positionXZ += moveDirection * moveSpeed * Time.deltaTime;

            // Distance between grenade and target
            float distance = Vector3.Distance(positionXZ, targetPosition);


            // Normalise distance to range of 0 - 1
            float distanceNormalized = 1 - distance / totalDistance;
            float reachedTargetDistance = 0.2f;

            // Maximum height of the trajectory
            float maxHeight = totalDistance / 4f;

            // Calculate vertical position using the animation curve
            float posY = AnimationCurve.Evaluate(distanceNormalized) * maxHeight;
            // Update the grenade position
            transform.position = new Vector3(positionXZ.x, posY, positionXZ.z);
            if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
            {
                // Damage radius and collision detection
                float damageRadius = 4f;
                Collider[] colliders = Physics.OverlapSphere(targetPosition, damageRadius);
                foreach (Collider collider in colliders)
                {
                    // Damage any units and destroy any crates in the radius
                    if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                    {
                        targetUnit.Damage(20);
                    }
                    if(collider.TryGetComponent<Crate>(out Crate crate))
                    {
                        crate.Damage();
                    }
                }

                OnAnyGrenadeExplode?.Invoke(this, EventArgs.Empty);
                // Detacth trail renderer child object and intiate explosion 
                trailRenderer.transform.parent = null;
                Instantiate(grenadeExpolosion, targetPosition + Vector3.up * 1f, quaternion.identity);  
                // Destroy grenade object after explosion starts
                Destroy(gameObject);

                OnGrenadeImpact();
            }
        }
        public void Setup(GridPosition targetGridPosition, Action OnGrenadeImpact)
        {
            
            this.OnGrenadeImpact = OnGrenadeImpact;
            // Get world position of the target grid position
            targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

            // Set initial XZ position of the grenade 
            positionXZ = transform.position;
            positionXZ.y = 0;
            // Distance between the initial and target grid position
            totalDistance = Vector3.Distance(positionXZ, targetPosition);
        }

    }
