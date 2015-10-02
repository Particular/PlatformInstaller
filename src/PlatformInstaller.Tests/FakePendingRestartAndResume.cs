public class FakePendingRestartAndResume : PendingRestartAndResume
{
    public override bool ResumedFromRestart => false;
}