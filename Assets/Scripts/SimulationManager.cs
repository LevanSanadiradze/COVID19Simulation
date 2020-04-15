using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public Object PersonPrefab;

    [Range(0.5f, 100.0f)] public float timeScale = 1.0f;
    [Range(0.1f, 10.0f)] public float sizeScale = 0.1f;

    [Range(1, 5000)] public int numberOfPeople = 500;
    public int initialInfectedPeople = 1;
    
    public float infectionRadiusInMeters = 2.0f;
    public float infectionChance = 0.2f;
    public float recoveryChance = 0.001f;

    public float personSpeedInMPS = 5.0f;


    [HideInInspector] public float personScale = 0f;
    private float minPersonScale = 1f;
    private float maxPersonScale = 5f;

    public Transform initialSpawnSpace;

    private Vector2 xSpawnBorders = Vector2.zero;
    private Vector2 ySpawnBorders = Vector2.zero;

    private Transform PeopleContainer;

    void Awake()
    {
        PeopleContainer = GameObject.Find("PeopleContainer").transform;

        calculatePersonScale();

        
        float halfPS = 0.5f * personScale;
        float xScale = (initialSpawnSpace.lossyScale.x / 2.0f) - halfPS;
        float yScale = (initialSpawnSpace.lossyScale.y / 2.0f) - halfPS;
        xSpawnBorders = new Vector2(initialSpawnSpace.position.x - xScale , initialSpawnSpace.position.x + xScale);
        ySpawnBorders = new Vector2(initialSpawnSpace.position.y - yScale, initialSpawnSpace.position.y + yScale);


        for(int i = 0; i < numberOfPeople; i++)
        {
           addAPerson();
        }

        initiallyInfectPeople();
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

    private void addAPerson()
    {
        Vector3 pos = new Vector3(Random.Range(xSpawnBorders.x, xSpawnBorders.y), Random.Range(ySpawnBorders.x, ySpawnBorders.y), 0f);

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
