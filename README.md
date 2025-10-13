# TicTacToeApi

Tic-tac-toe logic implemented through an Web API using ASP.NET.

## Usage

One endpoint: `GET /play?board=[board]`

Send the board in a URL encoded format and receive the server answer in plain text.
The board encoded format is a 9 characters string representing each 9 squares of the 
Tic-tac-toe board. The squares are put by rows, from the top left one to the bottom right one.

The server always plays with O. 

## How to build

Requirements:
- .NET 9.0

`dotnet build`

