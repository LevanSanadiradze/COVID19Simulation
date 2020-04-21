using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public Object PersonPrefab;

    [Range(0.5f, 100.0f)] public float timeScale = 1.0f;
    [Range(0.1f, 10.0f)] public float sizeScale = 0.1f;

    [Range(1, 1000)] public int numberOfPeoplePerInitialRoom = 100;
    public int initialInfectedPeople = 1;
    
    public float infectionRadiusInMeters = 2.0f;
    public float infectionChance = 0.2f;
    public float recoveryChance = 0.001f;

    public float personSpeedInMPS = 5.0f;


    [HideInInspector] public float personScale = 0f;
    private float minPersonScale = 1f;
    private float maxPersonScale = 5f;

    private Transform PeopleContainer;
    public Transform MainArea;

    void Start()
    {
        PeopleContainer = GameObject.Find("PeopleContainer").transform;

        calculatePersonScale();

        
        float halfPS = 0.5f * personScale;


        List<RoomScript> rooms = getSpawnRooms();

        foreach(RoomScript room in rooms)
        {
            for(int i = 0; i < numberOfPeoplePerInitialRoom; i++)
            {
                Transform space = getRandomSpaceOfRoom(room);

                float xScale = (space.lossyScale.x / 2.0f) - halfPS;
                float yScale = (space.lossyScale.y / 2.0f) - halfPS;
                Vector2 xSpawnBorders = new Vector2(space.position.x - xScale , space.position.x + xScale);
                Vector2 ySpawnBorders = new Vector2(space.position.y - yScale, space.position.y + yScale);

                addAPerson(xSpawnBorders, ySpawnBorders);
            }
        }

        initiallyInfectPeople();
    }

    private List<RoomScript> getSpawnRooms()
    {
        List<RoomScript> rooms = new List<RoomScript>();

        foreach(Transform row in MainArea)
        {
            foreach(Transform district in row)
            {
                rooms.Add(district.GetComponent<RoomScript>());
            }
        }

        return rooms;
    }

    public static Transform getRandomSpaceOfRoom(RoomScript room)
    {
        List<float> areas = new List<float>();
        float sumOfAreas = 0.0f;

        foreach(Transform space in room.MySpaces)
        {
            float area = space.localScale.x * space.localScale.y;
            sumOfAreas += area;
            areas.Add(area);
        }

        float rand = Random.Range(0f, sumOfAreas);

        float curSum = 0.0f;
        for(int i = 0; i < areas.Count; i++)
        {
            curSum += areas[i];

            if(rand <= curSum)
            {
                return room.MySpaces[i];
            }
        }

        return room.MySpaces[room.MySpaces.Count - 1];
    }

    private void initiallyInfectPeople()
    {
        for(int i = 0; i < initialInfectedPeople; i++)
        {
            PeopleContainer.GetChild(i).GetComponent<PersonScript>().setInfected();
        }
    }

    void Update()
    {
        onTimeScaleUpdate();
        onSizeScaleUpdate();
    }

    private void calculatePersonScale()
    {
        personScale = Mathf.Max(sizeScale, minPersonScale);
        personScale = Mathf.Min(personScale, maxPersonScale);
    }

    private void addAPerson(Vector2 xBorders, Vector2 yBorders)
    {
        Vector3 pos = new Vector3(Random.Range(xBorders.x, xBorders.y), Random.Range(yBorders.x, yBorders.y), 0f);

        GameObject person = GameObject.Instantiate(PersonPrefab, pos, new Quaternion(0, 0, 0, 0), PeopleContainer) as GameObject;
        person.transform.Find("PersonVisual").localScale = new Vector3(personScale, personScale, personScale);

        PersonScript PS = person.GetComponent<PersonScript>();

        PS.initialData(this);
    }

    private void onTimeScaleUpdate()
    {
        Time.timeScale = timeScale;
    }

    private void onSizeScaleUpdate()
    {
        calculatePersonScale();
        foreach(Transform t in PeopleContainer)
        {
            t.Find("PersonVisual").localScale = Vector3.one * personScale;
        }
    }
}
