using System.Collections.Generic;
public class GitHubIssuesResponse
{
    public IList<GithubIssue> Issues { get; set; }
}
public class GithubIssue
{
    public string Url { get; set; }
    public string Repository_Url { get; set; }
    public string Comments_Url { get; set; }
    public string Events_Url { get; set; }
    public string Html_Url { get; set; }
    public int Id { get; set; }
    public string Node_id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public GitHubUser User { get; set; }
    public IList<string> Labels { get; set; }
    public string State { get; set; }
    public string Locked { get; set; }
    public string Assignee { get; set; }
    public int Comments { get; set; }
    public string Body { get; set; }
}

public class GitHubUser
{
    public string Login { get; set; }
    public int Id { get; set; }
    public string Node_Id { get; set; }
    public string Avatar_Url { get; set; }
    public string Gravatar_Url { get; set; }
    public string Url { get; set; }
    public string Type { get; set; }
    public string Repos_Url { get; set; }
    public bool Site_Admin { get; set; }
}