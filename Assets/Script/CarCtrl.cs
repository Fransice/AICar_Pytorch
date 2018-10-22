using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarCtrl : MonoBehaviour
{
    public WheelCollider[] wheels;
    public Transform[] tran;
    public Rigidbody rigidbody;
    public Vector3 center;
    public float maxAngle;
    public double[] ops;
    public NeuralNetwork nn;
    public List<LineRenderer> lineList = new List<LineRenderer>();
    private string wPath = "C:\\Users\\13290\\Desktop\\weights.txt";
    private string sPath = "C:\\Users\\13290\\Desktop\\store.txt";
    public float maxMotor;
    public int num;
    public int seedis;
    public float perAngle;
    public float curSpeed;
    private Material mat;
    public LayerMask Wall;
    Vector3 StartV3;
    Quaternion StatQua;
    public int[] shape;
    public List<List<double>> store = new List<List<double>>();
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {

        StartV3 = gameObject.transform.position;
        StatQua = gameObject.transform.rotation;

        mat = new Material(Shader.Find("Legacy Shaders/Self-Illumin/Diffuse"));


        nn = new NeuralNetwork(shape);
        Py2Unity.Instance.RecFromPython();
        nn.LoadWeights(wPath);


        Vector3 vc = new Vector3(Random.value, Random.value, Random.value);
        float a = Vector3.Angle(vc, Vector3.one);
        float d = Mathf.Sin(a / 180 * Mathf.PI) * vc.magnitude;
        if (d > 0.8f)
        {
            mat.color = new Color(vc.x, vc.y, vc.z);
        }

        rigidbody.centerOfMass = center;

        for (int i = 0; i < num; i++)
        {
            LineRenderer lr = new GameObject("line" + i).AddComponent<LineRenderer>();
            lr.transform.SetParent(transform);
            lr.transform.localPosition = Vector3.zero;
            lr.transform.localRotation = Quaternion.identity;
            lr.SetWidth(0.06f, 0.06f);
            lr.material = mat;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lineList.Add(lr);
        }
    }
    void Update()
    {
        GetInputs();
        // float x = Input.GetAxis("Horizontal");
        // float y = Input.GetAxis("Vertical");
        // Move(y);
        // Turn(x);
        SetWc();
        curSpeed = rigidbody.velocity.magnitude;
    }


    public void Move(double f)
    {
        wheels[2].motorTorque = (float)f * maxMotor;
        wheels[3].motorTorque = (float)f * maxMotor;
    }
    public void Turn(double f)
    {
        wheels[0].steerAngle = (float)f * maxAngle;
        wheels[1].steerAngle = (float)f * maxAngle;
    }


    private void SetWc()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            Quaternion qua;
            wheels[i].GetWorldPose(out pos, out qua);
            tran[i].position = pos;
            tran[i].rotation = qua;
        }
    }
    double[] x = new double[2] { 0.1f, 0.2f };
    public void GetInputs()
    {
        List<double> list = new List<double>();
        Vector3 vf = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        for (int i = 1; i <= (num - 1) / 2; i++)
        {
            lineList[i].SetPosition(0, transform.position);
            lineList[num - i].SetPosition(0, transform.position);
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Quaternion.AngleAxis(-i * perAngle, Vector3.up) * vf);
            if (Physics.Raycast(ray, out hit, seedis, Wall))
            {
                list.Add(Mathf.Pow(1 - hit.distance / seedis, 1));
                lineList[i].SetPosition(1, hit.point);
            }
            else
            {
                list.Add(0);
                lineList[i].SetPosition(1, transform.position + ray.direction * seedis);
            }

            RaycastHit hit2;
            Ray ray2 = new Ray(transform.position, Quaternion.AngleAxis(i * perAngle, Vector3.up) * vf);
            if (Physics.Raycast(ray2, out hit2, seedis, Wall))
            {
                list.Add(Mathf.Pow(1 - hit2.distance / seedis, 1));
                lineList[num - i].SetPosition(1, hit2.point);
            }
            else
            {
                list.Add(0);
                lineList[num - i].SetPosition(1, transform.position + ray2.direction * seedis);
            }
        }
        lineList[0].SetPosition(0, transform.position);
        RaycastHit hit3;
        Ray ray3 = new Ray(transform.position, vf);
        if (Physics.Raycast(ray3, out hit3, seedis, Wall))
        {
            list.Add(Mathf.Pow(1 - hit3.distance / seedis, 1));
            lineList[0].SetPosition(1, hit3.point);
        }
        else
        {
            list.Add(0);
            lineList[0].SetPosition(1, transform.position + ray3.direction * seedis);
        }
        float zSp = transform.InverseTransformVector(rigidbody.velocity).z;
        float label = zSp > 0 ? 1 : -1;
        double[] inputs = new double[6] { label, list[0], list[1], list[2], list[3], list[4] };
        double[] outputs = nn.Run(inputs);
        Move(outputs[1]);
        Turn(outputs[0]);


        store.Add(new List<double>() { label, list[0], list[1], list[2], list[3], list[4] });

        if (store.Count >= 500)
        {
            SaveStore();
        }
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
    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            print("碰撞");
            gameObject.transform.position = StartV3;
            gameObject.transform.rotation = StatQua;
            double[] inputs = new double[6] { -1, 0, 0, 0, 0, 0 };
            double[] outputs = nn.Run(inputs);
            Move(outputs[1]);
            Turn(outputs[0]);
        }
    }
}
