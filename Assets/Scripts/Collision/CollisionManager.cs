using System.Collections;
using System.Collections.Generic;
using static CollisionDetection;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollisionManager : MonoBehaviour
{
    Octree tree;

    public enum CollisionType
    {
        Standard,
        Octree
    }

    public static CollisionType collisionType = CollisionType.Standard;

    [SerializeField]
    public uint nStartingParticles = 100;

    [SerializeField]
    private GameObject particlePrefab;

    private void Start()
    {
        // TODO: YOUR CODE HERE
        // Create the Octree. Create prefabs within the bounding box of the scene.
    }

    private void TreeCollisionResolution()
    {
        // TODO: YOUR CODE HERE
        // Perform sphere-sphere collisions using the Octree
    }

    private void StandardCollisionResolution()
    {
        Sphere[] spheres = FindObjectsOfType<Sphere>();
        PlaneCollider[] planes = FindObjectsOfType<PlaneCollider>();
        for (int i = 0; i < spheres.Length; i++)
        {
            Sphere s1 = spheres[i];
            for (int j = i + 1; j < spheres.Length; j++)
            {
                Sphere s2 = spheres[j];
                ApplyCollisionResolution(s1, s2);
            }
            foreach (PlaneCollider plane in planes)
            {
                ApplyCollisionResolution(s1, plane);
            }
        }
    }

    private void FixedUpdate()
    {
        CollisionChecks = 0;

        // TODO: YOUR CODE HERE
        // Call correct collision resolution type based
        // on collisionType variable.
    }

    private void Update()
    {
        // TODO: YOUR CODE HERE
        // Switch collision types if the "C" key is pressed.
    }
}
