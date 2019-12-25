public class GithubIssuesRequest
{
    public readonly string Username;
    public readonly string RepoName;

    public GithubIssuesRequest(string username, string repoName)
    {
        Username = username;
        RepoName = repoName;
    }
}