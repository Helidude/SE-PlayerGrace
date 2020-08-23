# Space Engineers PlayerGrace (SE-PlayerGrace)

A simple plugin that sets players LastLoginTime to the current date.<br />
That way players and grids wont be deleted from the "!identity purge" command when on extended leave.

For Torch server manager (https://github.com/TorchAPI)<br />
Removed the GUI control for now.

## Commands
!grace toggle<br />
Toggle AutoRemove State.<br />
If enabled(True), players will be removed from PlayerGrace when they log back in on the server.

!grace add<br />
Grant a player extended leave.

!grace add persist<br />
Grant a player extended leave. Player will not be affected by AutoRemove.

!grace remove<br />
Revoke a players extended leave. Manually removes player from PlayerGrace.

!grace list<br />
List players on leave and PlayerGrace AutoRemove status.