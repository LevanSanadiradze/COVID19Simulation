using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonScript : MonoBehaviour
{
    private bool infected = false;
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
        if(!infected && infectedPeopleNear > 0)
        {
            float dChance = Mathf.Pow(1.0f - SM.infectionChance, Time.deltaTime); //1.0f - Mathf.Pow(SM.infectionChance, Time.deltaTime);
            float stayHealthyChance = Mathf.Pow(dChance, infectedPeopleNear);
            float rand = Random.Range(0.0f, 1.0f);

            if(stayHealthyChance < rand)
            {
                setInfected();
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
        infected = true;
        transform.Find("PersonVisual").GetComponent<SpriteRenderer>().color = Color.red;
        InfectionSpace.SetActive(true);
        InfectionSpace.GetComponent<CircleCollider2D>().radius = SM.infectionRadiusInMeters * SM.sizeScale;
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
            if(infected) return;
            infectedPeopleNear ++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == 10) // Layer 10: PersonInfectionSpace
        {
            if(infected) return;
            infectedPeopleNear --;
        }
    }
}
