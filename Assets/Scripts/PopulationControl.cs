using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The PopulationControl class manages the generational progression of the algorithm, creating new populations based on evolutions of previous ones.
 * It utilizes the fitness of agents to determine if that agent survives and is allowed to "reproduce", carrying a portion of their genome to the next population. 
 */
public class PopulationControl : MonoBehaviour
{
    List<Pathfinder> population = new List<Pathfinder>();
    public GameObject agentPrefab;
    public int populationSize = 100;
    public int genomLength;
    public float cutOff = 0.3f;
    public int survivorsKept = 5;
    [Range(0f, 1f)]
    public float mutationRate = 0.01f;
    public Transform spawnPoint;
    public Transform endPoint;

    void InitPopulation()
    {
        for(int i = 0; i < populationSize; i++)
        {
            GameObject agentGo = Instantiate(agentPrefab, spawnPoint.position, Quaternion.identity);
            agentGo.GetComponent<Pathfinder>().InitAgent(new DNA(genomLength), endPoint.position);
            population.Add(agentGo.GetComponent<Pathfinder>());
        }
    }

    void NextGeneration()
    {
        int survivorCut = Mathf.RoundToInt(populationSize * cutOff);
        List<Pathfinder> survivors = new List<Pathfinder>();

        for(int i = 0; i < survivorCut; i++)
        {
            survivors.Add(GetFittest());
        }

        for(int i = 0; i < population.Count; i++)
        {
            Destroy(population[i].gameObject);
        }

        population.Clear();

        for(int i = 0; i < survivorsKept; i++)
        {
            //This will allow a creature thats pretty damn good to be kept.
            GameObject goAgent = Instantiate(agentPrefab, spawnPoint.position, Quaternion.identity);
            goAgent.GetComponent<Pathfinder>().InitAgent(survivors[i].agentDNA, endPoint.position);
            population.Add(goAgent.GetComponent<Pathfinder>());
        }

        while (population.Count < populationSize)
        {
            for(int i = 0; i < survivors.Count; i++)
            {
                GameObject goAgent = Instantiate(agentPrefab, spawnPoint.position, Quaternion.identity);
                goAgent.GetComponent<Pathfinder>().InitAgent(new DNA(survivors[i].agentDNA, survivors[Random.Range(0, 10)].agentDNA, mutationRate), endPoint.position);
                population.Add(goAgent.GetComponent<Pathfinder>());

                if(population.Count >= populationSize)
                {
                    break;
                }
            }
        }

        for(int i = 0; i < survivors.Count; i++)
        {
            Destroy(survivors[i].gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitPopulation();
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasActive())
        {
            NextGeneration();
        }
    }

    Pathfinder GetFittest()
    {
        float maxFitness = float.MinValue;
        int index = 0;
        for(int i = 0; i < population.Count; i++)
        {
            if(population[i].Fitness > maxFitness)
            {
                maxFitness = population[i].Fitness;
                index = i;
            }
        }

        Pathfinder fittest = population[index];
        population.Remove(fittest);
        return fittest;
    }

    bool hasActive()
    {
        for(int i = 0; i < population.Count; i++)
        {
            if(!population[i].hasFinished)
            {
                return true;
            }
        }

        return false;
    }

}
