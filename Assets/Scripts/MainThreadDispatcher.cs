using System.Collections.Concurrent;
using UnityEngine;

/// <summary>
/// 매개변수 없는 함수를 메인스레드에서 수행
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject container = new GameObject("MainThreadDispatcher");
                container.name = "MainThreadDispathcer";
                _instance = container.AddComponent<MainThreadDispatcher>();
              //  _instance.Init();
            }

            return _instance;
        }
    }

    private ConcurrentQueue<System.Action> _executionQueue;

    public void Init()
    {
        _executionQueue = new ConcurrentQueue<System.Action>();
    }

    private void Update()
    {
        while (_executionQueue.TryDequeue(out System.Action execution))
        {
            execution?.Invoke();
        }

    }

    public void Add(System.Action execution)
    {
        _executionQueue.Enqueue(execution);
    }
}