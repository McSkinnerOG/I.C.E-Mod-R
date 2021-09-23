using System;
using UnityEngine;

public class CharAnim2 : MonoBehaviour
{
	public CharAnim2()
	{
	}

	private void Start()
	{
		this.m_lastPos = base.transform.position;
		base.animation[this.m_attackAni].speed = this.m_attackAniSpeed;
		base.animation[this.m_dieAni].speed = this.m_dieAniSpeed;
		base.animation[this.m_runAni].speed = this.m_runAniSpeed;
		base.animation[this.m_sitAni].speed = this.m_sitAniSpeed;
		base.animation[this.m_idleAni].speed = this.m_idleAniSpeed;
	}

	private void FixedUpdate()
	{
		this.m_isMoving = ((base.transform.position - this.m_lastPos).sqrMagnitude > 0.0002f);
		this.m_lastPos = base.transform.position;
	}

	public void PlayAnimation(CharAnim2.ePose a_anim)
	{
		if (null != base.animation)
		{
			switch (a_anim)
			{
			case CharAnim2.ePose.eAttack:
				base.animation.CrossFade(this.m_attackAni, this.m_fadeDur);
				return;
			case CharAnim2.ePose.eDead:
				base.animation.CrossFade(this.m_dieAni, this.m_fadeDur);
				return;
			case CharAnim2.ePose.eSit:
				base.animation.CrossFade(this.m_sitAni, this.m_fadeDur);
				return;
			}
			base.animation.CrossFade((!this.m_isMoving) ? this.m_idleAni : this.m_runAni, this.m_fadeDur);
		}
	}

	public float m_fadeDur = 0.3f;

	public string m_attackAni = "attack";

	public float m_attackAniSpeed = 1f;

	public string m_dieAni = "die";

	public float m_dieAniSpeed = 1f;

	public string m_runAni = "run";

	public float m_runAniSpeed = 1f;

	public string m_sitAni = "sit";

	public float m_sitAniSpeed = 1f;

	public string m_idleAni = "idle";

	public float m_idleAniSpeed = 1f;

	private Vector3 m_lastPos = Vector3.zero;

	private bool m_isMoving;

	public enum ePose
	{
		eStand,
		eAttack,
		eDead,
		eSit
	}
}
