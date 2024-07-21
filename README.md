# [<h2>BanWho?</h2>](https://banwho.info/)


BanWho? is a League of Legends champion analytics web app designed for newer or returning players. The goals of the app are as follows:
- Provide accurate and up to date data on top champion win rates, pick rates, ban rates, and matchup win rates and picks.
- Utilize this data to present users with a recommendation on the strongest champion to ban in their games.
- Present this data in a clear and intuitive way with minimal bloat.
- Enable users to quickly retrieve ban recommendations in 2-3 mouse clicks.

The data is retrieved by crawling Emerald 4+ players and matches globally, then aggregated, cleaned, and stored. The stored data is then quickly retrieved and queried to present the user with top ban contenders according to win rate and picks / pick rate.
The intention is to provide the targeted users with a starting point on which champions to ban before discovering themselves what works best for their games.

Developed with .NET 8 Blazor, Quartz, MediatR, [Camille](https://github.com/MingweiSamuel/Camille), EFC, and MudBlazor.

Hosted with Azure at [BanWho?](https://banwho.info/)
