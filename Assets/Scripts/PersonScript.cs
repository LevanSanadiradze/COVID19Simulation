using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonScript : MonoBehaviour
{
    private enum PersonState 
    {
        Susceptible = 0,
        Infected = 1,
        Recovered = 2
    }

    private PersonState state = PersonState.Susceptible;
    private int infectedPeopleNear = 0;

    private SimulationManager SM;

    private Rigidbody2D RB;
    private Vector2 Velocity;
    private GameObject InfectionSpace;

    void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        InfectionSpace = transform.Find("InfectionSpace").gameObject;
        InfectionSpace.SetActive(false);
    }

    void Update()
    {
        if(state == PersonState.Susceptible && infectedPeopleNear > 0)
        {
            float dChance = Mathf.Pow(1.0f - SM.infectionChance, Time.deltaTime);
            float stayHealthyChance = Mathf.Pow(dChance, infectedPeopleNear);
            float rand = Random.Range(0.0f, 1.0f);

            if(stayHealthyChance < rand)
            {
                setInfected();
            }
        }
        else if(state == PersonState.Infected)
        {
            float dChance = Mathf.Pow(1.0f - SM.recoveryChance, Time.deltaTime);
            float rand = Random.Range(0.0f, 1.0f);

            if(dChance < rand)
            {
                setRecovered();
            }
        }
    }

    void FixedUpdate()
    {
        RB.velocity = Velocity;
    }

    public void initialData(SimulationManager sm)
    {
        SM = sm;
        Vector2 dir = Random.insideUnitCircle.normalized;
        
        Velocity = dir * SM.personSpeedInMPS * SM.sizeScale;
    }

    public void setInfected()
    {
        if(state != PersonState.Susceptible) return;

        state = PersonState.Infected;
        transform.Find("PersonVisual").GetComponent<SpriteRenderer>().color = Color.red;
        InfectionSpace.SetActive(true);
        InfectionSpace.GetComponent<CircleCollider2D>().radius = SM.infectionRadiusInMeters * SM.sizeScale;
    }

    public void setRecovered()
    {
        if(state != PersonState.Infected) return;

        state = PersonState.Recovered;
        transform.Find("PersonVisual").GetComponent<SpriteRenderer>().color = Color.blue;
        InfectionSpace.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.layer == 8) // Layer 8: PlayerObstacle
        {
            Vector2 normalsSum = Vector2.zero;

            for(int i = 0; i < other.contactCount; i++)
            {
                normalsSum += other.GetContact(i).normal;
            }

            Velocity = Vector2.Reflect(Velocity, normalsSum);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 10) // Layer 10: PersonInfectionSpace
        {
            if(state != PersonState.Susceptible) return;
            infectedPeopleNear ++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == 10) // Layer 10: PersonInfectionSpace
        {
            if(state != PersonState.Susceptible) return;
            infectedPeopleNear --;
        }
    }
}
