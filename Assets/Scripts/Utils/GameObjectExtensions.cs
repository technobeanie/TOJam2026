using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Utils
{
	public static class GameObjectExtensions
	{
		// const

		// public

		// protected

		// private

		// properties

		#region Unity Methods
		#endregion

		#region Public MethodsSet
		public static Bounds GetBounds(this GameObject obj, bool excludeParticleSystems = false)
		{
			Bounds bounds = new Bounds(obj.transform.position, Vector3.one);
			bool found = false;

			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < renderers.Length; ++i)
			{
				if (!excludeParticleSystems || !(renderers[i] is ParticleSystemRenderer))
				{
					TextMeshPro textMesh = renderers[i].GetComponent<TextMeshPro>();
					if (textMesh != null)
					{
						textMesh.ForceMeshUpdate();
					}

					if (!found)
					{
						found = true;
						bounds = renderers[i].bounds;
					}
					else
					{
						bounds.Encapsulate(renderers[i].bounds);
					}
				}
			}

			return bounds;
		}

		public static void SetLayerRecursively(this GameObject obj, string layerName)
		{
			Transform[] transforms = obj.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < transforms.Length; ++i)
			{
				transforms[i].gameObject.layer = LayerMask.NameToLayer(layerName);
			}
		}

		public static void SetProperFitScale(this GameObject obj, Renderer[] objRenderers, Collider collider, bool includeZAxis = false, bool resetLocalScale = true)
		{
			if (obj == null || objRenderers == null || collider == null)
			{
				return;
			}

			if (resetLocalScale)
			{
				obj.transform.localScale = Vector3.one;
			}

			Vector3 objMin = Vector2.zero;
			Vector3 objMax = Vector2.zero;
			Vector3 objCenter = Vector3.zero;

			GetMinMax(objRenderers, out objMin, out objMax);
			objCenter = Vector3.Lerp(objMin, objMax, 0.5f);

			float objScale = 1.0f;
			objScale = Mathf.Min(objScale, (collider.bounds.extents.x) / (objCenter.x - objMin.x));
			objScale = Mathf.Min(objScale, (collider.bounds.extents.y) / (objCenter.y - objMin.y));
			objScale = Mathf.Min(objScale, (collider.bounds.extents.x) / (objMax.x - objCenter.x));
			objScale = Mathf.Min(objScale, (collider.bounds.extents.y) / (objMax.y - objCenter.y));

			if (includeZAxis)
			{
				objScale = Mathf.Min(objScale, (collider.bounds.extents.z) / (objCenter.z - objMin.z));
				objScale = Mathf.Min(objScale, (collider.bounds.extents.z) / (objMax.z - objCenter.z));

				obj.transform.localScale = new Vector3(objScale, objScale, objScale);
			}
			else
			{
				obj.transform.localScale = new Vector3(objScale, objScale, obj.transform.localScale.z);
			}

			GetMinMax(objRenderers, out objMin, out objMax);
			objCenter = Vector3.Lerp(objMin, objMax, 0.5f);

			if (includeZAxis)
			{
				obj.transform.position += new Vector3(collider.bounds.center.x - objCenter.x, collider.bounds.center.y - objCenter.y, collider.bounds.center.z - objCenter.z);
			}
			else
			{
				obj.transform.position += new Vector3(collider.bounds.center.x - objCenter.x, collider.bounds.center.y - objCenter.y, 0.0f);
			}
		}

		public static void GetMinMax(Renderer[] objRenderers, out Vector3 objMin, out Vector3 objMax)
		{
			objMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			objMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			for (int i = 0; i < objRenderers.Length; ++i)
			{
				if (objRenderers[i].enabled)
				{
					Bounds bounds = objRenderers[i].bounds;

					SpriteRenderer spriteRenderer = objRenderers[i] as SpriteRenderer;
					if (spriteRenderer != null && spriteRenderer.sprite != null)
					{
						Vector2[] vertices = spriteRenderer.sprite.vertices;
						for (int j = 0; j < vertices.Length; ++j)
						{
							// Apply scale.
							Vector3 vertice = new Vector3(spriteRenderer.transform.position.x + (vertices[j].x * spriteRenderer.transform.lossyScale.x),
															spriteRenderer.transform.position.y + (vertices[j].y * spriteRenderer.transform.lossyScale.y),
															spriteRenderer.transform.position.z);

							// Apply rotation on the point (rotate a point around a pivot).
							vertice = (spriteRenderer.transform.rotation * (vertice - spriteRenderer.transform.position)) + spriteRenderer.transform.position;

							if (vertice.x < objMin.x) objMin.x = vertice.x;
							if (vertice.x > objMax.x) objMax.x = vertice.x;
							if (vertice.y < objMin.y) objMin.y = vertice.y;
							if (vertice.y > objMax.y) objMax.y = vertice.y;
							if (vertice.z < objMin.z) objMin.z = vertice.z;
							if (vertice.z > objMax.z) objMax.z = vertice.z;
						}
					}
					else
					{
						if (bounds.min.x < objMin.x) objMin.x = bounds.min.x;
						if (bounds.max.x > objMax.x) objMax.x = bounds.max.x;
						if (bounds.min.y < objMin.y) objMin.y = bounds.min.y;
						if (bounds.max.y > objMax.y) objMax.y = bounds.max.y;
						if (bounds.min.z < objMin.z) objMin.z = bounds.min.z;
						if (bounds.max.z > objMax.z) objMax.z = bounds.max.z;
					}
				}
			}
		}
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		#endregion
	}
}
