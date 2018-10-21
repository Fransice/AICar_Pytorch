using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Evolution : MonoBehaviour
{
    public static System.DateTime startTime;
    public GameObject agentPrefab;
    public Transform agentsParent;
    public int populationSize;
    public int[] layerShape;
    public string populationName;

    private GA ga;
    private List<Genome> genomeList = new List<Genome>();
    private List<NeuralNetwork> neuralNetworkList = new List<NeuralNetwork>();
    protected List<Agent> agentList = new List<Agent>();

    public bool loadWeights;
    public bool justRunTest;
    public TextAsset bestTextAsset;
    void Start()
    {
        CheckName();
        ResetEnvironmental();
        Init();
    }

    protected void Update()
    {
        Run();
    }

    void CheckName()
    {
        if (string.IsNullOrEmpty(populationName))
        {
            throw new System.Exception("种群名字不能为空！");
        }
        if (FindObjectsOfType<Evolution>().ToList().Find(a => a.populationName == populationName) != this)
        {
            throw new System.Exception("种群名字必须唯一！重复的名字:" + populationName);
        }
    }

    void SaveBest(double[] best, double score)
    {
        string str = "";
        for (int i = 0; i < best.Length; i++)
        {
            str += best[i] + (i == best.Length - 1 ? "" : ",");
        }
        StreamWriter sw = new StreamWriter(Path.Combine(Application.streamingAssetsPath, populationName + ".txt"));
        sw.WriteLine(str + ":" + score);
        sw.Close();
        PlayerPrefs.SetString(populationName, str);
        PlayerPrefs.Save();
    }

    void Init()
    {
        ga = new GA(populationSize);
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork nn = new NeuralNetwork(layerShape);
            if (loadWeights)
            {
                if (File.Exists(Path.Combine(Application.streamingAssetsPath, populationName + ".txt")))
                {
                    StreamReader sr = new StreamReader(Path.Combine(Application.streamingAssetsPath, populationName + ".txt"));
                    string temp = sr.ReadLine().Split(':')[0];
                    sr.Close();
                    string[] strArr = temp.Split(',');
                    double[] dw = new double[nn.weightNum];
                    for (int z = 0; z < nn.weightNum; z++)
                    {
                        dw[z] = double.Parse(strArr[z]);
                    }
                    if (i == 0)
                    {
                        nn.SetWeights(dw);
                    }
                    else
                    {
                        int index = UnityEngine.Random.Range(0, dw.Length);
                        dw[index] = UnityEngine.Random.Range(-1f, 1f);
                        nn.SetWeights(dw);
                    }
                }
                else
                {
                    Debug.LogError("没有保存最佳权重，无法加载");
                }
            }
            else
            {
                nn.RandomWeights();
            }

            Genome ge = new Genome(nn.GetWeights(), 0, nn.splitPoints);

            Agent ac = (Instantiate(agentPrefab, GetStartPos(), Quaternion.identity, agentsParent) as GameObject).GetComponent<Agent>();
            ac.name = agentPrefab.name + i;
            ac.SetInfo(nn, ge);

            neuralNetworkList.Add(nn);
            genomeList.Add(ge);
            agentList.Add(ac);
        }
    }

    void Run()
    {
        if (!justRunTest)
        {
            if (CheckGenerationEnd())
            {
                FinishGeneration();
                ResetEnvironmental();
            }
            if (isJiShi)
            {
                t += Time.deltaTime;
                jishiTxt.text = Mathf.RoundToInt(time - t).ToString(); ;
                if (t >= time)
                {
                    KillAll();
                    isJiShi = false;
                    t = 0;
                }
            }
            else
            {
                jishiTxt.text = "";
            }
        }
    }

    void FinishGeneration()
    {
        double[] best;
        double score;
        List<double[]> weightsList = ga.Run(genomeList, out best, out score);
        SaveBest(best, score);
        for (int i = 0; i < weightsList.Count; i++)
        {
            neuralNetworkList[i].SetWeights(weightsList[i]);
            agentList[i].nn = neuralNetworkList[i];
            genomeList[i] = new Genome(neuralNetworkList[i].GetWeights(), 0, neuralNetworkList[i].splitPoints);
            agentList[i].ge = genomeList[i];
            agentList[i].ResetAgent();
            agentList[i].gameObject.SetActive(true);
        }
    }

    public virtual Vector3 GetStartPos()
    {
        return Vector3.zero;
    }

    public virtual bool CheckGenerationEnd()
    {
        bool isEnd = true;
        foreach (var t in agentList)
        {
            if (t.gameObject.activeSelf)
            {
                isEnd = false;
                break;
            }
        }
        return isEnd;
    }

    public virtual void ResetEnvironmental()
    {
        startTime = System.DateTime.Now;
        foreach (var item in GameObject.FindGameObjectsWithTag("Trail"))
        {
            Destroy(item);
        }
        isJiShi = false;
        t = 0;
    }

    public Text jishiTxt;
    public bool isJiShi;
    public float time;
    public float t;
    public virtual void KillAll()
    {
        foreach (var item in agentList)
        {
            item.gameObject.SetActive(false);
        }
    }
}
