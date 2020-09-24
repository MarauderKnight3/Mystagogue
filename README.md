# Mystagogue
A private Terraria hacked client source program.


s (Enters a list of commands.)
i (Spawn an item.)
search (Returns all items with a name containing the concatenated arguments.)
sl (Hosts a spawnlist of items in the recipe section, all crafted with a copper coin each. <search term>[comma]<tags> e.g. "sl item,depreciated" or "sl chloro , melee" or "sl !, damage". Using the "!" search term is name unspecific and will not sort items by name. The first comma can be placed anywhere. Unrecongized tags will be ignored. "page" will unconventionally scan the next tag as a page number, do not call it without one, as it will incorrectly interpret another unrelated tag or cause no effect at all (if it is the last tag). While the spawnlist command is in effect, other recipes will not appear. When opening a spawnlist without copper coins in the inventory, the first available inventory slot will be filled with some. Having no open slots will stop the coins from spawning altogether. Items crafted with spawnlist recipes will not get prefixes.)
reforge (Note: use "Basic" as a prefix to remove the prefix of an item.)
rename (Cursor held item vanity name. Does not affect death messages.)
ut (Set item use time(speed). Lower times will mean quicker use iterations. Values too low will break the item, but will be fixed when set higher.)
at (Set item animation time(speed). Lower times will mean faster animations. Values too low will break the item, but will be fixed when set higher.)
shoot (Set item's projectile. Some projectiles may not work with certain items. Some projectiles may not function as expected with certain items.)
auto (Held item will autoreuse, if possible.)
scale (Will change weapon visual scale and hitbox size in percentage. Reuse with no arguments to reset the size.)
stack (Set the stack size to any positive number up to 2147483647. No arguments will set it to maxStack.)
maxstack (Set the maxStack size to any positive number up to 2147483647. No arguments will set it to the default maxStack.)
dmg (Held item damage. Note: Will desync on servers and multiplayer if resulting damage comes above 32767 damage on an enemy before counting criticals. Fail Safes in the net messages have been placed, and visual damage numbers will show how much damage was reported to the server and not dealt client-side.)
crit (Held item base critical chance. Values above 100 don't have additional benefits.)
veloc (Held item projectile muzzle velocity. May not work with certain projectiles or weapons, and some may not work as expected. Bullets and Arrows have limited velocity.)
tboost (Held item tile boost for tools and placeables in blocks, e.g. pickaxe reach.)
pick (Held item pickaxe power.)
axe (Held item axe power.)
hammer (Held item hammer power.)
toolgod (The first tool of each type will be buffed (the same tool buffed twice or thrice because of having two or three different tool stats is possible). Using item refresh commands will reactivate this, and changing the first tool item of each type will re-assign target tools.)
ri (Refresh the held item)
ris (Refresh all items)
setstacks2b (All items in personal storage and inventories with more than a max stack of 1 will have their stack set to 2b, excluding some things like dye slots.)
setstackslegit (All items in personal storage and inventories will be stack regulated.)
fvt (Favorite all inventory items.)
clr (Vaporizes all unfavorited items in the inventory and all items in the void pocket. Players should put what they want to keep in their inventory and favorite those items before using this command.)
refills (All favorited items in the inventory are refilled when they fall short of a max stack.)
god (Don't collide with hazards, take damage, etc. and possess undepleteable mana. The player can still receive debuffs.)
buddha (0-36000 anomalous regeneration. Each point is one health per second. 36000 will heal 600 health instantly, while 10 will give 10 health per second.)
nomanacost (Your mana cost buff will reach 100% and mana items will no longer cost mana.)
infflight (Flight and rocket boot timers are reset to a small value incessantly, allowing for infinite flight.)
boost (Many variables are changed, making the player faster and more agile, up to level 7 from default 0 power multiplier.)
jesus (Walk on still liquids.)
maxminions (Set player's max minions, up to 1000. It is not possible to have more than this many projectiles in a game at once, so that's where the limit is.)
givebuffs ("melee", "ranged", "magic", "summon" options. This command will give the best buffs for the chosen class option.)
killdebuffs (Kills debuffs, and makes the player immune to further debuffs.)
banless (Kills Frozen, Webbed, and Stoned, and makes the player immune to further occurences of Frozen, Webbed, and Stoned.)
p2pmaphack (See and be able to teleport to all other players on the map, regardless of PVP and team affiliation.)
magnet (Will attempt to move all items to you and also reserve them for you. Experimental)
flashlight (Point a light at your cursor, revealing blocks.)
nrt (Respawn instantly.)
tps (Teleport setting (numerical). Options 0-2.)

Capabilities:
Right click an item while in favorite item mode (holding alt while the chat is closed) to duplicate that item into the cursor. Simultaneously hold control to make a 2b stack instead of a full stack.
Right click to teleport while Teleport Setting is 1 or 2.
Depreciated items will not be deleted.
Player data will be updated regularly on servers to help prevent desync.
