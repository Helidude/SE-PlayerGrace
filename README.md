# Space Engineers PlayerGrace (SE-PlayerGrace)

A simple plugin that sets players LastLoginTime to the current date. 
That way players and grids wont be deleted from the "!identity purge" command when on extended leave.

For Torch server manager (https://github.com/TorchAPI)
Removed the GUI control for now. 

## Commands
!grace toggle
Toggle AutoRemove State. 
If enabled(True), players will be removed from PlayerGrace when they log back in on the server.

!grace add
Grant a player extended leave.

!grace add persist
Grant a player extended leave. Player will not be affected by AutoRemove.

!grace remove
Revoke a players extended leave. Manually removes player from PlayerGrace.

!grace list
List players on leave and Playergrace AutoRemove status.