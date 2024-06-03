namespace CourseWorkOnCSharp.ViewModels;

public class LobbyWindowViewModel
{
    private int _id;

    public int ID { 
        get => _id;
        set => _id = value; 
    }

    public string IDInfo
    {
        get => _id.ToString();
    }

    public LobbyWindowViewModel()
    {
    }
}