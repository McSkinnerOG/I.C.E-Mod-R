using System;
using UnityEngine;

public class FakeServer : MonoBehaviour
{
	public FakeServer()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Time.time > this.m_nextUpdateTime)
		{
			if (this.m_curHouses < this.m_maxHouseCount)
			{
				Vector3 position = new Vector3(UnityEngine.Random.Range(-this.m_radius, this.m_radius), 1f, UnityEngine.Random.Range(-this.m_radius, this.m_radius));
				UnityEngine.Object.Instantiate(this.m_housePrefab, position, Quaternion.identity);
				this.m_curHouses++;
			}
			int a_id = UnityEngine.Random.Range(0, this.m_curStep);
			this.AssignInput(a_id, UnityEngine.Random.Range(0, 9));
			this.m_nextUpdateTime = Time.time + 1f / (float)this.m_curStep;
			if (this.m_curStep < this.m_maxCharCount)
			{
				this.m_curStep++;
			}
			this.m_debugGuiText.text = string.Concat(new object[]
			{
				"curStep: ",
				this.m_curStep,
				" houses: ",
				this.m_curHouses,
				" dt: ",
				Time.smoothDeltaTime
			});
		}
	}

	private void AssignInput(int a_id, int a_inputdir)
	{
		bool flag = false;
		FakePlayer[] array = (FakePlayer[])UnityEngine.Object.FindObjectsOfType(typeof(FakePlayer));
		foreach (FakePlayer fakePlayer in array)
		{
			if (a_id == fakePlayer.m_id)
			{
				fakePlayer.SetInput(a_inputdir);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.SpawnPlayer(a_id, a_inputdir);
		}
	}

	private void SpawnPlayer(int a_id, int a_inputdir = 0)
	{
		Vector3 position = new Vector3(UnityEngine.Random.Range(-this.m_radius * 0.8f, this.m_radius * 0.8f), 1f, UnityEngine.Random.Range(-this.m_radius * 0.8f, this.m_radius * 0.8f));
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_playerPrefab, position, Quaternion.identity);
		FakePlayer component = gameObject.GetComponent<FakePlayer>();
		component.m_id = a_id;
		component.SetInput(a_inputdir);
	}

	public int m_maxCharCount = 10;

	public int m_maxHouseCount = 100;

	public GameObject m_playerPrefab;

	public GameObject m_housePrefab;

	public GUIText m_debugGuiText;

	public float m_radius = 3000f;

	private int m_curHouses;

	private int m_curStep = 1;

	private float m_nextUpdateTime;
}
