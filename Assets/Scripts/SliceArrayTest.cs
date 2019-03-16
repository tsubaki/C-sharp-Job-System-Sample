using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
public class SliceArrayTest : MonoBehaviour {

	[SerializeField] Transform[] targets;

	[SerializeField] Transform prefab;

	[SerializeField, Range(0, 50000)] int count = 3;

	TransformAccessArray tarray;
	NativeArray<Vector3> nArray ;

	IEnumerator Start()
	{
		targets = new Transform[count];
		for(int i=0; i<count; i++)
		{
			var obj = GameObject.Instantiate(prefab, 
				new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30)), 
				Quaternion.identity);
			targets[i] = obj.transform;
		}

		tarray = new TransformAccessArray(targets);
		nArray = new NativeArray<Vector3>(1, Allocator.Persistent);
		var param = new Vector3();
		nArray[0] = param;
		TestJob job = new TestJob();

		var endOfFrame = new WaitForEndOfFrame();
		while(enabled)
		{
			var handle = job.Schedule(tarray);
			var res = nArray[0];
			res.y -= 1;
			param.x += 1;
			Debug.Log(nArray[0] + "/" + param);
			//for(int i=0; i<count; i++)
			//{
			//	tarray[i].position += Vector3.forward * 0.2f;
			//}
			yield return null;
			handle.Complete();
		}
	}

	void OnDestroy()
	{
		tarray.Dispose();
		nArray.Dispose();
	}

    struct TestJob : IJobParallelForTransform
    {
        public void Execute(int index, TransformAccess transform)
        {
			transform.position += Vector3.forward * 0.01f;
        }
    }
}
