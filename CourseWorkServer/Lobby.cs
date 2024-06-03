namespace Server;

public class Lobby
{
    private List<User> _users;
    private List<uint> _combination = new List<uint>();
    private List<List<uint>> _currentButtonsState = new List<List<uint>>();
    private int _currentPosition = 0;

    public List<User> Users
    {
        get => _users;
        set => _users = value;
    }

    public List<uint> Combination
    {
        get => _combination;
        set => _combination = value;
    }

    public List<List<uint>> CurrentButtonState
    {
        get => _currentButtonsState;
        set => _currentButtonsState = value;
    }

    public int CurrentPosition
    {
        get => _currentPosition;
        set => _currentPosition = value;
    }

    public Lobby()
    {
        _users = new List<User>();
        _currentButtonsState.Add(new List<uint> {0, 0, 0, 0});
    }
    
    public Lobby(User master)
    {
        _users = new List<User> { master };
        _currentButtonsState.Add(new List<uint> {0, 0, 0, 0});
    }
}