using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Ren : MonoBehaviour
{
    public Rigidbody rb;
    public int[] shape;
    public NeuralNetwork nn;
    private string wPath = "C:\\Users\\13290\\Desktop\\weights.txt";
    private string sPath = "C:\\Users\\13290\\Desktop\\store.txt";
    public List<List<double>> store;
    // Use this for initialization
    void Start()
    {
        store = new List<List<double>>();
        nn = new NeuralNetwork(shape);
        Py2Unity.Instance.RecFromPython();
        nn.LoadWeights(wPath);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float zSp = transform.InverseTransformVector(rb.velocity).x;
        float label = zSp > 0 ? 1 : -1;
        double[] inputs = new double[1] { zSp };
        double[] outputs = nn.Run(inputs);
        DoAct(outputs[0]);
        store.Add(new List<double>() { zSp, label });
        if (store.Count >= 500)
        {
            SaveStore();
        }
    }

    void DoAct(double act)
    {
        rb.AddForce(transform.right * (float)act);
    }

    void SaveStore()
    {
        StreamWriter sw = new StreamWriter(sPath, false);
        for (int i = 0; i < store.Count; i++)
        {
            string data = "";
            for (int j = 0; j < store[i].Count; j++)
            {
                data += store[i][j];
                if (j != store[i].Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
        }
        sw.Close();
        store.Clear();
        Py2Unity.Instance.SendToPython("learn");
        Py2Unity.Instance.RecFromPython();
        nn.LoadWeights(wPath);
        Debug.Log("send_ok");
    }
}
