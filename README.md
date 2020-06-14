# StoreBot - WIP
StoreBot is a *work in progress* Discord bot that makes use of [StoreLib](https://github.com/TitleOS/StoreLib). Current features include printing of moderate listing details and FE3 package links.

## Usage:

Clone the repo and build StoreBot using Visual Studio 2019. Define your [Discord Bot Token](https://discordapp.com/developers/applications) in your environment variables, named "STOREBOTTOKEN". 
Run StoreBot.dll using the .NET 5 runtime:
```
dotnet StoreBot.dll or StoreBot.exe
```
Once StoreBot logs into Discord, invite the bot to the server of your choice, then refer to the "Commands" section below.


### Commands:

```
@StoreBot ProductID (Optional Endpoint)
```
