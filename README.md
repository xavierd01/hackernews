# hackernews
MVC Core displaying data from Hacker News API.

Development challenge: 
Use https://github.com/HackerNews/API to generate a page using C# and .NET that displays the title and author of the current best stories on Hacker News.

<ul>
<li>As a User, I would like to see a list of the best stories on Hacker News.</li>
<li>As a User, I would like the ability to search best stories by title.</li>
<li>As a User, I would like the ability to search best stories by author.</li>
<li>As a User, I would like viewing and searching of best stories to be responsive. I should not have to wait more than a couple seconds for the page to load.</li>
</ul>

<p>Used dotnet cli (dotnet new mvc) to generate MVC Core template. C#/.NET/Razor</p>

<p>Used In-Memory cache. 
https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory#caching-basics</p>

<p><b>TODO</b></p>
<p>Investigate better error handling for BestStoryController.GetStoryAsnc. Currently, if there is an issue retrieving story details, creates a story object with an error message in title then saves to cache. Trouble spot would be on subsequent calls. Even if connection was restored, story id is still in cache and would return object with error in title. Possible solution; on error, do not store error story in cache.</p>
