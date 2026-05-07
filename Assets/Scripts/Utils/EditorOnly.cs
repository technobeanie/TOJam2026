using UnityEngine;

public class EditorOnly : MonoBehaviour
{
    private void Awake()
    {
		DestroyImmediate(gameObject);
    }
}
