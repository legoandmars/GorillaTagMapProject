using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildEvents
{
	void OnBuild(VmodMonkeMapLoader.Behaviours.MapDescriptor mapDescriptor);
}
