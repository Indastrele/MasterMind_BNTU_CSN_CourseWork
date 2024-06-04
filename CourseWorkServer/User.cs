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
    private Stream _endPoint;
    private UserRole _role;

    public Stream Address
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
        _role = UserRole.Guesser;
    }

    public User(Stream endPoint)
    {
        _role = UserRole.Guesser;
        _endPoint = endPoint;
    }
}