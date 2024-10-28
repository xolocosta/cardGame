using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject _testPrefab;
    [SerializeField] GameObject _testGameObject;
    private void Start()
    {
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var res = PrefabUtility.GetPrefabInstanceStatus(_testGameObject);
            Debug.Log(res);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            var settings = new ConvertToPrefabInstanceSettings();
            settings.changeRootNameToAssetName = true;
            settings.logInfo = true;
            settings.objectMatchMode = ObjectMatchMode.ByName;
            PrefabUtility.ConvertToPrefabInstance(_testGameObject, _testPrefab, settings, InteractionMode.UserAction);
            Debug.Log(_testGameObject);
        }
    }
}
