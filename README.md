# .NET Core Telegram Bot Development Kit
This package offers a very simple and straightforward implementation of parts of the Telegram Bot API.

## History
This package exists for one bad and one good reason, the bad reason being that the existing more comprehensive implementations of the Telegram Bot API for .NET simply did not work for me. 
The libraries had grown too big and implemented all kinds of intelligent processing that caused my bots to become unreliable and unstable.
I created this API implementation as a very simple alternative, to act as a basis for all my bots.

The good reason is that I needed a library to power all my bots, and think there is no need to copy the code all the time... If you think this library is useful to you, feel free to use it. 
If you think you can improve it, feel free to contribute.

## Remarks

This package is built using Ninject dependency insertion. In the future it will be refactored to use dependency injection at constructor-level to eliminate the dependency on Ninject.

## Getting started

Go to the [telegram site](https://core.telegram.org/bots#6-botfather) to set up a new bot and obtain an API key.

Check out the bot-sample project. It contains sample code to get you started very quickly.

The sample code uses the `ThrottlingTelegramClient` implementation of the IMessagingClient API. This helps prevent getting needlessly delayed by the Telegram servers, 
especially when editing many messages in a chnnel or public chat. It does this by consolidating multiple message edits for the same message. This is especially useful when
your telegram bot updates the same message in a chat often. If you don't need this functionality, you can use the `TelegramClient` class directly. This will pass all requests
to the telegram servers without any additional processing. It's the simplest, purest way to use the API.

## Usage



## Documentation

Would be nice.