using System;
using UnityEngine;

public class NeuralNet
{
	public NeuralNet()
	{
	}

	public void Init(int a_layers = 3, int a_ioCount = 3)
	{
		this.m_layerCount = a_layers;
		this.m_inputOutputCount = a_ioCount;
		this.m_neuralNet = new float[this.m_layerCount, this.m_inputOutputCount, this.m_inputOutputCount];
		this.InitNetRandomly();
	}

	private void InitNetRandomly()
	{
		if (this.m_neuralNet == null)
		{
			return;
		}
		for (int i = 0; i < this.m_layerCount; i++)
		{
			for (int j = 0; j < this.m_inputOutputCount; j++)
			{
				for (int k = 0; k < this.m_inputOutputCount; k++)
				{
					this.m_neuralNet[i, j, k] = UnityEngine.Random.Range(-1f, 1f);
				}
			}
		}
	}

	public float[] DoNetIO(float[] a_inputs)
	{
		if (this.m_neuralNet == null)
		{
			this.Init(3, 3);
		}
		float[] array = new float[this.m_inputOutputCount];
		for (int i = 0; i < this.m_layerCount; i++)
		{
			if (i > 0)
			{
				a_inputs = array;
			}
			for (int j = 0; j < this.m_inputOutputCount; j++)
			{
				float num = 0f;
				for (int k = 0; k < this.m_inputOutputCount; k++)
				{
					num += this.m_neuralNet[i, j, k] * a_inputs[k];
				}
				array[j] = num;
			}
		}
		return array;
	}

	public void BecomeChild(NeuralNet a_mother, NeuralNet a_father)
	{
		if (this.m_neuralNet == null)
		{
			this.Init(3, 3);
		}
		for (int i = 0; i < this.m_layerCount; i++)
		{
			for (int j = 0; j < this.m_inputOutputCount; j++)
			{
				for (int k = 0; k < this.m_inputOutputCount; k++)
				{
					NeuralNet neuralNet = (UnityEngine.Random.Range(0, 2) != 0) ? a_father : a_mother;
					this.m_neuralNet[i, j, k] = neuralNet.m_neuralNet[i, j, k];
				}
			}
		}
		this.m_neuralNet[UnityEngine.Random.Range(0, this.m_layerCount), UnityEngine.Random.Range(0, this.m_inputOutputCount), UnityEngine.Random.Range(0, this.m_inputOutputCount)] = UnityEngine.Random.Range(-1f, 1f);
	}

	[HideInInspector]
	public float[,,] m_neuralNet;

	private int m_layerCount;

	private int m_inputOutputCount;
}
