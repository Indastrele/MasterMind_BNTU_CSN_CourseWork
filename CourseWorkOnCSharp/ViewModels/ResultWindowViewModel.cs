namespace CourseWorkOnCSharp.ViewModels;

public class ResultWindowViewModel : ViewModelBase
{
    public string Result
    {
        get;
        set;
    } = "I will give you my all!";

    public ResultWindowViewModel()
    {
        
    }

    public ResultWindowViewModel (string result)
    {
        Result = result;
    }
}