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

    private PlayerInRoomData roomData = null;

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
        if(roomData == null) return;

        if(state == PersonState.Susceptible)
        {
            float stayHealthyChance = Mathf.Pow(1.0f - SM.infectionChance, infectedPeopleNear);
            stayHealthyChance *= (1.0f - roomData.currentRoomScript.roomInfectionChance);
            float dChance = Mathf.Pow(stayHealthyChance, Time.deltaTime);
            float rand = Random.Range(0.0f, 1.0f);

            if(dChance < rand)
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

        goToOtherRooms();
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
        Transform visual = transform.Find("PersonVisual");
        visual.gameObject.layer = 12;
        visual.GetComponent<SpriteRenderer>().color = Color.red;
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

    private void goToOtherRooms()
    {
        RoomScript current = roomData.currentRoomScript;
        if(roomData.hasParent)
        {
            float chance =  Mathf.Pow(1.0f - current.chanceToLeaveRoom, Time.deltaTime);
            float rand = Random.Range(0.0f, 1.0f);

            if(rand > chance)
            {
                transform.position = roomData.perviousPosition;
                roomData = roomData.parentData;
            }
        }

        foreach(RoomScript rs in roomData.currentRoomScript.RoomsToGo)
        {
            float chance = Mathf.Pow(1.0f - rs.chanceToEnterRoom, Time.deltaTime);
            float rand = Random.Range(0.0f, 1.0f);

            if(rand > chance)
            {
                Transform space = SimulationManager.getRandomSpaceOfRoom(rs);
                float halfPS = 0.5f * SM.personScale;
                float xScale = (space.lossyScale.x / 2.0f) - halfPS;
                float yScale = (space.lossyScale.y / 2.0f) - halfPS;
                Vector2 xSpawnBorders = new Vector2(space.position.x - xScale, space.position.x + xScale);
                Vector2 ySpawnBorders = new Vector2(space.position.y - yScale, space.position.y + yScale);
                Vector3 pos = new Vector3(Random.Range(xSpawnBorders.x, xSpawnBorders.y), Random.Range(ySpawnBorders.x, ySpawnBorders.y), 0f);

                PlayerInRoomData PIRD = new PlayerInRoomData();
                PIRD.currentRoomScript = rs;
                PIRD.hasParent = true;
                PIRD.parentData = roomData;
                PIRD.perviousPosition = transform.position;

                transform.position = pos;
                roomData = PIRD;

                break;
            }
        }
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
        else if(other.gameObject.layer == 11) // Layer 11: Room Space
        {
            if(roomData == null)
            {
                roomData = new PlayerInRoomData();
                roomData.currentRoomScript = other.transform.parent.parent.GetComponent<RoomScript>();
            }
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
