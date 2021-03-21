using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The DNA class defines the shape of a basic DNA structure for every individual in a generation. 
 * It can be used to either spawn a completely randomized genome for an individual, or to derive a
 * genome for the individual based on mutation, as well as inherited traits from its defined parents
 * from a previous generation. 
 */

public class DNA
{
    //A list of Vector2 positions which an agent possessing the DNA will path to over its lifetime. 
    public List<Vector2> genome = new List<Vector2>();

    //Basic constructor for initializing a totally randomized genome.
    public DNA(int genomeLength = 50)
    {
        for (int i = 0; i < genomeLength; i++)
        {
            genome.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
        }
    }
    
    /*
     * A constructor for initializing a genome based on two given parental genomes, with a mutation chance
     * resulting in a completely random gene, not based on either parent. 
     */
    public DNA(DNA parent, DNA partner, float mutationRate = 0.01f)
    {
        for (int i = 0; i < parent.genome.Count; i++)
        {
            float mutationChance = Random.Range(0.0f, 1.0f);

            if (mutationChance <= mutationRate)
            {
                genome.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
            }
            else
            {
                int inheritChance = Random.Range(0, 2);

                if (inheritChance == 0)
                {
                    genome.Add(parent.genome[i]);
                }
                else
                {
                    genome.Add(partner.genome[i]);
                }
            }
        }
    }
}
