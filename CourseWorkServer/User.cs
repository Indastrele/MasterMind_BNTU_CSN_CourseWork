using System.Net;
using System.Net.Sockets;

namespace Server;

public enum UserRole
{
    Cryptographer,
    Guesser
}

public class User
{
    private Socket _endPoint;
    private UserRole _role;

    public Socket Address
    {
        get => _endPoint;
        set => _endPoint = value;
    }

    public UserRole Role
    {
        get => _role;
        set => _role = value;
    }

    public User()
    {
    }

    public User(Socket endPoint)
    {
        _endPoint = endPoint;
    }
}