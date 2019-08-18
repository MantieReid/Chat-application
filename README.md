# SausageChat

Sausage chat, is a fully asynchronous chatroom application with renaming, kicking, banning and muting features.
This is purely for fun and meant to help us learn about asynchronous sockets and managing threads in GUI development correctly.

Here is a GIF of the program in action. 
![alt text](https://github.com/ArchyInUse/SausageChat/blob/master/client-view-gif.gif)
![alt text](https://github.com/ArchyInUse/SausageChat/blob/master/server-view-gif.gif)


### How it works
#### Server:
The server consists of 2 main parts, ``SausageServer`` & ``SausageConnection``

SausageServer is where the main socket is located, it contains all information about the users and whenever a new user joins, it constructs a new Sausage Connection

Sausage Connection is a handler for a user, it contains the connected socket as well as handles all requests to and from the user, for example accepting and sending messages.

### Instructions
Requirements: Two separate computers. Windows 7 and up.

First, go to SausageClient.cs and go to line 24 and change the Ip address to the desired ip address for the sever view. Compile it.  Build it in release mode. 

Zip the release folder of SausageChatServer and zip it. Extract the release folder on the  computer that has the Ip address you gave in  sausageclient.cs and run the exe. Click on start server. 

Look in the release folder for sausage chat client and run the exe. Click on connect, and you should be able to send messages. 
 


#### Client:
SausageClient is a simple static class which has all the information (apart from the IP or other secret information) as well as all the methods used for interacting with the server.

#### Core:
Core contains all the base types both the client and server use. Server/User Message types, Packet formats, User (information such as name, GUID etc) and even some helper types such as SausageUserList


They interact with one another to make calls, where the server is the validator and also a sorter for requests between users while the client just sends messages and recieves information about all clients inside the chat room (information such as name change, muted, kicked, banned etc)


### Scraped features:
We've decided to ditch 2 features for a lack of infastrucutre and the amount of maintenance that would need to be done for those 2 features, we plan on eventually coming back to this project and largly improving it's code quality, as well as implement more complex yet more maintainable systems and maybe implement these features once we're done.

Fully operational friends system (online/offline friends) &
Fully operational DM System for friends
