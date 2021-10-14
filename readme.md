###############################################################################################################
##                                SeaBattle Muliplayer Portbale by .Net Fraemwork                            ##          
###############################################################################################################

---------------------------------------------------------------------------------------------------------------
---                                                   SUMMARY                                          ---
---------------------------------------------------------------------------------------------------------------

The Sea Battle game is a classic version of the popular school's years game in multiplayer mode between 
two players over TCP protocol. The MVC pattern was used to implement the project. The peculiarity of the game 
is that the players take turns regardless of the successful hit. You may remake this game with your own ideas.
It's realy work on Linux OS.
For it Install wine32, winetricks and dotnet48 with winetricks

---------------------------------------------------------------------------------------------------------------
---                                                HOW TO PLAY ?                                            ---
---------------------------------------------------------------------------------------------------------------

1. Open SeaBattle.exe in ...\SeaBattle\bin\Debug\
2. Create profile. Set your nickname in special field and change your avatar by clicking on the picture of the 
ship's rudder.
3. Create server or connect to him.If you chosen creating server, You must have static IPv4 and possibility to 
open ports.
4. Wait ,when your rival will connect to you.
5. Position your ships.To do this, click the button below corresponding to your ship. And in your field on the 
left, draw this ship in any orientation by cells. Remember that 1 decked ships -4, 2 decked -3, 3 decked -2, 4 
decked -1. All of them should be indented in 1 cell on each side of each other.When you are done, click on the
 red button on the right under the opponent's field and wait for the start of the fight.
6. The transmission of the move is strictly alternating. If you hit or hit you, the move is transferred to the 
opponent of the one you hit. As soon as all the ships of one of the parties are completely destroyed, the game 
will end. A button will appear on top next to the avatar, offering the opponent to replay the fight.

---------------------------------------------------------------------------------------------------------------
---                                               PROJECT STRUCTURE                                         ---
---------------------------------------------------------------------------------------------------------------

The game is based on the MVC pattern. There are 3 folders in the project: Models, Views and Controllers. 
Controllers launch game windows and intercept user actions in the game. NetController launches the main window 
and intercepts button clicks to establish a connection. Tcp Connection is a model for establishing a connection 
between a client and a server, as well as creating a player profile. All information is output to the Connection 
Viewer view. Game Controller launches a window for the placement of ships and combat, tracking user clicks and 
transmitting them to the SeaBattle model, the main game model. It uses the Ships and Battle models. Visual 
information about the game is displayed in the BattleViewer view. In the case of a new game, Game Controller
 calls the NewGame view.
----------------------------------------------------------------------------------------------------------------
[![Watch the video](https://github.com/pavelinfsec2020/seabattle/blob/main/screen.png)](http://www.youtube.com/watch?v=-IvYkM3O6PU&t=16s)