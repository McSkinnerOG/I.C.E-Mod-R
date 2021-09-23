using System;
using UnityEngine;

public class NeuralAnt : MonoBehaviour
{
	public NeuralAnt()
	{
	}

	private void Start()
	{
		this.m_foods = UnityEngine.Object.FindObjectsOfType<AntFood>();
		this.m_ants = UnityEngine.Object.FindObjectsOfType<NeuralAnt>();
		this.InitNetRandomly();
	}

	private void Update()
	{
		float y = base.transform.rotation.eulerAngles.y;
		float num = this.FindFood();
		float num2 = 0f;
		this.m_inputs[0] = y;
		this.m_inputs[1] = num;
		this.m_inputs[2] = num2;
		this.m_outputs = this.DoNetIO(this.m_inputs);
		float num3 = (this.m_outputs[0] + this.m_outputs[1] + this.m_outputs[2]) / 3f;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, num3 * 180f, 0f), 0.1f);
		base.transform.position += base.transform.forward * Time.deltaTime * this.m_speed;
		Vector3 position = base.transform.position;
		if (position.x > 11f)
		{
			position.x -= 22f;
		}
		else if (position.x < -11f)
		{
			position.x += 22f;
		}
		if (position.z > 11f)
		{
			position.z -= 22f;
		}
		else if (position.z < -11f)
		{
			position.z += 22f;
		}
		base.transform.position = position;
		if (null != this.m_debugTxt)
		{
			this.m_debugTxt.text = string.Concat(new object[]
			{
				this.m_inputs[0],
				"\n",
				this.m_inputs[1],
				"\n",
				this.m_inputs[2],
				"\n",
				this.m_outputs[0],
				"\n",
				this.m_outputs[1],
				"\n",
				this.m_outputs[2],
				"\nscore: ",
				this.m_score
			});
		}
	}

	private float FindFood()
	{
		float num = 99999f;
		float result = 0f;
		for (int i = 0; i < this.m_foods.Length; i++)
		{
			if (null != this.m_foods[i])
			{
				Vector3 forward = this.m_foods[i].transform.position - base.transform.position;
				if (forward.sqrMagnitude < num)
				{
					num = forward.sqrMagnitude;
					result = Quaternion.LookRotation(forward).eulerAngles.y;
					if (num < 1f)
					{
						this.m_foods[i].Consume();
						this.m_score++;
					}
				}
			}
		}
		return result;
	}

	private float FindCompetition()
	{
		float num = 99999f;
		float result = 0f;
		for (int i = 0; i < this.m_ants.Length; i++)
		{
			if (null != this.m_ants[i] && this != this.m_ants[i])
			{
				Vector3 forward = this.m_ants[i].transform.position - base.transform.position;
				if (forward.sqrMagnitude < num)
				{
					num = forward.sqrMagnitude;
					result = Quaternion.LookRotation(forward).eulerAngles.y;
				}
			}
		}
		return result;
	}

	private void InitNetRandomly()
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					this.m_neuralNet[i, j, k] = UnityEngine.Random.Range(-1f, 1f);
				}
			}
		}
	}

	private float[] DoNetIO(float[] a_inputs)
	{
		float[] array = new float[3];
		for (int i = 0; i < 3; i++)
		{
			if (i > 0)
			{
				a_inputs = array;
			}
			for (int j = 0; j < 3; j++)
			{
				float num = 0f;
				for (int k = 0; k < 3; k++)
				{
					num += this.m_neuralNet[i, j, k] * a_inputs[k];
				}
				array[j] = num;
			}
		}
		return array;
	}

	public void BecomeChild(NeuralAnt a_mother, NeuralAnt a_father)
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					NeuralAnt neuralAnt = (UnityEngine.Random.Range(0, 2) != 0) ? a_father : a_mother;
					this.m_neuralNet[i, j, k] = neuralAnt.m_neuralNet[i, j, k];
				}
			}
		}
		this.m_neuralNet[UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 3)] = UnityEngine.Random.Range(-1f, 1f);
	}

	private const int c_netLayers = 3;

	private const int c_layerNodes = 3;

	private const int c_nodeWeights = 3;

	private const int c_ioCount = 3;

	public float m_speed = 3f;

	public GUIText m_debugTxt;

	[HideInInspector]
	public float[,,] m_neuralNet = new float[3, 3, 3];

	private float[] m_inputs = new float[3];

	private float[] m_outputs = new float[3];

	private AntFood[] m_foods;

	private NeuralAnt[] m_ants;

	[HideInInspector]
	public int m_score;
}
