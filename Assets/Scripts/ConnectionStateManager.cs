using UnityEngine;
using UnityEngine.Events;

public enum ConnectionState
{
    DisconnectedFromServer,
    ConnectedToServer,
    WaitingForConfigurator,
    ConnectedToConfigurator,
    DisconnectedFromConfigurator
}

public class ConnectionStateManager : MonoBehaviour
{
    [Header("Current State")]
    [SerializeField] private ConnectionState _currentState = ConnectionState.DisconnectedFromServer;

    [Header("State Events")]
    public UnityEvent OnConnectedToServer;
    public UnityEvent OnWaitingForConfigurator;
    public UnityEvent OnConnectedToConfigurator;
    public UnityEvent OnDisconnectedFromConfigurator;
    public UnityEvent OnDisconnectedFromServer;

    public ConnectionState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged(_currentState);
            }
        }
    }

    private void OnStateChanged(ConnectionState newState)
    {
        switch (newState)
        {
            case ConnectionState.ConnectedToServer:
                OnConnectedToServer.Invoke();
                Debug.Log("Client connected to server - waiting for configurator");
                break;

            case ConnectionState.WaitingForConfigurator:
                OnWaitingForConfigurator.Invoke();
                Debug.Log("Client (tab) waiting for configurator to accept");
                break;

            case ConnectionState.ConnectedToConfigurator:
                OnConnectedToConfigurator.Invoke();
                Debug.Log("Client (tab) connected to configurator");
                break;

            case ConnectionState.DisconnectedFromConfigurator:
                OnDisconnectedFromConfigurator.Invoke();
                Debug.Log("Client disconnected from configurator");
                break;

            case ConnectionState.DisconnectedFromServer:
                OnDisconnectedFromServer.Invoke();
                Debug.Log("Configurator (Server) disconnected");
                break;
        }
    }

    public void ConnectToServer()
    {
        CurrentState = ConnectionState.ConnectedToServer;
    }

    public void WaitForConfigurator()
    {
        if(CurrentState != ConnectionState.ConnectedToConfigurator)
            CurrentState = ConnectionState.WaitingForConfigurator;
    }

    public void ConfiguratorAccepted()
    {
        if (CurrentState == ConnectionState.WaitingForConfigurator)
        {
            CurrentState = ConnectionState.ConnectedToConfigurator;
        }
    }

    public void DisconnectFromConfigurator()
    {
        if (CurrentState == ConnectionState.ConnectedToConfigurator)
        {
            CurrentState = ConnectionState.DisconnectedFromConfigurator;
        }
    }

    public void DisconnectFromServer()
    {
        if (CurrentState == ConnectionState.ConnectedToConfigurator || CurrentState == ConnectionState.WaitingForConfigurator)
        {
            CurrentState = ConnectionState.DisconnectedFromServer;
        }
    }
}