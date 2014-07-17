public class FakePendingRestartAndResume : PendingRestartAndResume
{
    public override bool ResumedFromRestart
    {
        get { return false; }
    }
}