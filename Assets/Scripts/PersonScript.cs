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

    public void initialData(SimulationManager sm)
    {
        SM = sm;
        Vector2 dir = Random.insideUnitCircle.normalized;
        
        RB.velocity = dir * SM.personSpeedInMPS * SM.sizeScale;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 8) // Layer 8: PlayerObstacle
        {
            if(other.gameObject.name == "Left" || other.gameObject.name == "Right")
            {
                Vector2 vel = RB.velocity;
                vel.x *= -1f;
                RB.velocity = vel;
            }
            else if(other.gameObject.name == "Top" || other.gameObject.name == "Bottom")
            {
                
                Vector2 vel = RB.velocity;
                vel.y *= -1f;
                RB.velocity = vel;
            }
        }
        else if(other.gameObject.layer == 10) // Layer 10: PersonInfectionSpace
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
