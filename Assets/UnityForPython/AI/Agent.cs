using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    internal NeuralNetwork nn;
    internal Genome ge;
    public bool isDraw;

    public virtual double[] GetInputs()
    {
        throw new System.Exception();
    }

    public virtual void ResetAgent()
    {
        throw new System.Exception();
    }

    public virtual void UseOutputs(double[] outputs)
    {
        throw new System.Exception();
    }

    public virtual void SetFitness()
    {
        throw new System.Exception();
    }

    public virtual void Draw()
    {
        throw new System.Exception();
    }

    public void SetInfo(NeuralNetwork nn, Genome ge)
    {
        this.nn = nn;
        this.ge = ge;
    }

    public void FixedUpdate()
    {
        double[] inputs = GetInputs();
        double[] outputs = nn.Run(inputs);
        UseOutputs(outputs);
        // SetFitness();
        //if (isDraw)
        //{
        //    Draw();
        //}
    }
}
