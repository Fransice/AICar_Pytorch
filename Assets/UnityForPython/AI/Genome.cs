using System;
using System.Collections.Generic;
public class Genome
{
    public double[] weights;
    public double fitness;
    public int[] splitPoints;

    public Genome(double[] weights, double fitness, int[] splitPoints)
    {
        this.weights = (double[])weights.Clone();
        this.fitness = fitness;
        this.splitPoints = (int[])splitPoints.Clone();
    }
}

