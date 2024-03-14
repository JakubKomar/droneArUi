namespace Mapbox.Examples
{
	using Mapbox.Unity.MeshGeneration.Interfaces;
	using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class LabelTextSetter : MonoBehaviour, IFeaturePropertySettable
	{
		[SerializeField]
        public TextMeshProUGUI textMeshProComponent;

        public void Set(Dictionary<string, object> props)
		{
			if (textMeshProComponent == null)
				return;
            textMeshProComponent.text = "";

			if (props.ContainsKey("name"))
			{
                textMeshProComponent.text = props["name"].ToString();
			}
			else if (props.ContainsKey("house_num"))
			{
                textMeshProComponent.text = props["house_num"].ToString();
			}
			else if (props.ContainsKey("type"))
			{
                textMeshProComponent.text = props["type"].ToString();
			}
		}
	}
}