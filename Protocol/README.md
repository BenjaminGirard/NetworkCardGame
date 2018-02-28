# PROTOCOL

## Connection Part

### CLIENT CONNEXION
UsernameAskRequest : Serveur asks client to athenticate (server refuses any other request at this point).
UsernameAuth : Client send authentication request with a username.

UsernameAuthAnswer : Server send answer to authentication request.
DataFromEstabishedConnection : Server send list of rooms information after athentication.


### RoomConnection

AskCreateRoom : Client ak to create a room
CreateRoomRequestAnswer : answer to the AskCreateRoom request.

AskMovingIntoRoom : Client asks to move in a specific room.
MovingIntoRoomAnswer : Server answer for AskMovingIntoRoom.

LeaveRoom : Client leaves the room (party is cancelled if started and PartyCancel + ClientConnectionRoomEvent requests will be echoed to other players in room (Game  data should reset)).

### NOTIFICATIONS

ClientConnectionRoomEvent : Server send information about connection/disconnection of a client from a room
							(room deleted if last client disconnected).
RoomCreated : a room has been created (containing the RoomInfo) and send a ClientConnectionRoomEvent request (player who created the room is automatically moved into it).

### Commons

Roominfo : informations about a room
Player : Informations about a player


## Game Part


### Notifications

AreYouReady : ping client to ask if he's ready when the room has four players.
IAmReady: ping server to indicate that player is ready.
CardDistributed : single card distributed to a client.
PartyCancelled : The party is cancelled due to disconnection (all the game data must be reset).
GameCancelled : The game is cancelled (all the players skipped their turn during the calling phase. Game data should be partially reset).

YourTurnCall : Client turn to make a call or pass during Announcement phase.
YourTurnPlayCard : Client turn to Play a Card.

TurnOver : specifying clients when the turn is over (clean cards on table).
GameOver : the game is over (this class send a resume of the points of the players).

### Common

Card : Informations about a card

### Player Actions

CardPlayed : card played by a player (echoed to the room)
Call : call (small, guard, guard without, guard against, pass)