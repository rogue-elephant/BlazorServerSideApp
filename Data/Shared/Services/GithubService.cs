using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BlazorServerSideApp.Data
{
    public class GithubService
    {
        public async IAsyncEnumerable<GithubIssue> GetIssuesFromRemoteAsync()
        {
            var cancellationToken = new CancellationToken();
            await foreach (var repoIssues in
            GetIssues(GetMyRepos
                (
                    "migraine-tracker",
                    "json-conversion-tool",
                    // "json-csv-tool",
                    "json-schema-to-markdown-tool"
                ), cancellationToken)
            .ReadAllAsync())
                foreach (var issue in repoIssues)
                    yield return issue;
        }

        public async Task<IList<GithubIssue>> GetIssues()
        {
            var issues = await GetIssuesFromRemoteAsync().ToListAsync();
            return issues;
        }

        private IEnumerable<GithubIssuesRequest> GetMyRepos(params string[] repoNames)
        {
            foreach (var repoName in repoNames)
                yield return new GithubIssuesRequest("rogue-elephant", repoName);
        }

        private string BuildUrl(GithubIssuesRequest request) =>
        $"https://api.github.com/repos/{request.Username}/{request.RepoName}/issues?state=open";
        private ChannelReader<GithubIssue[]> GetIssues(IEnumerable<GithubIssuesRequest> requests, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateBounded<GithubIssue[]>(new BoundedChannelOptions(50)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

            foreach (var request in requests)
                Task.Run(async () =>
                {
                    using (var client = new AsyncWebClient())
                    {
                        await channel.Writer.WriteAsync(await client.Get<GithubIssue[]>(BuildUrl(request), cancellationToken));
                        channel.Writer.Complete();
                    }
                });

            return channel.Reader;
        }
    }
}