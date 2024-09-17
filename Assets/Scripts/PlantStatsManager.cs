using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStatsManager : MonoBehaviour
{
    // Length of ray cast perpendicularly from plant -> should be able to intersect with DayCollider, NightCollider, and Rain
    private float rayMaxDistance = 200f;
    // Layer that interacts with ray cast
    private LayerMask raycastMask;

    // Amount of sunlight stored in plant
    private float m_sunlightScore;
    [SerializeField] private float sunlightMax = 50f;
    [SerializeField] private float sunlightIncreaseFactor = 5f;
    private float sunlightScore
    {
        get
        {
            return m_sunlightScore;
        }
        set
        {
            if (value == m_sunlightScore)
                return;

            m_sunlightScore = value;
            if (value > sunlightMax)
                m_sunlightScore = sunlightMax;
            else if (value < 0)
                m_sunlightScore = 0;
        }
    }

    // Amount of water stored in plant
    private float m_waterScore;
    [SerializeField] private float waterMax = 50f;
    [SerializeField] private float waterIncreaseFactor = 5f;
    private float waterScore
    {
        get
        {
            return m_waterScore;
        }
        set
        {
            if (value == m_waterScore)
                return;

            m_waterScore = value;
            if (value > waterMax)
                m_waterScore = waterMax;
            else if (value < 0)
                m_waterScore = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        raycastMask = LayerMask.GetMask("DetectRaycast");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Cast ray perpendicular to the ground
        // Increase sunlight score if daytime
        // Increase water score if it is raining
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), rayMaxDistance, raycastMask);
        foreach (RaycastHit hit in hits)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            string name = hit.collider.gameObject.name;
            if (name == "DayCollider")
            {
                sunlightScore += Time.fixedDeltaTime * sunlightIncreaseFactor;
            }
            else if (name == "Rain")
            {
                waterScore += Time.fixedDeltaTime * waterIncreaseFactor;
            }
        }
    }
}
