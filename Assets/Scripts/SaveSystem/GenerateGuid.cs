using UnityEngine;
[ExecuteAlways]
public class GenerateGuid : MonoBehaviour
{
    [SerializeField] private string _GUID = "";

    public string GUID { get { return _GUID; } set { _GUID = value; } }

    private void Awake()
    {
        if (!Application.IsPlaying(gameObject))
        {
            if (_GUID == "")
            {
                _GUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
