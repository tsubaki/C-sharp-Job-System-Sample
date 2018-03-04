using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

public class BounceCube0 : MonoBehaviour 
{
	[SerializeField] Transform[] targets;

	NativeArray<float> velocity;

	void OnEnable()
	{
		velocity = new NativeArray<float>(targets.Length, Allocator.Persistent);
		for(int i=0; i< targets.Length; i++)
		{
			velocity[i] = -1;
		}
	}

	void OnDisable()
	{
		velocity.Dispose();
	}

	void Update()
	{
		// バッファの準備
		var results = new NativeArray<RaycastHit>(targets.Length, Allocator.Temp);

		for(int i=0; i<targets.Length; i++)
		{
			RaycastHit hit;
			Physics.Raycast(targets[i].localPosition, Vector3.down, out hit, 130);
			results[i] = hit;
		}

		// 重力加速度を追加
		// 距離が0.5未満なら跳ねる。
		// 終わったらraycastはもう不要なので破棄
		for(int i=0; i<targets.Length; i++)
		{

			if(	velocity[i] < 0 && 
				results[i].distance < 0.5f)
			{
				velocity[i] = 2;
			}
			velocity[i] -= 0.098f ;
		}
		results.Dispose();

		for(int i=0; i<targets.Length; i++)
		{
			targets[i].localPosition += Vector3.up * velocity[i];
		}
	}
}